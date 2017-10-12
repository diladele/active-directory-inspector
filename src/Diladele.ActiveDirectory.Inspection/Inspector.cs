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

                // construct the event log listener
                _listener = new Listener();
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
            }
        }

        private System.Object _guard = new System.Object();
        private Storage    _storage  = new Storage();
        private bool _stopping = false;
        private bool _active = false;
        
        private Timer    _timer;        
        private Listener _listener;
        
        
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
        // may and will through exceptions when needed (wrapped by safe function)
        //
        private void OnTimerElapsed(Object state)
        {
            // these are copies of user activity and storage
            List<Activity> activities;
            Storage        storage;
            {
                lock (_guard)
                {
                    activities = _listener.GetActivities();
                    storage    = Storage.Clone(_storage);
                }
            }

            // we have some event log records, adjust the storage based on each (quick)
            storage.Update(activities);

            // harvest workstations (quick)
            var workstations = Harvester.Harvest();

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
                storage.Probe(workstation);
            }

            // finally if we got here then everything in data is fresh and probed, replace it
            lock (_guard)
            {
                // swap the data in the class
                _storage = storage;

                // and save it on disk
                Storage.SaveToDisk(_storage);
            }

            // fine, let's wait for the next timer fire
        }
    }
}
