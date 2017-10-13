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
