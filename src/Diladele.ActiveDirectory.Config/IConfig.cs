using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diladele.ActiveDirectory.Config
{
    public interface IConfig
    {
        string ListenPort { get; }
    }
}
