using PortableAppsManager.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static bool Launch(AppItem app)
        {
            bool returnval = false;

            try
            {
                Process.Start(app.ExePath);
                returnval = true;
            }
            catch (Exception ex)
            {
            }

            return returnval;
        }
    }
}
