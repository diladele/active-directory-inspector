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
    public class Address //: ICloneable
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

        public DateTime   NextProbeTime;                // default never? 0?
        public List<User> Users = new List<User>();

        // workstation information
        public string     CommonName;
        public string     DistinguishedName;        // we consider this to be unique id
        public string     DnsHostName;
        public Int64      LastLogon;
        public string     Name;


        /*
        public object Clone()
        {
            Address result = new Address();
            {
                // copy all simple members
                result.IP = this.IP;
                result.NextProbeTime = this.NextProbeTime;
                
                // copy list
                result.Users = new List<User>();
                foreach (var user in this.Users)
                {
                    result.Users.Add((User)user.Clone());
                }
            }
            return result;
        }*/
    }
}
