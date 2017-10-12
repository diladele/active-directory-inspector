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
            // invalid network address is not interesting
            if (activity.Network_Address == "-")
                return;

            Debug.Assert(false);
            throw new NotImplementedException();
        }
    }
}
