using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Supervisor
{
    class HostsFile
    {
        private String filePath;

        public class Host
        {
            public int id;
            public String symbolicName;
            public String hostName;
        }

        public HostsFile(String filePath)
        {
            this.filePath = filePath;
        }

        public System.Collections.Generic.List<Host> ReadHosts()
        {
            System.Collections.Generic.List<Host> hosts = new System.Collections.Generic.List<Host>();
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
                        Host host = new Host();
                        host.id = Convert.ToInt32(tokens[0]);
                        host.symbolicName = tokens[1];
                        host.hostName = tokens[2];
                        hosts.Add(host);
                    }
                }
            } while (line != null);

            return hosts;
        }
    }
}
