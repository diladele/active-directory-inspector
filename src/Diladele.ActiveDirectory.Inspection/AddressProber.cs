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
    public static class AddressProber
    {
        public static bool Probe(this Address address)
        {
            // we can only probe a filled in IP address
            Debug.Assert(address.IP != null);

            // get all users
            List<User> users = UserProber.Probe(address.IP);

            // see if there were users found
            if (0 == users.Count)
            {
                // no, well such computer is not interesting for us
                return false;                
            }

            // set the next probe time in 15 seconds and store users
            address.NextProbeTime = DateTime.Now + new TimeSpan(0, 0, 15);
            address.Users = users;

            // and return nicely
            return true;
        }
    }
}
