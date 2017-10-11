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
    public class EventLogListener
    {
        public EventLogListener()
        {
            _log = new EventLog("Security");
            {
                _log.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
                _log.EnableRaisingEvents = true;                
            }
        }

        public List<InfoBase> GetEvents()
        {
            List<InfoBase> result = null;

            lock (_lock)
            {
                result = _queue;
                _queue = new List<InfoBase>();
            }

            return result;
        }

        private EventLog       _log;
        private List<InfoBase> _queue = new List<InfoBase>();
        private System.Object  _lock  = new System.Object();

        private void OnEntryWritten(object sender, EntryWrittenEventArgs e)
        {
            lock (_lock)
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
            LogonInfo info = (new EventLogEntryParser()).ParseLogonEvent(entry);
            {
                _queue.Add(info);
            }
        }  
    }
}
