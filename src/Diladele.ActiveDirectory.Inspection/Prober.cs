using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    public class Prober
    {
        public static Address Probe(IPAddress address)
        {
            log.DebugFormat("Probing address {0}", address.ToString());

            // this is the list of users found on that ip address
            Address result = new Address();
            {
                result.IP = address;
            }

            // issue WMI request to the remote IP
            ManagementScope scope = new ManagementScope(@"\\" + address.ToString() + @"\root\cimv2");
            ObjectQuery     query = new ObjectQuery("select * from win32_process");

            log.DebugFormat("Creating management searcher with scope {0}, query {1}", scope.Path, query.QueryString);

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    string processName = mo["name"].ToString();
                    if (!processName.EndsWith("explorer.exe"))
                        continue;

                    User user = Prober.ConstructUser(mo);
                    {
                        // for debug
                        user.DebugProbedIpAddress = address.ToString();

                        // log it
                        log.DebugFormat("Found user {0}\\{1} in process {2}", user.Domain, user.Name, user.DebugProbedProcessPath);
                    }
                    result.Users.Add(user);
                }
            }

            // log it again
            log.DebugFormat("Address {0} probed successfully, found {1} users.", address.ToString(), result.Users.Count);

            // and return nicely
            return result;
        }

        private static User ConstructUser(ManagementObject mo)
        {
            User user = new User();
            {
                user.DebugProbedProcessID   = mo["ProcessId"].ToString();
                user.DebugProbedProcessPath = mo["CommandLine"].ToString();
            }

            // get owner
            {
                object[] args = { string.Empty, string.Empty };

                int res = Convert.ToInt32(mo.InvokeMethod("GetOwner", args));
                if (0 == res)
                {
                    user.Name   = args[0].ToString();
                    user.Domain = args[1].ToString();
                }
            }

            // get sid
            {
                object[] args = { string.Empty };
                int res = Convert.ToInt32(mo.InvokeMethod("GetOwnerSid", args));
                if (0 == res)
                {
                    user.Sid = args[0].ToString();
                }
            }

            // and return nicely
            return user;
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
