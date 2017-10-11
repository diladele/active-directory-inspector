using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    // should be one and only in the process 
    //
    public class Inspector : IDisposable
    {
        public Inspector()
        {
            // load the storage from disk
            lock (_guard)
            {
                // load existing storage from disk
                _storage = Storage.LoadFromDisk();

                // construct the listener
                _listener = new EventLogListener();

                // construct the harvester
                _harverster = new Harvester();
            }

            // and start the timer
            _timer = new Timer(this.OnTimerElapsedSafe, null, 1000, 250);
        }

        public void Dispose()
        {
            Timer timer = null;
            {
                lock (_guard)
                {
                    // mark as stopping
                    _stopping = true;

                    // and get the timer
                    timer  = _timer;
                    _timer = null;
                }
            }

            // stop the timer being OUTSIDE of lock
            timer.Dispose();

            // lock us again 
            lock (_guard)
            {
                // and clean up
                _listener   = null;
                _harverster = null;
            }
        }

        private System.Object _guard = new System.Object();
        private Storage    _storage  = new Storage();
        private bool _stopping = false;
        private bool _active = false;
        
        private Timer            _timer;
        private Harvester        _harverster;
        private EventLogListener _listener;
        
        
        private void OnTimerElapsedSafe(Object state)
        {
            // timer may fire even if the previous timer routing is still running, so here we check the flag
            lock (_guard)
            {
                if(_active)
                {
                    // ok another timer is active, return without doing anything
                    return;
                }
                else
                {
                    // no timers are active, mark us as the running one
                    _active = true;
                }
            }

            try
            {
                // call the exception unsafe routine
                OnTimerElapsed(state);
            }
            finally
            {
                // reset the active flag
                lock (_guard)
                {
                    if (_active)
                        _active = false;
                }
            }
        }

        //
        // may and will through exceptions at will (wrapped by safe function)
        //
        private void OnTimerElapsed(Object state)
        {
            // these are copies of event log records and and storage data
            List<InfoBase>      records;
            List<IpAddressInfo> data;
            {
                lock (_guard)
                {
                    records = _listener.GetEvents();
                    data    = _storage.Clone();
                }
            }

            // we have some event log records, adjust the storage based on each (quick)
            (new DataUpdater()).Update(data, records);

            // harvest workstations (quick)
            var workstations = (new Harvester()).GetWorkstations();

            // now probe each workstation (slow)
            foreach(var workstation in workstations)
            {
                // each update may take a lot of time, so check for bail out
                lock (_guard)
                {
                    if(_stopping)
                        return;
                }

                // probe and possible add to data
                (new DataProber()).Probe(data, workstation);
            }

            // finally if we got here then everything in data is fresh and probed, replace it
            lock (_guard)
            {
                // swap the data in the class
                _storage.Swap(data);
            }

            // fine, let's wait for the next timer fire
        }
    }
}
