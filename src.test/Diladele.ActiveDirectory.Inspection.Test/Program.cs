using Diladele.ActiveDirectory.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
            //TestRegex();
            TestWebServer();
            //TestHarvester();
            //TestListener();

            //TestInspector();            
            //TestProber();
        }

        static void TestRegex()
        {
            string url  = "http://192.168.1.103:8000/ip/lookup/192.168.1.1";
            Match match = Regex.Match(url, "/ip/lookup/(.*)/?", RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new Exception("Invalid URL in HandleIpLookUp");

            // copy out the ip address
            string ip_addr = match.Groups[1].Value;
        }

        static void TestWebServer()
        {
            // load storage from disk
            var storage = StorageFactory.LoadFromDisk();

            // and give it to web server
            using (var webserver = new WebServer(storage))
            {
                Thread.Sleep(100 * 1000);
            }
        }


        static void TestHarvester()
        {
            // load storage from disk
            var storage = StorageFactory.LoadFromDisk();

            // create harvester
            using(var harvester = new Harvester(storage))
            {
                // wait 60 seconds
                Thread.Sleep(60 * 1000);
            }

            // and save the storage
            StorageFactory.SaveToDisk(storage);
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
