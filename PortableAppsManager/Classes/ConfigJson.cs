using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using PortableAppsManager.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Classes
{
    public class ConfigJson
    {
        public ElementTheme Theme { get; set; }

        public string PortableAppsDirectory { get; set; }

        public List<AppItem> Apps = new List<AppItem>();

        //FUNCTIONS
        public static void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(Globals.Settings, Formatting.Indented);

            if (!File.Exists(Globals.SettingsFile))
            {
                Globals.IsFirstTimeRun = true;
                Directory.CreateDirectory(Globals.RootDirectory);
            }

            using (var fileStream = new FileStream(Globals.SettingsFile, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.Write(json);
                }
            }
        }

        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(Globals.SettingsFile))
                {
                    using (var fileStream = new FileStream(Globals.SettingsFile, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            string json = reader.ReadToEnd();
                            ConfigJson config = JsonConvert.DeserializeObject<ConfigJson>(json);
                            Globals.Settings = config;
                        }
                    }
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
            }
        }
    }
}
