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

        public Address Clone()
        {
            Address result = new Address();
            {
                result.IP                = this.IP;
                result.CommonName        = this.CommonName;
                result.DistinguishedName = this.DistinguishedName;
                result.DnsHostName       = this.DnsHostName;
                result.LastLogon         = this.LastLogon;
                result.Name              = this.Name;
                
                result.Users = new List<User>();
                foreach(var user in this.Users)
                {
                    result.Users.Add((User)user.Clone());
                }
            }
            return result;

        }

        // public DateTime   NextProbeTime;                // default never? 0?
        public List<User> Users = new List<User>();

        // debugging information about the workstation this address is from (empty if address was created from event log notification)
        public string     CommonName;
        public string     DistinguishedName;
        public string     DnsHostName;
        public Int64      LastLogon;
        public string     Name;

        public string AsString
        {
            get
            {
                string users = "";
                {
                    foreach(var user in this.Users)
                    {
                        users += string.Format(@" {0}\{1}", user.Domain, user.Name);
                    }
                }
                return string.Format("IP {0}, User Count: {1}, Users:{2}", this.IP.ToString(), this.Users.Count, users);
            }
        }

        public string AsJson
        {
            get
            {
                string json_ip    = string.Format("\"IP\": \"{0}\"", this.SerializedIP);
                
                List<string> tmp = new List<string>();
                foreach (var user in this.Users)
                {
                    string json_user = string.Format("\"name\": \"{0}\", \"domain\": \"{1}\"", user.Domain, user.Name);
                    tmp.Add("{" + json_user + "}");
                }

                var result = string.Join(", ", tmp.ToArray());

                return "{" + json_ip + ", \"users\": [ " + result + "] }";
            }
        }
    }
}
