using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Diladele.ActiveDirectory.Inspection
{
    [Serializable]
    [XmlRoot("Storage")]
    public class Storage
    {
        public Storage()
        {
            _workstations = new List<Workstation>();
        }

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
        }

        private List<Workstation> _workstations;
        public List<Workstation> Workstations
        {
            get { return _workstations; }
        }

        private static string GetDiskPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Diladele",
                "Active Directory Inspector",
                "storage.xml"
           );
        }

        public static Storage LoadFromDisk()
        {
            // construct path
            string path = GetDiskPath();

            // dump it
            Trace.TraceInformation("Storage is being loaded from file {0}", path);
            
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

                    Trace.TraceInformation("Storage is successfully loaded from file {0}", path);
                }
                catch(Exception e)
                {
                    Trace.TraceError("Error while loading storage from file {0}. Error: {1}, Stack trace: {2}", path, e.Message, e.StackTrace);
                    Trace.TraceError("Storage is considered empty because of error above");
                }
            }
            return result;
        }

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
            XmlSerializer ser = new XmlSerializer(typeof(Storage));
            using(TextWriter writer = new StreamWriter(new_path))
            {
                ser.Serialize(writer, storage);
            }

            // good now move the new file into the old one
            if (File.Exists(cur_path))
                File.Delete(cur_path);
            File.Move(new_path, cur_path);

            Trace.TraceInformation("Storage successfully saved to file {0}", cur_path);
        }

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
        }
    }
}
