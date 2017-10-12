using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    public class Activity
    {
        public string    Logon_ID;
        public string    Logon_GUID;
        public string    Network_Address;       // on what IP address this event occured

    }

    class LoggedOn : Activity
    {
        public enum LogonType
        {
            Unknown = 0,
            Interactive = 2,
            Network,
            Batch,
            Service,
            Proxy,
            Unlock,
            NetworkCleartext,
            NewCredentials,
            RemoteInteractive,
            CachedInteractive,
            CachedRemoteInteractive,
            CachedUnlock
        }; 

        public string    Security_ID;
        public string    Account_Name;
        public string    Account_Domain;        
        public LogonType Logon_Type;        
    }

    class LoggedOff : Activity
    {
    }
}
