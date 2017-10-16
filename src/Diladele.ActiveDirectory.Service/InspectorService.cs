using Diladele.ActiveDirectory.Inspection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Runtime.InteropServices;  

namespace Diladele.ActiveDirectory.Service
{
    public partial class InspectorService : ServiceBase
    {
        public InspectorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Starting the service...");

            try
            {
                // load storage
                _storage = StorageFactory.LoadFromDisk();

                // start harvester
                _harvester = new Harvester(_storage);

                // and start listener
                _listener = new Listener(_storage);
            }
            catch(Exception e)            
            {
                // dump to log
                log.ErrorFormat("Unexpected error on service start: {0}", e.Message);

                // and rethrow it
                throw;
            }

            // tell the scm manager we are started
            ServiceInterop.ServiceStatus serviceStatus = new ServiceInterop.ServiceStatus();
            {
                serviceStatus.dwCurrentState = ServiceInterop.ServiceState.SERVICE_RUNNING;
            }
            ServiceInterop.SetServiceStatus(this.ServiceHandle, ref serviceStatus);  

            // and log the successful start
            log.Info("Service started successfully.");
        }

        protected override void OnStop()
        {
            log.Info("Stopping the service...");

            try
            {
                // stop harvester
                _harvester.Dispose();

                // dump the storage
                StorageFactory.SaveToDisk(_storage);

                // and reset all
                _harvester = null;
                _listener  = null;
                _storage   = null;
            }
            catch(Exception e)
            {
                log.WarnFormat("Ignoring unexpected error on service stop: {0}", e.Message);
            }

            log.Info("Service stopped successfully.");
        }

        private Storage   _storage;
        private Harvester _harvester;
        private Listener  _listener;
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
