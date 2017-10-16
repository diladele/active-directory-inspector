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
    public class StorageFactory
    {
        public static Storage LoadFromDisk()
        {
            // this is the result
            Storage storage = new Storage();

            // construct path
            string path = GetDiskPath();

            // dump it
            Trace.TraceInformation("Storage is loaded from file {0}.", path);

            // and deserialize it
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Storage));

                using (StreamReader reader = new StreamReader(path))
                {
                    storage = (Storage)serializer.Deserialize(reader);
                }                
            }
            catch (Exception e)
            {
                Trace.TraceError("Error while loading storage from file {0}. Error: {1}, Stack trace: {2}", path, e.Message, e.StackTrace);
                Trace.TraceError("Storage is considered empty because of error above");
            }
            return storage;
        }




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

        private static string GetDiskPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Diladele",
                "Active Directory Inspector",
                "storage.xml"
           );
        }
    }
}
