using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using PortableAppsManager.Interop;
using PortableAppsManager.Managers;
using PortableAppsManager.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Classes
{
    public class ConfigJson
    {
        public ElementTheme Theme { get; set; } = ElementTheme.Default;
        public BackdropService.Backdrops Backdrops { get; set; } = BackdropService.Backdrops.Mica;

        public bool ShowDescriptionCard { get; set; } =true;
        public bool ShowPortableAppsComCard { get; set; } =true;

        public bool HasSeenGamesPageIntroduction { get; set; } = false;

        public long MaxDiskUsageBytes { get; set; } = 10737418240;

        public string PortableAppsDirectory { get; set; }

        public List<AppItem> Apps = new List<AppItem>();

        //FUNCTIONS
        public static void SaveSettings()
        {
            
            var json = JsonConvert.SerializeObject(Globals.Settings, Formatting.Indented);

            if (!File.Exists(Globals.SettingsFile))
            {
                Log.Information("App is either running for the first time or the config was deleted!");
                Globals.IsFirstTimeRun = true;
                Directory.CreateDirectory(Globals.RootDirectory);
                Log.Verbose($"Created config directory");
            }

            Log.Verbose($"Creating config file");
            using (var fileStream = new FileStream(Globals.SettingsFile, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    Log.Verbose($"Writing config JSON");
                    writer.Write(json);
                    Log.Verbose($"Config JSON written");
                }
            }
        }

        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(Globals.SettingsFile))
                {
                    Log.Verbose("Config file exists!");
                    using (var fileStream = new FileStream(Globals.SettingsFile, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            Log.Verbose("parsing config JSON");
                            string json = reader.ReadToEnd();
                            ConfigJson config = JsonConvert.DeserializeObject<ConfigJson>(json);
                            Globals.Settings = config;
                        }
                    }
                    Log.Verbose($"initializing LibraryManager");
                    Globals.library = new Managers.LibraryManager(Globals.Settings.PortableAppsDirectory);
                    Log.Verbose($"{Globals.library.ToString()} initialized");
                }
                else
                {
                    Globals.Settings = new ConfigJson();
                    SaveSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error occured while loading settings: " + ex.Message, "Error");
                Log.Error(ex.Message);
            }
        }
    }
}
