using PortableAppsManager.Classes;
using PortableAppsManager.Services;
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
        public Launcher() { }

        #region Static Functions
        public static bool IsAppLaunchAvailable(string ExecutablePath)
        {
            return File.Exists(ExecutablePath);
        }
        #endregion

        public void Launch(AppItem a, bool UseShellExecute = false) 
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo info = new ProcessStartInfo();

                info.UseShellExecute = a.UseShellExecute;
                info.FileName = a.ExePath;

                process.StartInfo = info;

                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("elevation"))
                    {
                        DialogService.ShowSimpleDialog("This app requires elevated(administrator) privileges to run. Edit the application and toggle Launch as Admin", "OK", "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Launch(bool UseShellExecute = false)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo info = new ProcessStartInfo();

                info.UseShellExecute = App.UseShellExecute;
                info.FileName = App.ExePath;

                process.StartInfo = info;

                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("elevation"))
                    {
                        DialogService.ShowSimpleDialog("This app requires elevated(administrator) privileges to run. Edit the application and toggle Launch as Admin", "OK", "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
