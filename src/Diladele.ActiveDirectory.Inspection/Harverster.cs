using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    // 
    //
    public class Harvester
    {
        public static List<Workstation> Harvest()
        {
            // this is the value to return
            List<Workstation> result = new List<Workstation>();

            // connect to LDAP
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
                        Workstation w = new Workstation();
                        {
                            if (entry.Properties["cn"].Count > 0)
                                w.CommonName = (string)entry.Properties["cn"][0];

                            if (entry.Properties["distinguishedName"].Count > 0)
                                w.DistinguishedName = (string)entry.Properties["distinguishedName"][0];

                            if (entry.Properties["dNSHostName"].Count > 0)
                                w.DnsHostName = (string)entry.Properties["dNSHostName"][0];

                            if (entry.Properties["lastLogon"].Count > 0)
                                w.LastLogon = (Int64)entry.Properties["lastLogon"][0];

                            if (entry.Properties["name"].Count > 0)
                                w.Name = (string)entry.Properties["name"][0];
                        }
                        result.Add(w);
                    }
                }
            }
            return result;
        }
    }
}
