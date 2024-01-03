using Microsoft.UI.Xaml;
using PortableAppsManager.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager
{
    public class Globals
    {
        public static bool IsFirstTimeRun;

        public static Window m_window;
        public static ConfigJson Settings;

        public static string RootDirectory = "Settings";
        public static string SettingsFile = Path.Combine(RootDirectory, "config.json");
    }
}
