using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Diladele.ActiveDirectory.Inspection
{
    public interface IHarvester
    {
        List<Workstation> GetWorkstations();
    }

    public interface IStorage
    {
        void Insert(Address a);
    }
}
