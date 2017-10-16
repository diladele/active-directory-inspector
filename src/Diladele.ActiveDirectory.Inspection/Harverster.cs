using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    class Workstation
    {
        public string CommonName;
        public string DistinguishedName;
        public string DnsHostName;
        public Int64  LastLogon;
        public string Name;
    }

    //
    //
    //
    class WorkstationHarvester
    {
        public static List<Workstation> Harvest()
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
                    searcher.PageSize = 1000;
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

    //
    //
    //
    class WorkstationProber
    {
        public static List<Address> Probe(Workstation workstation)
        {
            // this is the result
            List<Address> result = new List<Address>();

            try
            {
                // resolve the workstation name, get all addresses from it
                foreach (IPAddress ip in Dns.GetHostAddresses(workstation.DnsHostName))
                {
                    Address address = Prober.Probe(ip);
                    
                    if (address.Users.Count == 0)
                    {
                        // there are no one logged on on that IP, skip it then
                        continue; ;
                    }

                    // debug check we have filled in the address itself
                    Debug.Assert(address.IP == ip);
                    Debug.Assert(address.Users.Count > 0);
                    
                    // assign some other fields for reference
                    address.DistinguishedName = workstation.DistinguishedName;
                    address.CommonName        = workstation.CommonName;
                    address.DnsHostName       = workstation.DnsHostName;
                    address.LastLogon         = workstation.LastLogon;
                    address.Name              = workstation.Name;

                    // and add this address
                    result.Add(address);
                }
            }
            catch (Exception e)
            {
                Trace.TraceInformation("WorkstationProber - probe failed for workstation {0}. Error: {1}", workstation.DnsHostName, e.Message);
            }

            // and return (possibly empty) list
            return result;
        }

    }

    //
    // 
    //
    public class Harvester : IDisposable
    {
        public Harvester(IStorage storage)
        {
            log.Info("Creating a new LDAP harvester...");

            // create guard
            _guard        = new System.Object();
            _storage      = storage;
            _active       = false;
            _disposed     = false;
            _workstations = new List<Workstation>();
            _timer        = new Timer(this.OnTimerElapsedSafe, null, 1000, 250);
        }

        public void Dispose()
        {
            log.Info("LDAP harvester is being disposed...");

            // this is the timer to dispose
            Timer timer = null;

            // lock the object and check the state
            lock (_guard)
            {
                if (_disposed)
                    return;

                timer     = _timer;
                _disposed = true;
            }

            // this event we use to wait for timer disposal
            WaitHandle timerDisposed = new AutoResetEvent(false);

            // tell timer to dispose being *outside* the lock
            timer.Dispose(timerDisposed);

            // wait until all callbacks are completed
            timerDisposed.WaitOne();

            // lock the object
            lock (_guard)
            {
                // debug check
                Debug.Assert(_disposed == true);
            }
        }

        private System.Object     _guard;
        private bool              _disposed;
        private bool              _active;
        private IStorage          _storage;
        private Timer             _timer;               // periodic timer runs every minute
        private List<Workstation> _workstations;        // only timer function has access to this list

        private void OnTimerElapsedSafe(Object state)
        {
            // timer may fire even if the previous timer routing is still running, so here we check the flag
            lock (_guard)
            {
                // see if another timer is active, return without doing anything
                if (_active)
                    return;
                
                // no timers are active, mark us as the running one
                _active = true;
            }

            try
            {
                // dump start
                log.Debug("LDAP harvester timer elapsed, starting processing...");

                // call the exception unsafe routine
                OnTimerElapsed(state);

                // dump end
                log.Debug("LDAP harvester timer completed.");
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

        private void OnTimerElapsed(Object state)
        {
            // get next workstation to probe
            Workstation workstation = null;
            {
                lock (_guard)
                {
                    if(_workstations.Count > 0)
                    {
                        // get one
                        workstation = _workstations[0];

                        // and pop it
                        _workstations.RemoveAt(0);
                    }
                }
            }

            // see if we were able to get something
            if(workstation != null)
            {
                log.DebugFormat("Probing workstation {0}.", workstation.DnsHostName);

                // ok we have a workstation to probe, do it
                List<Address> addresses = WorkstationProber.Probe(workstation);
                foreach(var address in addresses)
                {
                    log.DebugFormat("Workstation {0} will be added with address {1}.", workstation.DnsHostName, address.AsString);

                    lock (_guard)
                    {
                        _storage.Insert(address);
                    }
                }
            }
            else
            {
                // we do not have any workstations left, issue a call to LDAP server
                List<Workstation> workstations = WorkstationHarvester.Harvest();

                // and update the list in class
                lock (_guard)
                {
                    _workstations = workstations;
                }
            }
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
