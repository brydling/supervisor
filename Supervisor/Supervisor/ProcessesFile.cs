using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Supervisor
{
    class ProcessesFile
    {
        private String filePath;

        public class Process
        {
            public int processId;
            public int hostId;
            public String symbolicName;
        }

        public ProcessesFile(String filePath)
        {
            this.filePath = filePath;
        }

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
                        proc.processId = Convert.ToInt32(tokens[0]);
                        proc.hostId = Convert.ToInt32(tokens[1]);
                        proc.symbolicName = tokens[2];
                        processes.Add(proc);
                    }
                }
            } while (line != null);

            return processes;
        }
    }
}
