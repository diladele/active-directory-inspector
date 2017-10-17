using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    public interface IStorage
    {
        void Insert(Address a);
        bool LookUp(string ip, out Address a);
        bool Dump(out List<Address> addresses);
    }


    //
    // thread safe object - collection of addresses
    //
    [Serializable]
    [XmlRoot("Storage")]
    public class Storage : IStorage
    {
        public Storage()
        {
            // create members
            _guard     = new System.Object();
            _addresses = new List<Address>();
        }

        public void Insert(Address new_address)
        {
            lock(_guard)
            {
                // throw away the existing one
                _addresses.RemoveAll(item => item.IP.Equals(new_address.IP));

                // debug check
                foreach (var address in _addresses)
                {
                    if (address.IP.Equals(new_address.IP))
                    {
                        Debug.Assert(false);
                    }
                }

                // and add it again
                _addresses.Add(new_address);
            }
        }

        public bool LookUp(string ip, out Address address)
        {
            // assume not found
            address = null;

            // parse the ip to look for
            IPAddress value = null;
            {
                if(!IPAddress.TryParse(ip, out value))
                    return false;
            }

            lock (_guard)
            {
                Address found = _addresses.Find(item => item.IP.Equals(value));
                if (found == null)
                    return false;

                address = found.Clone();
            }

            return true;
        }

        public bool Dump(out List<Address> addresses)
        {
            addresses = new List<Address>();

            lock (_guard)
            {
                foreach (var address in _addresses)
                {
                    addresses.Add(address.Clone());
                }
            }
            return true;
        }

        public string ToXmlString()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Storage));

            using (StringWriter textWriter = new StringWriter())
            {
                ser.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        private System.Object _guard;
        public List<Address> _addresses; // it is public to be serialized :(
    }
}
