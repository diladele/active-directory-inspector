using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    //
    //
    //
    class ActivityParser
    {
        public LoggedOn ParseLogonEvent(EventLogEntry entry)
        {
            var result = new LoggedOn();
            {
                result.Security_ID     = this.GetReplacementString(entry, New_Logon_Security_ID);
                result.Account_Name    = this.GetReplacementString(entry, New_Logon_Account_Name);
                result.Account_Domain  = this.GetReplacementString(entry, New_Logon_Account_Domain);
                result.Logon_ID        = this.GetReplacementString(entry, New_Logon_ID);
                result.Logon_GUID      = this.GetReplacementString(entry, New_Logon_GUID);                
                result.Logon_Type      = this.ParseLogonType(entry);                
                result.Network_Address = this.GetReplacementString(entry, Source_Network_Address);
            }

            // subject info (the one who performed this operation)
            // Security_ID       = GetReplacementString(entry, Security_ID)
            // Account_Name      = GetReplacementString(entry, Account_Name)
            // Account_Domain    = GetReplacementString(entry, Account_Domain)
            // Logon_ID          = GetReplacementString(entry, Logon_ID)

            // additional logon information not yet parsed
            // Restricted Admin Mode
	        // Virtual Account:		No
	        // Elevated Token:		Yes
            // Impersonation Level:		Impersonation            
            // Linked Logon ID:		0x0
            // Network Account Name:	-
            // Network Account Domain:	-
            // Process_ID    = GetReplacementString(entry, Process_ID)
            // Process_Name  = GetReplacementString(entry, Process_Name)
            // Workstation Name:	 string
            // Source Port:		0
            // Logon_Process    = GetReplacementString(entry, Logon_Process)
            // Authentication Package:	Negotiate
            //    Transited Services:	-
            //    Package Name (NTLM only):	-
            //    Key Length:		0

            /*
            for (int index = 0; index < entry.ReplacementStrings.Length; ++index)
            {
                string replacementString = entry.ReplacementStrings[index];
                Console.WriteLine("ReplacementString {0}:{1}", index, replacementString);
            }*/
            return result;
        }

        private LoggedOn.LogonType ParseLogonType(EventLogEntry entry)
        {
            LoggedOn.LogonType result = LoggedOn.LogonType.Unknown;

            string value_str = GetReplacementString(entry, New_Logon_Type);
            if(value_str.Length > 0)
            {
                uint value = 0;
                if (UInt32.TryParse(value_str, out value))
                {
                    try
                    {
                        result = (LoggedOn.LogonType)value;
                    }
                    catch(Exception e)
                    {
                        Debug.Write(e.Message);
                    }
                }
            }

            // and return nicely
            return result;
        }

        public LoggedOff ParseLogoffEvent(EventLogEntry entry, bool userInitiated)
        {
            return new LoggedOff();
        }


        // subject
        const int Security_ID = 0;
        const int Account_Name = 1;
        const int Account_Domain = 2;
        const int Logon_ID = 3;

        // new logon
        const int New_Logon_Security_ID = 4;
        const int New_Logon_Account_Name = 5;
        const int New_Logon_Account_Domain = 6;
        const int New_Logon_ID = 7;
        const int New_Logon_Type = 8;
        const int New_Logon_GUID = 12;

        // process information
        const int Process_ID = 16;
        const int Process_Name = 17;

        // network information
        const int Source_Network_Address = 18;

        // detailed authentication information
        const int Logon_Process = 9;

        private string GetReplacementString(EventLogEntry entry, int index)
        {
            try
            {
                return entry.ReplacementStrings[index];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return "";
        }
    }
}
