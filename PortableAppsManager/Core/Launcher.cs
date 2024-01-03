using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Core
{
    public class Launcher
    {
        public static bool IsAppLaunchAvailable(string ExecutablePath)
        {
            return File.Exists(ExecutablePath);
        }
    }
}
