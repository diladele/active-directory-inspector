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
        public Listener()
        {
            _log = new EventLog("Security");
            {
                _log.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
                _log.EnableRaisingEvents = true;                
            }
        }

        public List<Activity> GetActivities()
        {
            List<Activity> result = null;

            lock (_guard)
            {
                result = _queue;
                _queue = new List<Activity>();
            }

            return result;
        }

        private List<Activity> _queue = new List<Activity>();
        private System.Object      _guard = new System.Object();
        private EventLog           _log;

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

                        case 4634:
                            HandleLogoffEvent(entry);
                            break;

                        case 4647:
                            HandleUserInitiatedLogoffEvent(entry);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e1)
                {
                    Debug.Write(e1.Message);
                }
            }
        }

        private void HandleUserInitiatedLogoffEvent(EventLogEntry entry)
        {
 	        //throw new NotImplementedException();
        }

        private void HandleLogoffEvent(EventLogEntry entry)
        {
 	        //throw new NotImplementedException();
        }

        private void HandleLogonEvent(EventLogEntry entry)
        {
            LoggedOn activity = (new ActivityParser()).ParseLogonEvent(entry);
            {
                _queue.Add(activity);
            }
        }
    }
}
