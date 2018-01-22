using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Supervisor
{
   /* This class is for control of one process */
   class ProcessControls
   {
      // Label showing the process name, if it is currently started and clickable to start/stop the process
      public System.Windows.Forms.Label NameLabel = new System.Windows.Forms.Label();

      // Checkbox for starting minimized
      public System.Windows.Forms.CheckBox StartMinimizedCheckbox = new System.Windows.Forms.CheckBox();

      // Button to kill the process, if it doesn't respond to regular stop command
      public System.Windows.Forms.Button KillButton = new System.Windows.Forms.Button();                 

      private TCPLineClient client;
      private int id;

      // Constructor
      public ProcessControls(TCPLineClient client, string name, int id, int xPos, int yPos)
      {
         this.client = client;
         this.id = id;

         this.StartMinimizedCheckbox.Location = new System.Drawing.Point(xPos, yPos + 65);
         this.StartMinimizedCheckbox.Name = "StartMinimizedCheckbox" + id;
         this.StartMinimizedCheckbox.Size = new System.Drawing.Size(98, 25);
         this.StartMinimizedCheckbox.TabIndex = 0;
         this.StartMinimizedCheckbox.Text = "Start minimized";
         this.StartMinimizedCheckbox.UseVisualStyleBackColor = true;
         this.StartMinimizedCheckbox.Enabled = false;

         this.NameLabel.BackColor = System.Drawing.SystemColors.ControlDark;
         this.NameLabel.Location = new System.Drawing.Point(xPos, yPos);
         this.NameLabel.Size = new System.Drawing.Size(155, 60);
         this.NameLabel.TabIndex = 2;
         this.NameLabel.Text = name + "\n@" + client.name;
         this.NameLabel.Click += new System.EventHandler(this.NameLabel_Click);
         this.NameLabel.AutoSize = false;
         this.NameLabel.Font = new System.Drawing.Font("Arial", 16);
         this.NameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         this.NameLabel.BackColor = System.Drawing.Color.Gray;

         this.KillButton.Location = new System.Drawing.Point(xPos + 97, yPos + 65);
         this.KillButton.Name = "KillButton" + id;
         this.KillButton.Size = new System.Drawing.Size(59, 25);
         this.KillButton.TabIndex = 1;
         this.KillButton.Text = "Kill";
         this.KillButton.UseVisualStyleBackColor = true;
         this.KillButton.Click += new System.EventHandler(this.KillButton_Click);
         this.KillButton.Enabled = false;
      }

      // Called when the name-label is clicked
      private void NameLabel_Click(object sender, EventArgs e)
      {
         if (this.NameLabel.BackColor == Color.Red)
         {
            string str = "start;" + id + ";";
            if (this.StartMinimizedCheckbox.Checked == true)
            {
               str += "minimized;";
            }
            client.AddToSendQueue(str);
         }
         else if (this.NameLabel.BackColor == Color.Green)
         {
            string str = "stop;" + id + ";";
            client.AddToSendQueue(str);
         }
      }

      // Called when the kill-button is clicked
      private void KillButton_Click(object sender, EventArgs e)
      {
         string str = "kill;" + id + ";";
         client.AddToSendQueue(str);
      }
   }
}
