using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Diladele.ActiveDirectory.Inspection.Test
{
    class Program
    {
        private static string GetDiskPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Diladele",
                "Active Directory Inspector",
                "application.log"
           );
        }

        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(GetDiskPath()));
            Trace.AutoFlush = true;

            TestHarvester();
            //TestListener();

            //TestInspector();            
            //TestProber();
        }


        static void TestHarvester()
        {
            // load storage from disk
            var storage = StorageFactory.LoadFromDisk();

            // create listener
            using(var harvester = new Harvester(storage))
            {
                // wait 100 seconds
                Thread.Sleep(100 * 1000);
            }
        }

        static void TestInspector()
        {
            // load storage from disk
            var storage = StorageFactory.LoadFromDisk();

            // create listener
            var listener = new Listener(storage);

            // wait 10 seconds
            Thread.Sleep(10000);

            /*

            // create harvester
            using(var harvester = new Harvester())
            {
                // and finally inspector
                using (var inspector = new Inspector(storage, harvester, listener))
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        // wait 
                        Thread.Sleep(10000);
                    }
                }
            }
             * 
             * */
        }

        static void TestListener()
        {
            // load storage from disk
            var storage = StorageFactory.LoadFromDisk();

            // create listener
            var listener = new Listener(storage);

            while(true)
            {
                // wait 
                Thread.Sleep(5000);
            }
        }

        static void TestProber()
        {
            /*
            var ip = IPAddress.Parse("192.168.1.103");

            List<User> users = Prober.Probe(ip);
            foreach (var user in users)
            {
                Console.WriteLine(user.Domain);
                Console.WriteLine(user.Name);
                Console.WriteLine(user.Sid);
            }*/            
        }
    }
}
