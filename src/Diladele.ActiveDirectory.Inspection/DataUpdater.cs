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
    class DataUpdater
    {
        public void Update(List<IpAddressInfo> data, List<InfoBase> records)
        {
            // note this update is quick as we do not do any probing
            foreach (InfoBase record in records)
            {
                this.Update(data, record);
            }
        }

        private void Update(List<IpAddressInfo> data, InfoBase record)
        {
            Debug.Assert(false);
            throw new NotImplementedException();
        }
    }
}
