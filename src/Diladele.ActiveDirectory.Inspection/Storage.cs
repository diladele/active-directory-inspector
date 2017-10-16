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
        bool Find(string ip, out string user);
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

        public bool Find(string ip, out string user)
        {
            throw new NotImplementedException();
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
        private List<Address> _addresses;
    }
}
