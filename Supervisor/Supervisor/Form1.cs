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
    public partial class Supervisor : Form
    {
        private HostsFile hostsFile = new HostsFile("hosts.ini");
        private ProcessesFile processesFile = new ProcessesFile("processes.ini");
        private System.Collections.Generic.Dictionary<int,Host> hosts = new System.Collections.Generic.Dictionary<int,Host>();

        private class Host
        {
            public int id;
            public string symbolicName;
            public string hostName;
            public TCPLineClient client;
            public bool connected = false;
            public System.Collections.Generic.Dictionary<int,ProcessControls> processControlsList = new System.Collections.Generic.Dictionary<int,ProcessControls>();
        }

        public Supervisor()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Collections.Generic.List<HostsFile.Host> hostsFromFile = hostsFile.ReadHosts();
            System.Collections.Generic.List<ProcessesFile.Process> processesFromFile = processesFile.ReadProcesses();

            foreach (HostsFile.Host h in hostsFromFile)
            {
                Host host = new Host();
                host.id = h.id;
                host.symbolicName = h.symbolicName;
                host.hostName = h.hostName;
                host.client = new TCPLineClient();
                host.client.Init(host.hostName, host.symbolicName, 5666);
                this.hosts.Add(host.id, host);
            }

            int nextX = 5, nextY = 5;

            foreach (ProcessesFile.Process p in processesFromFile)
            {
                if (hosts.ContainsKey(p.hostId) == true)
                {
                    ProcessControls processControls = new ProcessControls(hosts[p.hostId].client, p.symbolicName, p.processId, nextX, nextY);
                    hosts[p.hostId].processControlsList.Add(p.processId, processControls);

                    if (nextX > 880)
                    {
                        nextX = 5;
                        nextY += 140;
                    }
                    else
                    {
                        nextX += 205;
                    }

                    this.Controls.Add(processControls.NameLabel);
                    this.Controls.Add(processControls.StartButton);
                    this.Controls.Add(processControls.StartMinimizedCheckbox);
                    this.Controls.Add(processControls.StopButton);
                    this.Controls.Add(processControls.KillButton);
                }
            }

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (System.Collections.Generic.KeyValuePair<int,Host> pair in hosts)
            {
                Host host = pair.Value;
                host.client.Update();
                if (host.client.HasData())
                {
                    string[] tokens = host.client.Get().Split(';');
                    if (tokens[0] == "running" && tokens.Length >= 2)
                    {
                        int procId = Convert.ToInt32(tokens[1]);
                        host.processControlsList[procId].NameLabel.BackColor = Color.Green;
                        host.processControlsList[procId].StartButton.Enabled = false;
                        host.processControlsList[procId].StartMinimizedCheckbox.Enabled = false;
                        host.processControlsList[procId].StopButton.Enabled = true;
                        host.processControlsList[procId].KillButton.Enabled = true;
                    }
                    else if (tokens[0] == "stopped" && tokens.Length >= 2)
                    {
                        int procId = Convert.ToInt32(tokens[1]);
                        host.processControlsList[procId].NameLabel.BackColor = Color.Red;
                        host.processControlsList[procId].StartButton.Enabled = true;
                        host.processControlsList[procId].StartMinimizedCheckbox.Enabled = true;
                        host.processControlsList[procId].StopButton.Enabled = false;
                        host.processControlsList[procId].KillButton.Enabled = false;
                    }
                }

                if (host.connected == false && host.client.State() == TCPLineClient.StateType.CONNECTED)
                {
                    host.connected = true;
                }
                else if (host.connected == true && host.client.State() != TCPLineClient.StateType.CONNECTED)
                {
                    host.connected = false;
                    foreach (System.Collections.Generic.KeyValuePair<int, ProcessControls> processControlsPair in host.processControlsList)
                    {
                        ProcessControls processControls = processControlsPair.Value;
                        processControls.StartButton.Enabled = false;
                        processControls.StartMinimizedCheckbox.Enabled = false;
                        processControls.StopButton.Enabled = false;
                        processControls.KillButton.Enabled = false;
                        processControls.NameLabel.BackColor = Color.Gray;
                    }
                }
            }
        }
    }
}
