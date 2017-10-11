using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //

    [Serializable]
    [XmlRoot("Storage")]
    public class Storage
    {
        public Storage()
        {
            _data = new List<IpAddressInfo>();
        }

        public List<IpAddressInfo> Clone()
        {
            List<IpAddressInfo> result = new List<IpAddressInfo>();
            {
                foreach(IpAddressInfo entry in _data)
                {
                    result.Add(entry.Clone());
                }
            }
            return result;
        }

        public List<IpAddressInfo> Swap(List<IpAddressInfo> value)
        {
            Debug.Assert(value != null);

            List<IpAddressInfo> result;
            {
                result = _data;
                _data  = value;

                // swap ALWAYS saves data to disk
                Storage.SaveToDisk(this);
            }
            return result;
        }

        private List<IpAddressInfo> _data;
        public List<IpAddressInfo> Data
        {
            get { return _data; }
        }

        private static string GetDiskPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Diladele",
                "Active Directory Inspector",
                "data.xml"
           );
        }

        public static Storage LoadFromDisk()
        {
            // construct path
            string path = GetDiskPath();
            
            // and deserialize it
            var result = new Storage();
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Storage));

                    using (StreamReader reader = new StreamReader(path))
                    {
                        result = (Storage)serializer.Deserialize(reader);
                    }
                }
                catch(Exception e)
                {
                    // TODO: write to log
                }
            }
            return result;
        }

        public static void SaveToDisk(Storage storage)
        {
            string cur_path = GetDiskPath();
            string new_path = cur_path + ".tmp";

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
            XmlSerializer ser = new XmlSerializer(typeof(Storage));
            using(TextWriter writer = new StreamWriter(new_path))
            {
                ser.Serialize(writer, storage);
            }

            // good now move the new file into the old one
            if (File.Exists(cur_path))
                File.Delete(cur_path);
            File.Move(new_path, cur_path);
        }
    }
}
