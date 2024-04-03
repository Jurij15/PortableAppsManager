using Microsoft.UI.Xaml;
using PortableAppsManager.Classes;
using PortableAppsManager.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager
{
    public class Globals
    {
        private protected readonly static string VersionDefinition = "VERSION - TYPE (DD/MM/YYYY)";
        public static readonly string Version = "1.0.0 - BETA (03042024)"; //VERSION - TYPE (DD/MM/YYYY)

        public static bool IsFirstTimeRun;

        public static Window m_window;
        public static ConfigJson Settings;
        public static LibraryManager library;

        public static string RootDirectory = "Settings";
        public static string SettingsFile = Path.Combine(RootDirectory, "config.json");
    }
}
