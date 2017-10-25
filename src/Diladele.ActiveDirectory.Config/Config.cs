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
            get { 

                // todo - read data from registry (the registry should be populated by the INSTALLER!!
                throw new NotImplementedException(); 
            }
        }
    }
}
