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
        public Inspector(IStorage storage)
        {
            // debug check
            Debug.Assert(storage != null);
            
            // create members
            _guard     = new System.Object();
            _storage   = storage;
            
            // manual events to end the thread (not set) and first time event (set)
            _exitEvent = new ManualResetEvent(false);
            _firstTime = new ManualResetEvent(true);

            // thread itself
            _thread = new Thread(new ThreadStart(ThreadProc));
            _thread.Start();
        }

        public void Dispose()
        {
            // lock the object
            lock (_guard)
            {
                // check if we are already disposed
                if (_disposed)
                    return;

                // mark as disposed (actually disposing but anyway)
                _disposed = true;
            }

            // copy thread handle to stack variable
            Thread thread;
            {
                lock (_guard)                
                {
                    // safely get the thread handle
                    thread  = _thread;
                    _thread = null;

                    // tell the thread to stop
                    _exitEvent.Set();
                }
            }
            
            // wait for the thread to stop being outside of lock
            thread.Join();
            
            // lock the object
            lock (_guard)
            {
                // debug check
                Debug.Assert(_disposed == true);

                // and clear all
                _exitEvent.Dispose();
                _firstTime.Dispose();
            }
        }

        private ManualResetEvent _exitEvent;
        private ManualResetEvent _firstTime;
        private System.Object _guard;
        private Thread _thread;
        private bool _disposed;

        //private Storage    _storage  = new Storage();
        //private bool _stopping = false;
        //private bool _active = false;
        
        //private Timer      _timer;        
        private IStorage   _storage;


        private void ThreadProc()
        {
            // get the events
            ManualResetEvent exitEvent;
            ManualResetEvent firstTime;
            {
                lock (_guard)
                {
                    exitEvent = _exitEvent;
                    firstTime = _firstTime;
                }
            }

            // loop until forever (wake up every 15 seconds)
            while(true)
            {
                int res = WaitHandle.WaitAny(new WaitHandle[] { exitEvent, firstTime }, 15* 1000);
                switch(res)
                {
                    case 1:
                        {
                            // it is first time run, reset the event so that it never fires again
                            firstTime.Reset();

                            // and do the same as on normal timeout
                            goto case WaitHandle.WaitTimeout;
                        }

                    case WaitHandle.WaitTimeout:
                        {
                            // good, copy out the variables
                            IStorage   storage;
                            {
                                lock (_guard)
                                {
                                    storage   = _storage;
                                }
                            }

                            // and run the tick safely
                            try
                            {
                                OnThreadTick(storage);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceWarning("Inspector - ignoring error: {0}", e.Message);
                            }
                        }
                        continue;

                    case 0:  break;
                    default: break;
                }

                // if we got here, thread need to stop
                return; 
            }
        }

        //
        // may and will through exceptions when needed (wrapped by safe function)
        //
        private void OnThreadTick(IStorage storage)
        {
            /*
             * // mark start
            DateTime start = DateTime.Now;

            // trace start
            Trace.TraceInformation("----------------------------------------------");
            Trace.TraceInformation(" TIMER TICK START at {0}", start.ToString());
            Trace.TraceInformation("----------------------------------------------");
            
            // get the logon/logoff activities and update the storage
            List<Activity> activities = listener.PopActivities();
            {
                (new Updater(storage)).Update(activities);
            }

            // now the storage is updated with event log activities, some workstations *may* have beed marked probe now, let's do the probing
            

            // harvest workstations (quick) being outside of lock
            var workstations = Harvester.Harvest();

            // now probe each workstation (slow)
            foreach (var workstation in workstations)
            {
                // make a local clone of the storage                
                Storage storage = null;

                // lock the object
                lock (_guard)
                {
                    // each probe may take a lot of time, so check for bail out
                    if (_stopping)
                        return;

                    // clone it
                    storage = Storage.Clone(_storage);
                }

                // probe and possible add to data (take a lot of time!)
                storage.Probe(workstation);

                // ok probe completed, refresh the main storage right now so that possible callers get a fresh view
                lock (_guard)
                {
                    // swap the data in the class
                    _storage = storage;

                    // and save it on disk
                    Storage.SaveToDisk(_storage);
                }
            }
             * * */

            // mark end
            DateTime end = DateTime.Now;

            Trace.TraceInformation("----------------------------------------------");
            //Trace.TraceInformation(" TIMER TICK END at {0}, took: {1}", end.ToString(), (end - start).ToString());
            Trace.TraceInformation("----------------------------------------------");
             
        }
    }
}
