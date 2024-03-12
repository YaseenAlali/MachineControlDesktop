using MachineControlDesktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace MachineControlDesktop
{
    public partial class Main : Form
    {
        private bool _localNetwork = false;
        private bool _connected = false;
        private string _DirName;
        private int _ImagesGroupingTimeInMilliseconds;

        private readonly List<Device> _Devices = new List<Device>();
        private Timer timer;
        private HttpListener httpListener;


        public Main(bool localNetwork, string dirName = "Images", int imagesGroupingTimeInMilliseconds = 60000)
        {
            InitializeComponent();
            _localNetwork = localNetwork;
            _DirName = dirName;
            _ImagesGroupingTimeInMilliseconds = imagesGroupingTimeInMilliseconds;


            textBox2.Text = _localNetwork ? "Local" : "Public";
            CreateDirIfNotExists();

            InitTimer();
            PopulateTreeView(treeView1, _DirName);
        }
        private void CreateDirIfNotExists()
        {
            if (!Directory.Exists(_DirName))
            {
                Directory.CreateDirectory(_DirName);
            }
        }
        private void InitTimer()
        {
            timer = new Timer();
            timer.Interval = 10000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private async Task HandleWebSocketCommunication(Device device)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            var webSocket = device.GetSocket();
            while (webSocket.State == WebSocketState.Open)
            {
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        do
                        {
                            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            ms.Write(buffer, 0, result.Count);
                        } 
                        while (!result.EndOfMessage);
                    }
                    catch (WebSocketException ex)
                    {
                        if (webSocket.State == WebSocketState.Aborted ||
                            webSocket.State == WebSocketState.Closed ||
                            webSocket.State == WebSocketState.CloseReceived)
                        {
                            Console.WriteLine("The client closed the connection.");
                            _Devices.Remove(device);
                            return; 
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        HandleNewImage(ms, device.CameraType);
                    }
                }
            }
        }
        private void HandleNewImage(MemoryStream ms, CameraType cameraType)
        {
            try
            {
                var base64Data = Encoding.UTF8.GetString(ms.ToArray());
                var binaryData = Convert.FromBase64String(base64Data);

                var now = DateTime.Now;
                string directoryPath = HandleNewImageDirectory(now);

                string filePath = CreateImagePath(now, directoryPath, cameraType);

                File.WriteAllBytes(filePath, binaryData);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
        private string HandleNewImageDirectory(DateTime now)
        {
            var recentDirectories = Directory.GetDirectories(_DirName)
                                             .Where(dir => now - Directory.GetLastWriteTime(dir) < TimeSpan.FromMilliseconds(_ImagesGroupingTimeInMilliseconds))
                                             .ToList();

            string directoryPath;

            if (recentDirectories.Any())
            {
                directoryPath = recentDirectories.First();
            }
            else
            {
                var directoryName = now.ToString("yyyyMMddHHmmss");
                directoryPath = Path.Combine(_DirName, directoryName);
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
        private static string CreateImagePath(DateTime now, string directoryPath, CameraType cameraType)
        {
            int counter = 1;
            var cameraTypeString = cameraType.ToString();
            var fileName = $"{now.ToString("yyyyMMddHHmmss")}{cameraTypeString}.jpg";
            var filePath = Path.Combine(directoryPath, fileName);



            while (File.Exists(filePath))
            {
                fileName = $"{now.ToString("yyyyMMddHHmmss")}{cameraTypeString}{counter}.jpg";
                filePath = Path.Combine(directoryPath, fileName);
                counter++;
            }

            return filePath;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            PopulateTreeView(treeView1, _DirName);
        }
        private void PopulateTreeView(System.Windows.Forms.TreeView treeView, string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }
        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            foreach (var file in directoryInfo.GetFiles())
                directoryNode.Nodes.Add(new TreeNode(file.Name));
            return directoryNode;
        }
        private void CaptureButtonPressed(object sender, EventArgs e)
        {

            SendMessageToAllClients(sender);    
        }
        private void SendMessageToAllClients(object sender)
        {
            for (int i = 0; i < _Devices.Count; i++)
            {
                Device device = _Devices[i];
                device.SendCommand("TakePicture", () => {
                    OnSocketCloseNotDetected(device);
                    i--;
                });
            }
        }

        private Action OnSocketCloseNotDetected(Device device)
        {
            return async () =>
            {
                await device.CloseSocket();
                _Devices.Remove(device);
            };
        }

        private async void ServerControlButClicked(object sender, EventArgs e)
        {
                if (_connected)
                {
                    await ClearConnections("Server restarted");
                }


                var ip = Networking.GetLocalIpAddress();
                var httpListener = new HttpListener();
                int port = Networking.FindFreePort();

                httpListener.Prefixes.Add($"http://{ip}:{port}/ws/");
                httpListener.Start();

                _connected = true;
                ServerControlBut.Text = "Restart server";
                textBox1.Text = $"Running on {ip}:{port}";

                while (_connected)
                {
                    var context = await httpListener.GetContextAsync();

                    if (context.Request.IsWebSocketRequest)
                    {
                        var webSocketContext = await context.AcceptWebSocketAsync(null);
                        var webSocket = webSocketContext.WebSocket;
                        var clientIp = context.Request.RemoteEndPoint.Address.ToString();
                        var clientPort = context.Request.RemoteEndPoint.Port;
                        var type = context.Request.Headers["CameraType"];
                        if (string.IsNullOrEmpty(type))
                        {
                            await HandleMissingHeaders(context, webSocket);
                            continue;
                        }
                        var device = new Device(webSocket, clientIp, clientPort, (CameraType)int.Parse(type));
                        _Devices.Add(device);
                        _ = HandleWebSocketCommunication(device);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
        }

        private static async Task HandleMissingHeaders(HttpListenerContext context, WebSocket webSocket)
        {
            context.Response.StatusCode = 403;

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Missing camera type headers", CancellationToken.None);
            }
            context.Response.Close();
        }

        private void DelButton_Click(object sender, EventArgs e)
        {
            string path = _DirName;

            try
            {
                string[] dirs = Directory.GetDirectories(path);

                foreach (string dir in dirs)
                {
                    Directory.Delete(dir, true);
                }

                string[] files = Directory.GetFiles(path);

                foreach (string file in files)
                {
                    File.Delete(file);
                }

                treeView1.Nodes.Clear();
                PopulateTreeView(treeView1, path);

                timer.Stop();
                timer.Start();
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
        }
        private async Task ClearConnections(string reason = "Server closed")
        {
            foreach (var device in _Devices)
            {
                await device.CloseSocket(reason);
            }
            _Devices.Clear();
        }
        private async void CloseConBtn_Click(object sender, EventArgs e)
        {
            await ClearConnections();
        }
    }
}
