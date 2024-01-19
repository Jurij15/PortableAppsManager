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

        public void Launch(bool UseShellExecute = false)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo info = new ProcessStartInfo();

                info.UseShellExecute = UseShellExecute;
                info.FileName = App.ExePath;

                process.StartInfo = info;

                process.Start();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
