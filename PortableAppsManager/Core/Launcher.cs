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
        public AppItem App;
        
        public Launcher(AppItem a) 
        { 
            App = a;
        }
        #region Static Functions
        public static bool IsAppLaunchAvailable(string ExecutablePath)
        {
            return File.Exists(ExecutablePath);
        }
        #endregion

        public static void Launch(AppItem app, bool UseShellExecute = false)
        {
            try
            {
                Process.Start(app.ExePath);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
