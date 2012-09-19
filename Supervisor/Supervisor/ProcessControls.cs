using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Supervisor
{
    class ProcessControls
    {
        public System.Windows.Forms.Label NameLabel = new System.Windows.Forms.Label();
        public System.Windows.Forms.Button StartButton = new System.Windows.Forms.Button();
        public System.Windows.Forms.Button StopButton = new System.Windows.Forms.Button();
        public System.Windows.Forms.Button KillButton = new System.Windows.Forms.Button();

        private TCPLineClient client;
        private int id;

        public ProcessControls(TCPLineClient client, string name, int id, int xPos, int yPos)
        {
            this.client = client;
            this.id = id;

            this.StartButton.Location = new System.Drawing.Point(xPos, yPos);
            this.StartButton.Name = "StartButton"+id;
            this.StartButton.Size = new System.Drawing.Size(200, 25);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            this.StartButton.Enabled = false;

            this.NameLabel.AutoSize = true;
            this.NameLabel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.NameLabel.Location = new System.Drawing.Point(xPos, yPos + 30);
            this.NameLabel.Size = new System.Drawing.Size(200, 30);
            this.NameLabel.TabIndex = 2;
            this.NameLabel.Text = name;
            this.NameLabel.AutoSize = false;
            this.NameLabel.Font = new System.Drawing.Font("Arial", 16);
            this.NameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.NameLabel.BackColor = System.Drawing.Color.Gray;

            this.StopButton.Location = new System.Drawing.Point(xPos, yPos + 65);
            this.StopButton.Name = "StopButton"+id;
            this.StopButton.Size = new System.Drawing.Size(200, 25);
            this.StopButton.TabIndex = 1;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            this.StopButton.Enabled = false;

            this.KillButton.Location = new System.Drawing.Point(xPos, yPos + 95);
            this.KillButton.Name = "KillButton" + id;
            this.KillButton.Size = new System.Drawing.Size(200, 25);
            this.KillButton.TabIndex = 1;
            this.KillButton.Text = "Kill";
            this.KillButton.UseVisualStyleBackColor = true;
            this.KillButton.Click += new System.EventHandler(this.KillButton_Click);
            this.KillButton.Enabled = false;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            string str = "start;" + id + ";";
            client.AddToSendQueue(str);
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            string str = "stop;" + id + ";";
            client.AddToSendQueue(str);
        }

        private void KillButton_Click(object sender, EventArgs e)
        {
            string str = "kill;" + id + ";";
            client.AddToSendQueue(str);
        }
    }
}
