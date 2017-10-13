using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    public class Listener
    {
        public Listener(IStorage storage)
        {
            _guard   = new System.Object();
            _storage = storage;
            _log     = new EventLog("Security");
            {
                _log.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
                _log.EnableRaisingEvents = true;                
            }
        }

        //private List<Activity> _queue = new List<Activity>();
        private System.Object  _guard;
        private IStorage       _storage;
        private EventLog       _log;

        private void OnEntryWritten(object sender, EntryWrittenEventArgs e)
        {
            lock (_guard)
            {
                // no exception should leave the handler
                try
                {
                    // we only process some events
                    EventLogEntry entry = e.Entry;
                    switch (entry.InstanceId)
                    {
                        case 4624:
                            HandleLogonEvent(entry);
                            break;

                            /*

                        case 4634:
                            HandleLogoffEvent(entry);
                            break;

                        case 4647:
                            HandleUserInitiatedLogoffEvent(entry);
                            break;
                             * */

                        default:
                            break;
                    }
                }
                catch (Exception e1)
                {
                    Trace.TraceWarning(e1.Message);
                }
            }
        }

        private void HandleUserInitiatedLogoffEvent(EventLogEntry entry)
        {
 	        //throw new NotImplementedException();
            //Trace.TraceInformation(
             //           "Updater - processed logoff activity on IP {0}.", activity.Network_Address
              //      );
        }

        private void HandleLogoffEvent(EventLogEntry entry)
        {
 	        //throw new NotImplementedException();
        }

        private void HandleLogonEvent(EventLogEntry entry)
        {
            // get the activity
            LoggedOn activity = new LoggedOn();
            {
                var parser = new ActivityParser();
                {
                    if(!parser.ParseLogon(entry, activity))
                        return;
                }
            }

            // local activities are not interesting
            if (activity.Local)
                return;
            
            // when there is a logon, we *always* do probing
            Address address = Prober.Probe(activity.Network_Address);
            
            if(address != null)
            {
                // we probed successfully, update the storage
                _storage.Insert(address);
            }

            // and trace it
            Trace.TraceInformation(
                "Updater - processed logon activity of {0}\\{1} on {2}",
                activity.Account_Domain,
                activity.Account_Name,
                activity.Network_Address
            );
        }
    }
}
