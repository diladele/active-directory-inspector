using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    public static class StorageUpdater
    {
        public static void Update(this Storage storage, List<Activity> activities)
        {
            // trace it
            Trace.TraceInformation("StorageUpdater - processing {0} accumulated EventLog activities....", activities.Count);

            // note this update is quick as we do not do any probing
            foreach (var activity in activities)
            {
                StorageUpdater.Update(storage, activity);
            }

            Trace.TraceInformation("StorageUpdater - all EventLog activities processed.");
        }

        public static void Update(this Storage storage, Activity activity)
        {
            // local or invalid network address are not interesting
            if (activity.Network_Address == "-" || activity.Network_Address == "::1")
                return;

            // see what activity it is
            if (activity is LoggedOn)
            {
                HandleLoggedOn(storage, (LoggedOn)activity);
            }
            else if (activity is LoggedOff)
            {
                HandleLoggedOff(storage, (LoggedOff)activity);
            }
            else
            {
                Trace.TraceWarning("StorageUpdater - ignoring activity of unknown type {0}", activity.GetType().ToString());
            }
        }

        public static void HandleLoggedOn(this Storage storage, LoggedOn activity)
        {
            // trace it
            Trace.TraceInformation("StorageUpdater - handling loggon on activity of {0}\\{1} on {2}", activity.Account_Domain, activity.Account_Name, activity.Network_Address);

            // parse the network address
            IPAddress addr;
            if(!IPAddress.TryParse(activity.Network_Address, out addr))
                return;

            // flag to show if at least one workstation was found
            bool found = false;
            
            // try to find the workstation
            foreach(var workstation in storage.Workstations)
            { 
                foreach(var address in workstation.Addresses)
                {
                    if(address.IP == addr)
                    {
                        // trace it
                        Trace.TraceInformation("StorageUpdater - found workstation {0}, by IP {1}. Resetting next probe time to Now.", workstation.DnsHostName, activity.Network_Address);

                        // reset to 'probe now'
                        address.NextProbeTime = new System.DateTime();

                        // and continue
                        found = true;
                    }
                }
            }

            if (!found)
            {
                // if we got here, no such workstation was found, dump this
                Trace.TraceWarning("StorageUpdater - could find corresponding workstation in the storage, ignoring the logon event from IP {3} by user {0}\\{1}.", activity.Account_Domain, activity.Account_Name, activity.Network_Address);

                // note, this workstation *will* be found by the next harvester event in the LDAP, so nothing to worry about
            }
        }

        public static void HandleLoggedOff(this Storage storage, LoggedOff activity)
        {
            // trace it
            Trace.TraceInformation("StorageUpdater - handling logoff on IP {0}", activity.Network_Address);

            // parse the network address
            IPAddress addr;
            if (!IPAddress.TryParse(activity.Network_Address, out addr))
                return;

            // try to find the workstation
            foreach (var workstation in storage.Workstations)
            {
                foreach (var address in workstation.Addresses)
                {
                    if (address.IP == addr)
                    {
                        // trace it
                        Trace.TraceInformation("StorageUpdater - found workstation {0}, by IP {1}. Resetting next probe time to Now.", workstation.DnsHostName, activity.Network_Address);

                        // reset to 'probe now'
                        address.NextProbeTime = new System.DateTime();
                    }
                }
            }

            // if we got here, no such workstation was found, dump this
            Trace.TraceWarning("StorageUpdater - could find corresponding workstation in the storage, ignoring the logoff event from IP {0}.", activity.Network_Address);
        }
    }
}
