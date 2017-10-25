using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Config
{
    public class ConfigFactory
    {
        public static IConfig Instance()
        {
            return new Config();
        }
    }
}
