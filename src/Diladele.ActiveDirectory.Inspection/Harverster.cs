using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    // 
    //
    public class Harvester : IHarvester, IDisposable
    {
        public Harvester()
        {
            // create guard
            _guard = new System.Object();

            // manual event to end the thread (not set) and first time event (set)
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

        public List<Workstation> GetWorkstations()
        {
            List<Workstation> result = null;

            lock (_guard)
            {
                result     = _harvested;
                _harvested = null;
            }
            return result;
        }

        private ManualResetEvent  _exitEvent;
        private ManualResetEvent  _firstTime;
        private System.Object     _guard;
        private Thread            _thread;
        private bool              _disposed;
        private List<Workstation> _harvested;

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

            // loop until forever (wake up every 1 minute)
            while(true)
            {
                int res = WaitHandle.WaitAny(new WaitHandle[] { exitEvent, firstTime }, 60 * 1000);
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
                            // timeout, do the harvesting
                            List<Workstation> harvested = OnSafeThreadTick();
                            if(harvested != null)
                            {
                                // save into the parent class
                                lock (_guard)
                                {
                                    _harvested = harvested;
                                }
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

        private List<Workstation> OnSafeThreadTick()
        {
            try
            {
                return OnThreadTick();
            }
            catch(Exception e)
            {
                Trace.TraceWarning("Harvester - ignoring harvest error: {0}", e.Message);
            }
            return null;
        }

        private List<Workstation> OnThreadTick()
        {
            // trace it
            Trace.TraceInformation("Harvester - harvesting LDAP directory....");

            // this is the value to return
            List<Workstation> result = new List<Workstation>();

            // connect to LDAP
            using (DirectoryEntry root = new DirectoryEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(root))
                {
                    searcher.PropertiesToLoad.AddRange(new[] { "cn", "distinguishedName", "dNSHostName", "lastLogon", "name" });
                    searcher.PageSize      = 1000;
                    searcher.ClientTimeout = new TimeSpan(0, 0, 20);
                    searcher.Filter = "(objectCategory=computer)";

                    var entries = searcher.FindAll();
                    foreach (SearchResult entry in entries)
                    {
                        Workstation w = new Workstation();
                        {
                            if (entry.Properties["cn"].Count > 0)
                                w.CommonName = (string)entry.Properties["cn"][0];

                            if (entry.Properties["distinguishedName"].Count > 0)
                                w.DistinguishedName = (string)entry.Properties["distinguishedName"][0];

                            if (entry.Properties["dNSHostName"].Count > 0)
                                w.DnsHostName = (string)entry.Properties["dNSHostName"][0];

                            if (entry.Properties["lastLogon"].Count > 0)
                                w.LastLogon = (Int64)entry.Properties["lastLogon"][0];

                            if (entry.Properties["name"].Count > 0)
                                w.Name = (string)entry.Properties["name"][0];
                        }
                        result.Add(w);
                    }
                }
            }

            // trace it
            Trace.TraceInformation("Harvester - harvest finished, got {0} workstations.", result.Count);

            // and return
            return result;
        }
    }
}
