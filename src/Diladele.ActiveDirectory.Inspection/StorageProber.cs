using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    // probes the new workstation that was just retrieved from LDAP and does not have any info assocciated 
    //
    class NewProber
    {
        public static bool Probe(Workstation workstation)
        {
            Debug.Assert(workstation.Addresses.Count == 0);

            // at least one address was probed
            bool success = false;

            try
            {
                // resolve the workstation name, get all addresses from it
                foreach (IPAddress ip in Dns.GetHostAddresses(workstation.DnsHostName))
                {
                    Address address = new Address();
                    {
                        address.IP = ip;
                    }
                    if (address.Probe())
                    {
                        // ok we probed the address successfuly, debug check
                        Debug.Assert(address.Users.Count > 0);

                        // add it to workstation
                        workstation.Addresses.Add(address);

                        // and raise the success flag
                        success = true;
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceInformation("NewProber - probe failed for workstation {0}. Error: {1}", workstation.DnsHostName, e.Message);
            }

            // may be true or false
            return success;
        }
    }

    //
    //
    //
    class ExistingProber
    {
        public static bool Probe(Workstation workstation)
        {
            // first we clear the addresses of the workstation
            workstation.Addresses.Clear();

            // let the new prober decide
            return NewProber.Probe(workstation);
        }

    }
    
    //
    //
    //
    public static class StorageProber
    {
        public static void Probe(this Storage storage, Workstation workstation)
        {
            // trace it
            Trace.TraceInformation("StorageProber - {0} | starting probe...", workstation.DnsHostName);

            // first of all, see if we have this workstation in the list
            Workstation to_probe = storage.Workstations.Find(p => p.DistinguishedName.ToUpper() == workstation.DistinguishedName.ToUpper());

            if (to_probe != null)
            {
                // ok workstation is there, see if it is time to probe it
                if (!to_probe.NeedsProbing)
                {
                    // trace it
                    Trace.TraceInformation("StorageProber - {0} | found in the storage but does not need probing yet, skipped.", workstation.DnsHostName);

                    // and do nothing
                    return;
                }

                // trace we were able to find it
                Trace.TraceInformation("StorageProber - {0} | found in the storage, probing it...", workstation.DnsHostName);

                // do the probing; note the prober updates the workstation being probed in place
                if (!ExistingProber.Probe(to_probe))
                {
                    // trace failure
                    Trace.TraceInformation("StorageProber - {0} | probe failed for existing workstation. Removed from storage.", workstation.DnsHostName);

                    // probe failed; the station *may* be offline, throw it away
                    storage.Workstations.Remove(to_probe);
                }
                else
                {
                    // trace success
                    Trace.TraceInformation("StorageProber - {0} | probe succeeded. Storage refreshed.", workstation.DnsHostName);
                }
            }
            else
            {
                // trace it
                Trace.TraceInformation("StorageProber - {0} | NOT found in the storage, it must be new, probing it...", workstation.DnsHostName);

                // workstation is not present in storage, let's resolve and probe it
                if (NewProber.Probe(workstation))
                {
                    // probed successfully, debug check
                    Debug.Assert(workstation.Addresses.Count > 0);

                    // and add it to the storage
                    storage.Workstations.Add(workstation);

                    // trace success
                    Trace.TraceInformation("StorageProber - {0} | probe succeeded for newly harvested workstation. Added it to storage.", workstation.DnsHostName);
                }
                else
                {
                    // trace failure
                    Trace.TraceInformation("StorageProber - {0} | probe failed for newly harvested workstation. Ignoring it (storage not changed).", workstation.DnsHostName);
                }
            }
        }
    }

        /*
        public static bool ProbeExisting(IpAddressInfo to_probe)
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
         * 
         */
    
}
