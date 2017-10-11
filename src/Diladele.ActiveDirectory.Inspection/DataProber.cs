using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;

namespace Diladele.ActiveDirectory.Inspection
{
    class DataProber
    {
        public void Probe(List<IpAddressInfo> data, WorkstationInfo workstation)
        {
            // first of all, see if we have this workstation in the list
            IpAddressInfo to_probe = null;
            {
                foreach(var entry in data)
                {
                    if (entry.HostName.ToUpper() == workstation.DnsHostName.ToUpper())
                        to_probe = entry;
                }
            }

            if (to_probe != null)
            {
                // ok workstation is there, see if it is time to probe it
                bool need_probing = false;
                {
                    if (to_probe.NextProbeTime < DateTime.Now)
                        need_probing = true;
                }
                if (!need_probing)
                    return;

                // do the probing
                if (!ProbeExisting(to_probe))
                {
                    // probe failed; the station *may* be offline, throw it away
                    data.Remove(to_probe);
                }
            }
            else
            {
                // workstation is not there, let's resolve it and probe
                IpAddressInfo new_ip = ProbeNew(workstation.DnsHostName);
                if (new_ip != null)
                {
                    // good; add it to data
                    data.Add(new_ip);
                }
            }
        }

        private bool ProbeExisting(IpAddressInfo to_probe)
        {
            IpAddressInfo new_ip = ProbeNew(to_probe.HostName);
            if (new_ip != null)
            {
                // update fields (todo: redesign)
                to_probe.IpAddress         = new_ip.IpAddress;
                to_probe.SamAccountName    = new_ip.SamAccountName;
                to_probe.UserSid           = new_ip.UserSid;
                to_probe.UserPrincipalName = new_ip.UserPrincipalName;
                to_probe.HostName          = new_ip.HostName;
                to_probe.NextProbeTime     = new_ip.NextProbeTime;

                return true;
            }
            return false;
        }

        private IpAddressInfo ProbeNew(string hostname)
        {
            try
            {
                // get IP address (for now first one)
                string ip_addr = "";
                {
                    IPHostEntry he = Dns.GetHostEntry(hostname);
                    if (he.AddressList.Length > 0)
                    {
                        IPAddress ip = he.AddressList[0];
                        {
                            ip_addr = ip.ToString();
                        }
                        // Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                        // s.Connect(ip, 80);
                    }
                }

                // run the prober
                List<ProbedInfo> info = (new Prober(ip_addr)).Probe();

                if (info.Count > 0)
                {
                    // take the first one for now
                    IpAddressInfo result = new IpAddressInfo();
                    {
                        result.IpAddress         = info[0].ProbedIpAddress;
                        result.SamAccountName    = info[0].UserName;
                        result.UserPrincipalName = info[0].UserSid;
                        result.UserSid           = info[0].UserSid;
                        result.HostName          = info[0].ProbedComputer;
                        result.NextProbeTime     = DateTime.Now + new TimeSpan(0, 0, 15); // next 15 seconds
                    }
                    return result;
                }
            }
            catch(Exception e)
            {
                // TODO: write e
            }
            return null;
        }
    }
}
