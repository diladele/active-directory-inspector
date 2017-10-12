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
    public static class StorageUpdater
    {
        public static void Update(this Storage storage, List<Activity> activities)
        {
            // note this update is quick as we do not do any probing
            foreach (var activity in activities)
            {
                StorageUpdater.Update(storage, activity);
            }
        }

        public static void Update(this Storage storage, Activity activity)
        {
            // local or invalid network address are not interesting
            if (activity.Network_Address == "-" || activity.Network_Address == "::1")
                return;

            Debug.Assert(false);
            throw new NotImplementedException();
        }
    }
}
