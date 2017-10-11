using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    public class Prober
    {
        public Prober(string ip_addr)
        {
            // debug check the IP is not empty
            Debug.Assert(ip_addr.Length > 0);

            // save it
            this.ip_addr = ip_addr;
            this.computer = this.ProbeComputer();
        }

        private string ProbeComputer()
        {
            // this is the computer name to return
            string result = "";

            // issue WMI request to the remote IP
            ManagementScope scope = new ManagementScope(@"\\" + ip_addr + @"\root\cimv2");
            ObjectQuery query = new ObjectQuery("select * from Win32_ComputerSystem");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    string dnsHostName = mo["DNSHostName"].ToString();
                    string domain      = mo["Domain"].ToString();
                    bool partOfDomain  = (bool)mo["PartOfDomain"];

                    result = dnsHostName;
                    if (partOfDomain)
                    {
                        if (domain.Length > 0)
                            result += "." + domain;
                    }
                }
            }

            // and return nicely
            return result;
        }

        public List<ProbedInfo> Probe()
        {
            // this is the result value
            List<ProbedInfo> result = new List<ProbedInfo>();

            // issue WMI request to the remote IP
            ManagementScope scope = new ManagementScope(@"\\" + ip_addr + @"\root\cimv2");
            ObjectQuery query = new ObjectQuery("select * from win32_process");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    string processName = mo["name"].ToString();
                    if (!processName.EndsWith("explorer.exe"))
                        continue;

                    result.Add(this.ConstructInfo(mo));
                }
            }

            // and return nicely
            return result;
        }

        private string ip_addr;
        private string computer;

        private ProbedInfo ConstructInfo(ManagementObject mo)
        {
            ProbedInfo info = new ProbedInfo();

            info.ProbedProcessID = mo["ProcessId"].ToString();
            info.ProbedProcessPath = mo["CommandLine"].ToString();
            info.ProbedIpAddress = this.ip_addr;
            info.ProbedComputer = this.computer;

            // get owner
            {
                object[] args = { string.Empty, string.Empty };

                int res = Convert.ToInt32(mo.InvokeMethod("GetOwner", args));
                if (0 == res)
                {
                    info.UserName = args[0].ToString();
                    info.Domain = args[1].ToString();
                }
            }

            // get sid
            {
                object[] args = { string.Empty };
                int res = Convert.ToInt32(mo.InvokeMethod("GetOwnerSid", args));
                if (0 == res)
                {
                    info.UserSid = args[0].ToString();
                }
            }

            // and return nicely
            return info;
        }
    }
}
