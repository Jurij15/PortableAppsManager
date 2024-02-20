using Microsoft.UI.Xaml.Documents;
using PortableAppsManager.Classes;
using PortableAppsManager.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input.ForceFeedback;

namespace PortableAppsManager.Managers
{
    public class LibraryManager
    {
        private Driller _driller;

        public LibraryManager(string LibraryPath)
        {
            _driller = new Driller();
        }

        public List<AppItem> GetAllApps()
        {
            return Globals.Settings.Apps;
        }

        public List<Driller.DrillerFoundApp> IndexLibrary(string DirectoryPath, List<string> Exceptions, bool IgnoreExisting = false)
        {
            List<Driller.DrillerFoundApp> drills = new List<Driller.DrillerFoundApp> ();

            if (IgnoreExisting)
            {
                foreach (var item in Globals.Settings.Apps)
                {
                    Exceptions.Add(Path.GetDirectoryName(item.ExePath));
                }   
            }

            var temp = _driller.GetAllAppsInsideDirectory(DirectoryPath, Exceptions);

            drills = temp.ToList();

            return drills;
        }

        public void AddAppToLibrary(AppItem app)
        {
            Globals.Settings.Apps.Add(app);
            ConfigJson.SaveSettings();
        }

        public void RemoveAppFromLibrary(AppItem item)
        {
            foreach (var app in Globals.Settings.Apps)
            {
                if (app.ID == item.ID)
                {
                    Globals.Settings.Apps.Remove(app);
                    break;
                }
            }

            ConfigJson.SaveSettings();
        }

        public void UpdateApp(AppItem ModifiedAppItem)
        {
            foreach (var app in Globals.Settings.Apps)
            {
                if (app.ID == ModifiedAppItem.ID)
                {
                    int index = Globals.Settings.Apps.IndexOf(app);
                    RemoveAppFromLibrary(ModifiedAppItem);

                    Globals.Settings.Apps.Insert(index, ModifiedAppItem);
                    break;
                }
            }

            ConfigJson.SaveSettings();
        }

        public List<AppItem> GetPinnedApps()
        {
            List<AppItem> apps = new List<AppItem>();

            foreach (var item in Globals.Settings.Apps)
            {
                if (item.PinToHome)
                {
                    apps.Add(item);
                }
            }

            return apps;
        }
    }
}
