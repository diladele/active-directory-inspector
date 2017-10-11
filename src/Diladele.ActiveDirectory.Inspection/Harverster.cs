using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading;

namespace Diladele.ActiveDirectory.Inspection
{
    public class WorkstationInfo
    {
        public string CN;
        public string DN;
        public string DnsHostName;
        public Int64  LastLogon;
        public string Name;
    }
    //
    // 
    //
    public class Harvester
    {
        public List<WorkstationInfo> GetWorkstations()
        {
            List<WorkstationInfo> result = new List<WorkstationInfo>();

            using (DirectoryEntry root = new DirectoryEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(root))
                {
                    searcher.PropertiesToLoad.AddRange(new[] { "cn", "distinguishedName", "dNSHostName", "lastLogon", "name" });
                    searcher.PageSize      = 1000;
                    searcher.ClientTimeout = new TimeSpan(0, 0, 20);
                    searcher.Filter = "(objectCategory=computer)";

                    var entries = searcher.FindAll();
                    foreach (SearchResult entry in entries)
                    {
                        WorkstationInfo info = new WorkstationInfo();
                        {
                            if (entry.Properties["cn"].Count > 0)
                                info.CN = (string)entry.Properties["cn"][0];

                            if (entry.Properties["distinguishedName"].Count > 0)
                                info.DN = (string)entry.Properties["distinguishedName"][0];

                            if (entry.Properties["dNSHostName"].Count > 0)
                                info.DnsHostName = (string)entry.Properties["dNSHostName"][0];

                            if (entry.Properties["lastLogon"].Count > 0)
                                info.LastLogon = (Int64)entry.Properties["lastLogon"][0];

                            if (entry.Properties["name"].Count > 0)
                                info.Name = (string)entry.Properties["name"][0];
                        }
                        result.Add(info);
                    }
                }
            }
            return result;
        }
    }
}
