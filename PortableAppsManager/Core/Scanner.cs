using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Animation;
using PortableAppsManager.Helpers;
using PortableAppsManager.Interop;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.EnterpriseData;
using static PortableAppsManager.Core.Driller;

namespace PortableAppsManager.Core
{
    public class Scanner
    {
        private string DirectoryPath;
        private List<string> Exceptions;
        private DispatcherQueue dispatcherQueue;

        private List<Driller.DrillerFoundApp> FoundAppList;

        public Scanner(string DirPath, List<string> exceptions)
        {
            DirectoryPath = DirPath;
            Exceptions = exceptions;

            dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            Log.Verbose($"Scanner Loaded, DirectoryPath: {DirectoryPath}, Exceptions count: {exceptions.Count.ToString()}");
        }

        #region EventArgs
        public class ScannerDirectoryChangedEventArgs : EventArgs
        {
            public ScannerDirectoryChangedEventArgs(int totalscanneddirs, string CurrentDir)
            {
                ScannedDirsTotal = totalscanneddirs;
                CurrentDirectory = CurrentDir;
            }

            public int ScannedDirsTotal { get; private set; } //amount of already scanned directories
            public string CurrentDirectory { get; private set; } //current scanning directory
        }
        #endregion

        #region Events
        public event EventHandler<ScannerDirectoryChangedEventArgs> ScannerDirectoryChanged;
        public event EventHandler<int> ScannerStarting;
        #endregion

        private int ScannedDirectoriesCount = 0;
        public async Task<List<Driller.DrillerFoundApp>> ScanDirectory()
        {    
            FoundAppList = new List<Driller.DrillerFoundApp> ();

            int TotalDirctories = Directory.GetDirectories(DirectoryPath, "*", SearchOption.AllDirectories).Count();

            dispatcherQueue.TryEnqueue(() =>
            {
                ScannerStarting.Invoke("", TotalDirctories);
            });

            Log.Verbose("Scanning directories");
            foreach (var item in Directory.GetDirectories(DirectoryPath))
            {
                Log.Verbose($"directory found: {item}");
                if (IsDirOnExceptionList(item))
                {
                    Log.Verbose($"directory {item} is on ExecptionList, skipping");
                    continue;
                }
                else
                {
                    await Task.Delay(1);
                    await DirectoryFound(item);
                }
            }

            Log.Verbose($"found {FoundAppList.Count} apps in {TotalDirctories} directories");

            Log.Verbose("organizing array");
            List<Driller.DrillerFoundApp> organizedlist = new List<DrillerFoundApp>();
            List<Driller.DrillerFoundApp> originals = FoundAppList;
            //check he array for portableappps
            foreach (var item in originals.ToList())
            {
                if (item.Source == DrillerFoundAppSource.PortableApps)
                {
                    organizedlist.Add(item);
                }
            }

            //remove portableapps in originals
            foreach (var item in originals.ToList())
            {
                if (item.Source == DrillerFoundAppSource.PortableApps)
                {
                    originals.Remove(item);
                }
            }

            //all leftover apps just go to the new array
            foreach (var item in originals)
            {
                organizedlist.Add(item);
            }

            FoundAppList = null;
            FoundAppList = organizedlist;

            //clear to save memory
            originals.Clear();
            ScannedDirectoriesCount = 0;
            
            return FoundAppList;
        }

        private bool IsDirOnExceptionList(string Path)
        {

            bool returnVal = false;
            foreach (var item in Exceptions)
            {
                if (Path == item)
                {
                    returnVal = true;
                    break;
                }
            }
            return returnVal;
        }

        //maybe make it multithreaded/ async in some way, it could hang the ui thread
        private async Task DirectoryFound(string DirectoryPath)
        {
            Log.Verbose("DirectoryFound: " + DirectoryPath);

            //raise the event that a directory has changed
            dispatcherQueue.TryEnqueue(() =>
            {
                ScannerDirectoryChanged.Invoke("", new ScannerDirectoryChangedEventArgs(ScannedDirectoriesCount, DirectoryPath)); ;
            });

            //first, lets get all files in directory
            //if it is NOT portableapps, scan it for every single exe and add it to a list
            if (!IsPortableAppsDir(DirectoryPath))
            {
                foreach (var exe in Directory.GetFiles(DirectoryPath))
                {
                    if (Path.GetExtension(Path.GetFileName(exe)) == ".exe" && !Path.GetFileName(exe).Contains(".paf.exe")) //ignore paf files as theyre installers
                    {
                        DrillerFoundApp app = new DrillerFoundApp();
                        app.ExecutableParentDirectoryPath = DirectoryPath;
                        app.ExecutablePath = exe;
                        app.Source = DrillerFoundAppSource.Unknown;

                        FoundAppList.Add(app);
                    }
                }
            }
            //if it is portableapps, great, lets get the exe and add it to a list
            else
            {
                foreach (var exe in Directory.GetFiles(DirectoryPath))
                {
                    if (Path.GetExtension(Path.GetFileName(exe)) == ".exe")
                    {
                        //MessageBox.Show(Path.GetExtension(Path.GetFileName(file)));
                        // it is probably the portable exe
                        DrillerFoundApp app = new DrillerFoundApp();
                        app.ExecutableParentDirectoryPath = DirectoryPath;
                        app.ExecutablePath = exe;
                        app.Source = DrillerFoundAppSource.PortableApps;

                        FoundAppList.Add(app);
                        break;
                    }
                }
            }

            ScannedDirectoriesCount++;
            //now, just get directories and call the function recursively
            foreach (var item in Directory.GetDirectories(DirectoryPath))
            {
                //skip the directory if it is on exceptions
                if (IsDirOnExceptionList(item))
                {
                    continue;
                }
                else
                {
                    await Task.Run(() => DirectoryFound(item));
                }
            }
        }

        private bool IsPortableAppsDir(string DirectoryPath)
        {
            bool ReturnVal = false;

            bool foundAppDir = false;
            bool foundDataDir = false;
            bool foundOtherDir = false;
            foreach (string dir in Directory.GetDirectories(DirectoryPath))
            {
                if (Path.GetFileName(dir) == "App")
                {
                    foundAppDir = true;
                }
                if (Path.GetFileName(dir) == "Data")
                {
                    foundDataDir = true;
                }
                if (Path.GetFileName(dir) == "Other")
                {
                    foundOtherDir = true;
                }
            }

            ReturnVal = BooleanHelper.CheckAllTrue(foundAppDir, foundDataDir, foundOtherDir);

            return ReturnVal;
        }
    }
}
