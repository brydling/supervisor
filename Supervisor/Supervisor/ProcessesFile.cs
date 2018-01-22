using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Supervisor
{
   // Class for parsing the processes config file
   class ProcessesFile
   {
      private String filePath;

      // Class describing a process
      public class Process
      {
         public int processId;         // The ID of the process, as called by the local_supervisor on the host.
         public int hostId;            // The ID of the host on which this process exists.
         public String symbolicName;   // The symbolic name to attach to this process. For visual presentation.
      }

      // Constructor
      public ProcessesFile(String filePath)
      {
         this.filePath = filePath;
      }

      // Function for parsing the config file. Returns a list of processes.
      public System.Collections.Generic.List<Process> ReadProcesses()
      {
         System.Collections.Generic.List<Process> processes = new System.Collections.Generic.List<Process>();
         StreamReader reader = new StreamReader(filePath);

         string line;
         do
         {
            line = reader.ReadLine();
            if (line != null && line.Length > 0 && line[0] != '#')
            {
               string[] tokens = line.Split(';');
               if (tokens.Length >= 3)
               {
                  Process proc = new Process();
                  proc.hostId = Convert.ToInt32(tokens[0]);
                  proc.processId = Convert.ToInt32(tokens[1]);
                  proc.symbolicName = tokens[2];
                  processes.Add(proc);
               }
            }
         } while (line != null);

         return processes;
      }
   }
}
