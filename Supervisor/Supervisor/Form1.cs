using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Supervisor
{
    public partial class Form1 : Form
    {
        private static System.Windows.Forms.Label label2;
        TCPLineClient client = new TCPLineClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2 = new System.Windows.Forms.Label();
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(0, 51);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(35, 13);
            label2.TabIndex = 0;
            label2.Text = "label2";
            this.Controls.Add(label2);

            client.Init("localhost", 5666);
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.AddToSendQueue("start;1;");
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            client.AddToSendQueue("stop;1;");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            client.Update();

            if (client.HasData())
            {
                string[] tokens = client.Get().Split(';');
                if (tokens[0] == "running")
                {
                    this.Notepad.BackColor = Color.Green;
                }
                else if (tokens[0] == "stopped")
                {
                    this.Notepad.BackColor = Color.Red;
                }

            }

            if (client.State() == TCPLineClient.StateType.CONNECTED)
            {
                this.Start.Enabled = true;
                this.Stop.Enabled = true;
            }
            else
            {
                this.Start.Enabled = false;
                this.Stop.Enabled = false;
                this.Notepad.BackColor = Color.Gray;
            }

        }
    }
}
