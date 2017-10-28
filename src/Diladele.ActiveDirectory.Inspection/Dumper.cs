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
    public class Dumper : IDisposable
    {
        public Dumper(Storage storage)
        {
            log.Info("Creating a new storage dumper...");

            // create guard
            _guard        = new System.Object();
            _storage      = storage;
            _active       = false;
            _disposed     = false;
            _timer        = new Timer(this.OnTimerElapsedSafe, null, 1000, 250);
        }

        public void Dispose()
        {
            log.Info("Storage dumper is being disposed...");

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

        private System.Object _guard;
        private bool          _disposed;
        private bool          _active;
        private Storage       _storage;
        private Timer         _timer;               // periodic timer runs every minute
        
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
                log.Debug("Storage dumper timer elapsed, starting storage dump...");

                // call the exception unsafe routine
                OnTimerElapsed(state);

                // dump end
                log.Debug("Storage dumper timer completed.");
            }
            catch(Exception e)
            {
                log.WarnFormat("Ignoring storage dumper error: {0}", e.ToString());
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
            // get storage (it is thread safe, remember)
            Storage storage = null;
            {
                lock (_guard)
                {
                    storage = _storage;
                }
            }

            // log it
            log.DebugFormat("Dumper is saving storage to disk...");
            
            // save storage to disk
            StorageFactory.SaveToDisk(storage);
            
            log.DebugFormat("Dumper saved storage to disk successfully.");
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
