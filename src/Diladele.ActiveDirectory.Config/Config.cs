using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Config
{
    class Config : IConfig
    {
        public string ListenPort
        {
            get 
            {
                using(RegistryKey key = Registry.LocalMachine.OpenSubKey(_root + "Server"))
                {
                    try
                    {
                        return (string)key.GetValue("ListenPort");
                    }
                    catch
                    {
                        return "8443";
                    }
                }
            }
        }

        private string _root = @"HKEY_LOCAL_MACHINE\SOFTWARE\Diladele\Active Directory Inspector\1.0";
    }
}
