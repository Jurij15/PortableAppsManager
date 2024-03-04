using Microsoft.UI.Xaml.Documents;
using PortableAppsManager.Classes;
using PortableAppsManager.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserDataTasks;
using Windows.Gaming.Input.ForceFeedback;
using Windows.System;

namespace PortableAppsManager.Managers
{
    public class LibraryManager
    {
        private Driller _driller;

        public LibraryManager(string LibraryPath)
        {
            _driller = new Driller();

            if (Globals.Settings.PinnedApps is null)
            {
                Globals.Settings.PinnedApps = new List<AppItem>(); //initialize the array
            }
        }

        public bool IsLibraryAvailable()
        {
            if (!Directory.Exists(Globals.Settings.PortableAppsDirectory))
            {
                return false;
            }

            return true;
        }

        public List<AppItem> GetAllApps()
        {
            return Globals.Settings.Apps;
        }

        public List<Driller.DrillerFoundApp> TopLevelIndexLibrary(string DirectoryPath, List<string> Exceptions, bool IgnoreExisting = false)
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

        public Scanner _scanner;
        public async Task<List<Driller.DrillerFoundApp>> IndexLibrary(string DirectoryPath, List<string> Exceptions, bool IgnoreExisting, Action<int> ScannerStartingEvent, Action<Scanner.ScannerDirectoryChangedEventArgs> ScannerDirChangedEvent)
        {
            List<Driller.DrillerFoundApp> drills = new List<Driller.DrillerFoundApp>();

            if (IgnoreExisting)
            {
                foreach (var item in Globals.Settings.Apps)
                {
                    Exceptions.Add(Path.GetDirectoryName(item.ExePath));
                }
            }

            _scanner = new Scanner(DirectoryPath, Exceptions);

            _scanner.ScannerStarting += (s, e) =>
            {
                ScannerStartingEvent(e);
            };
            _scanner.ScannerDirectoryChanged += (s, e) =>
            {
                ScannerDirChangedEvent(e);
            };

            List<Driller.DrillerFoundApp> temp = await _scanner.ScanDirectory();

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
            return Globals.Settings.PinnedApps;
        }

        public void PinApp(AppItem app)
        {
            Globals.Settings.PinnedApps.Add(app);

            ConfigJson.SaveSettings();

            //also set it in the actuall app class, so that the ui will update
            app.PinToHome = true;
            UpdateApp(app);
        }

        public async void UnpinApp(AppItem app)
        {
            Log.Verbose(Globals.Settings.PinnedApps.Remove(app).ToString());

            ConfigJson.SaveSettings();

            //also set it in the actuall app class, so that the ui will update
            app.PinToHome = false;
            UpdateApp(app);
            await Task.Delay(50); //await for update to finish?
        }

        /// <summary>
        /// Clears the current array of pinned apps and sets the new one
        /// </summary>
        /// <param name="apps">New list of pinned apps</param>
        public void ClearSetPinnedApps(List<AppItem> apps)
        {
            Globals.Settings.PinnedApps = null; //clear the array
            Globals.Settings.PinnedApps = apps;
        }
    }
}
