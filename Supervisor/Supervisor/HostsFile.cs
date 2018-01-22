using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Supervisor
{
   // Class for parsing the hosts config file
   class HostsFile
   {
      private String filePath;

      // Class describing a host
      public class Host
      {
         public int id;
         public String symbolicName;
         public String hostName;
      }

      // Constructor
      public HostsFile(String filePath)
      {
         this.filePath = filePath;
      }

      // Function for parsing the config file. Returns a list of hosts.
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
                  host.hostName = tokens[1];
                  host.symbolicName = tokens[2];
                  hosts.Add(host);
               }
            }
         } while (line != null);

         return hosts;
      }
   }
}
