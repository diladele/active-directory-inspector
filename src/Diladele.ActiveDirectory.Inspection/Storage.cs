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
                foreach(var address in _addresses)
                {
                    if (address.IP == new_address.IP)
                    {
                        _addresses.Remove(address);
                    }   
                }
                _addresses.Add(new_address);
            }
        }

        private System.Object _guard;
        private List<Address> _addresses;
    }
}
