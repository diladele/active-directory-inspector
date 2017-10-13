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

            //TestHarvester();
            TestInspector();
            //TestEventLogListener();
            //TestProber();
        }


        static void TestHarvester()
        {
            foreach(Workstation w in Harvester.Harvest())
            {
                Console.WriteLine(w.DnsHostName);
            }
        }

        static void TestInspector()
        {
            using(var inspector = new Inspector())
            {
                for (var i = 0; i < 1000; i++)
                {
                    // wait 
                    Thread.Sleep(10000);
                }
            }
        }

        static void TestEventLogListener()
        {
            var listener = new Listener();
            while (true)
            {
                // wait 
                Thread.Sleep(5000);

                // get the events accumulated
                foreach(Activity info in listener.GetActivities())
                {
                    Console.WriteLine(info);
                }
            }
        }

        static void TestProber()
        {
            var ip = IPAddress.Parse("192.168.1.103");

            List<User> users = UserProber.Probe(ip);
            foreach (var user in users)
            {
                Console.WriteLine(user.Domain);
                Console.WriteLine(user.Name);
                Console.WriteLine(user.Sid);
            }
            
        }
    }
}
