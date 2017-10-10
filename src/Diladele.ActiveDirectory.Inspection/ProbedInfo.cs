using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    public class ProbedInfo
    {
        public string UserName;
        public string UserSid;
        public string Domain;

        // debug info
        public string ProbedIpAddress;
        public string ProbedComputer;
        public string ProbedProcessID;
        public string ProbedProcessPath;
    }
}
