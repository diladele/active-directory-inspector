using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Diladele.ActiveDirectory.Inspection.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEventLogListener();
            TestProber();
        }

        static void TestEventLogListener()
        {
            var listener = new EventLogListener();
            while (true)
            {
                // wait 
                Thread.Sleep(5000);

                // get the events accumulated
                foreach(InfoBase info in listener.GetEvents())
                {
                    Console.WriteLine(info);
                }
            }
        }

        static void TestProber()
        {
            Prober prober = new Prober("192.168.1.103");
            {
                List<ProbedInfo> probes = prober.Probe();
                foreach (ProbedInfo info in probes)
                {
                    Console.WriteLine(info.Domain);
                    Console.WriteLine(info.UserName);
                    Console.WriteLine(info.UserSid);
                }
            }
        }


    }
}
