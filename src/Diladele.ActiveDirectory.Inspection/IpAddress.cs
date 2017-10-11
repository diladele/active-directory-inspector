using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    public class IpAddressInfo
    {
        public string   IpAddress;
        public string   SamAccountName;
        public string   UserSid;
        public string   UserPrincipalName;
        public string   HostName;
        public DateTime NextProbeTime;  // default never? 0?

        public IpAddressInfo Clone()
        {
            IpAddressInfo result = new IpAddressInfo();
            {
                result.IpAddress         = this.IpAddress;
                result.SamAccountName    = this.SamAccountName;
                result.UserSid           = this.UserSid;
                result.UserPrincipalName = this.UserPrincipalName;
                result.HostName          = this.HostName;
                result.NextProbeTime     = this.NextProbeTime;
            }
            return result;
        }
    }
}
