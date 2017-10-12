using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    public class User : ICloneable
    {
        public string Name; // sAMAccountName
        public string Domain;
        public string Sid;
        public string PrincipalName;
        //public string HostName;

        //public string UserName;
        

        // debug info
        public string DebugProbedIpAddress;
        public string DebugProbedComputer;
        public string DebugProbedProcessID;
        public string DebugProbedProcessPath;

        public object Clone()
        {
            User result = new User();
            {
                result.Name   = this.Name;
                result.Domain = this.Domain;
                result.Sid    = this.Sid;
                result.PrincipalName = this.PrincipalName;

                result.DebugProbedIpAddress   = this.DebugProbedIpAddress;
                result.DebugProbedComputer    = this.DebugProbedComputer;
                result.DebugProbedProcessID   = this.DebugProbedProcessID;
                result.DebugProbedProcessPath = this.DebugProbedProcessPath;
            }
            return result;
        }
    }
}
