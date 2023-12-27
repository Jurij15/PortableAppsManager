using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Helpers
{
    public class BooleanHelper
    {
        public static bool CheckAllTrue(params bool[] values)
        {
            return Array.TrueForAll(values, x => x);
        }
    }
}
