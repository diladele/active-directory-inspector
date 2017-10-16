using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;

namespace Diladele.ActiveDirectory.Inspection
{
    
    //
    //
    //

    /*
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
     * */
}
