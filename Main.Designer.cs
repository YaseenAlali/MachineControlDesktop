namespace MachineControlDesktop
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CaptureButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ServerControlBut = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.DelButton = new System.Windows.Forms.Button();
            this.CloseConBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CaptureButton
            // 
            this.CaptureButton.Location = new System.Drawing.Point(42, 131);
            this.CaptureButton.Name = "CaptureButton";
            this.CaptureButton.Size = new System.Drawing.Size(143, 62);
            this.CaptureButton.TabIndex = 0;
            this.CaptureButton.Text = "Capture";
            this.CaptureButton.UseVisualStyleBackColor = true;
            this.CaptureButton.Click += new System.EventHandler(this.CaptureButtonPressed);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(42, 83);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(485, 22);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Server not active";
            //this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // ServerControlBut
            // 
            this.ServerControlBut.Location = new System.Drawing.Point(357, 134);
            this.ServerControlBut.Name = "ServerControlBut";
            this.ServerControlBut.Size = new System.Drawing.Size(170, 62);
            this.ServerControlBut.TabIndex = 2;
            this.ServerControlBut.Text = "Start server";
            this.ServerControlBut.UseVisualStyleBackColor = true;
            this.ServerControlBut.Click += new System.EventHandler(this.ServerControlButClicked);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(357, 22);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(170, 22);
            this.textBox2.TabIndex = 3;
            //this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(42, 298);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(485, 327);
            this.treeView1.TabIndex = 4;
            //this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // DelButton
            // 
            this.DelButton.Location = new System.Drawing.Point(42, 223);
            this.DelButton.Name = "DelButton";
            this.DelButton.Size = new System.Drawing.Size(143, 69);
            this.DelButton.TabIndex = 5;
            this.DelButton.Text = "Delete all";
            this.DelButton.UseVisualStyleBackColor = true;
            this.DelButton.Click += new System.EventHandler(this.DelButton_Click);
            // 
            // CloseConBtn
            // 
            this.CloseConBtn.Location = new System.Drawing.Point(357, 223);
            this.CloseConBtn.Name = "CloseConBtn";
            this.CloseConBtn.Size = new System.Drawing.Size(170, 69);
            this.CloseConBtn.TabIndex = 6;
            this.CloseConBtn.Text = "Close connections";
            this.CloseConBtn.UseVisualStyleBackColor = true;
            this.CloseConBtn.Click += new System.EventHandler(this.CloseConBtn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 650);
            this.Controls.Add(this.CloseConBtn);
            this.Controls.Add(this.DelButton);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.ServerControlBut);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.CaptureButton);
            this.Name = "Main";
            this.Text = "Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button CaptureButton;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.Button ServerControlBut;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button DelButton;
        private System.Windows.Forms.Button CloseConBtn;
    }
}