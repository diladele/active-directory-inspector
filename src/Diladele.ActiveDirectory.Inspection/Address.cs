using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    public class Address
    {
        [XmlIgnore]
        public IPAddress  IP;                           // null if not initialized

        [XmlElement("IP")]
        public string SerializedIP
        {
            get { 
                return this.IP.ToString(); 
            }
            set {
                if(string.IsNullOrEmpty(value))
                    throw new Exception("Cannot set empty string as Address.IP value");

                this.IP = IPAddress.Parse(value);
            }
        }

        // public DateTime   NextProbeTime;                // default never? 0?
        public List<User> Users = new List<User>();

        // debugging information about the workstation this address is from (empty if address was created from event log notification)
        public string     CommonName;
        public string     DistinguishedName;
        public string     DnsHostName;
        public Int64      LastLogon;
        public string     Name;
    }
}
