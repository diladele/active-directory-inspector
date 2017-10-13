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
            // this is the list of users found on that ip address
            Address result = new Address();

            // issue WMI request to the remote IP
            ManagementScope scope = new ManagementScope(@"\\" + address.ToString() + @"\root\cimv2");
            ObjectQuery     query = new ObjectQuery("select * from win32_process");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    string processName = mo["name"].ToString();
                    if (!processName.EndsWith("explorer.exe"))
                        continue;

                    User user = Prober.ConstructUser(mo);
                    {
                        user.DebugProbedIpAddress = address.ToString();
                    }
                    result.Users.Add(user);
                }
            }

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
    }
}
