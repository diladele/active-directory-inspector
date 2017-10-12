using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    public class Workstation : ICloneable
    {
        public string CommonName;
        public string DistinguishedName;        // we consider this to be unique id
        public string DnsHostName;
        public Int64  LastLogon;
        public string Name;
        public List<Address> Addresses = new List<Address>();

        public bool NeedsProbing
        {
            get
            {
                foreach(var address in this.Addresses)
                {
                    if (address.NextProbeTime < DateTime.Now)
                        return true;
                }
                return false;
            }
        }
        
        public object Clone()
        {
            Workstation result = new Workstation();
            {
                // copy all simple members
                result.DistinguishedName = this.DistinguishedName;
                result.CommonName  = this.CommonName;
                result.DnsHostName = this.DnsHostName;
                result.LastLogon   = this.LastLogon;
                result.Name        = this.Name;

                // copy list
                result.Addresses = new List<Address>();
                foreach (var address in this.Addresses)
                {
                    result.Addresses.Add((Address)address.Clone());
                }
            }
            return result;
        }
    }
}
