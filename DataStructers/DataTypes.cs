using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MachineControlDesktop
{
    public enum CameraType
    {
        Top, I_Front, I_Back, O_Front, O_Back,
    }
    public class Device
    {
        private readonly WebSocket _Socket;
        public string IP { get; }
        public int Port { get; }
        public CameraType CameraType { get; }

        public Device(WebSocket socket, string IP, int port, CameraType cameraType)
        {
            this._Socket = socket;
            this.IP = IP;
            this.Port = port;
            this.CameraType = cameraType;
        }

        public async Task CloseSocket(string reason = "Server closed the connection")
        {
            if (IsSocketOpen())
            {
                await _Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
            }
        }

        public void SendCommand(string command = "TakePicture")
        {
            if (IsSocketOpen())
            {
                _Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(command)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        public void SendCommand(string command = "TakePicture", Action onSocketNotOpen = null)
        {
            if (IsSocketOpen())
            {
                _Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(command)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                onSocketNotOpen?.Invoke();
            }
        }

        public async Task SendCommandAsync(string command = "TakePicture")
        {
            if (IsSocketOpen())
            {
                await _Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(command)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public bool IsSocketOpen()
        {
            return _Socket.State == WebSocketState.Open;
        }

        public WebSocket GetSocket()
        {
            return _Socket;
        }
    }

}
