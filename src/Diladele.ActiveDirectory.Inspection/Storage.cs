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
    [Serializable]
    [XmlRoot("Storage")]
    public class Data
    {
        public List<Workstation> Workstations = new List<Workstation>();
    }

    //
    // thread safe object - collection of addresses
    //
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

        /*
        public List<Workstation> Swap(List<Workstation> value)
        {
            Debug.Assert(value != null);

            List<Workstation> result;
            {
                result        = _workstations;
                _workstations = value;

                // swap ALWAYS saves data to disk
                Storage.SaveToDisk(this);
            }
            return result;
        }*/

        private System.Object _guard;
        private List<Address> _addresses;


        /*
        public List<Workstation> Workstations
        {
            get { return _data.Workstations; }
        }*/


        /*
        public static void SaveToDisk(Storage storage)
        {
            string cur_path = GetDiskPath();
            string new_path = cur_path + ".tmp";

            Trace.TraceInformation("Storage (workstation count {0}) is being saved to file {1}", storage.Workstations.Count, cur_path);

            // remove old file
            if (File.Exists(new_path))
            {
                File.Delete(new_path);
            }

            // recreate folder if it does not exist
            if(!Directory.Exists(Path.GetDirectoryName(new_path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(new_path));
            }

            // and serialize the storage
            XmlSerializer ser = new XmlSerializer(typeof(Data));
            using(TextWriter writer = new StreamWriter(new_path))
            {
                ser.Serialize(writer, storage);
            }

            // good now move the new file into the old one
            if (File.Exists(cur_path))
                File.Delete(cur_path);
            File.Move(new_path, cur_path);

            Trace.TraceInformation("Storage successfully saved to file {0}", cur_path);
        }*/

        /*
        public static Storage Clone(Storage v)
        {
            Storage result = new Storage();
            {
                // clone all simple members (we have none)

                // and manually clone the  list
                result._workstations = new List<Workstation>();
                foreach(var workstation in v._workstations)
                {
                    result._workstations.Add((Workstation)workstation.Clone());
                }
            }
            return result;
        }*/


        /*
        public Workstation Find(IPAddress address)
        {
            lock (_guard)
            {
                foreach (var w in _data.Workstations)
                {
                    foreach (var a in w.Addresses)
                    {
                        if (a.IP == address)
                        {
                            return (Workstation)w.Clone();
                        }
                    }
                }
            }
            return null;
        }*/


        /*
        public void Add(Workstation v)
        {
            lock (_guard)
            {
                foreach (var w in _data.Workstations)
                {
                    if(w == v)
                    {
                        _data.Workstations.Remove(w);
                        _data.Workstations.Add(v);
                    }
                }
            }
        }*/
    }
}
