using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ABI.Microsoft.UI.Xaml;
using IniParser.Model;
using IniParser;
using Microsoft.UI.Xaml.Media.Imaging;
using PortableAppsManager.Classes;
using PortableAppsManager.Helpers;
using PortableAppsManager.Interop;
using WinUIEx.Messaging;
using Windows.Graphics.Printing.OptionDetails;

namespace PortableAppsManager.Core
{
    public class Driller
    {
        public AppItem DrillerFoundAppToAppItem(DrillerFoundApp found)
        {
            AppItem item = new AppItem();
            item.ID = Guid.NewGuid().ToString();
            string n = Path.GetFileNameWithoutExtension(found.ExecutablePath);
            string res = Regex.Replace(n, @"Portable", "", RegexOptions.IgnoreCase);
            item.AppName = res;
            item.ExePath = found.ExecutablePath;
            item.Tags = new List<string>();

            //reset the portableapps variables
            item.PortableApps_IsShareable = false;
            item.PortableApps_IsFreeware = false;
            item.PortableApps_IsOpenSource = false;
            item.PortableApps_DisplayVersion = "";
            item.PortableApps_IsCommercialUse = false;
            item.PortableApps_Homepage = "https://portableapps.com/";
            item.PortableApps_PackageVersion = "";
            //item.ImgSource = ImageHelper.ConvertIconToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(found.ExecutablePath));
            if (File.Exists(Path.Combine(found.ExecutableParentDirectoryPath, "App", "AppInfo", "appinfo.ini")))
            {
                //this is a PortableAppsApp, we can get as much info as possible
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(Path.Combine(found.ExecutableParentDirectoryPath, "App", "AppInfo", "appinfo.ini"));

                item.AppName = Regex.Replace(data["Details"]["Name"], @"Portable", "", RegexOptions.IgnoreCase);
                item.Author = data["Details"]["Publisher"];
                item.Description = data["Details"]["Description"];
                item.PortableApps_Homepage = data["Details"]["Homepage"];
                if (!item.PortableApps_Homepage.Contains("http")) //some apps do not have the correct url
                {
                    item.PortableApps_Homepage = "https://" + item.PortableApps_Homepage;
                }

                item.Tags.Add(data["Details"]["Category"]);

                item.PortableApps_IsOpenSource = Convert.ToBoolean(data["License"]["OpenSource"]);
                item.PortableApps_IsFreeware = Convert.ToBoolean(data["License"]["Freeware"]);

                item.PortableApps_IsShareable = Convert.ToBoolean(data["License"]["Freeware"]);
                item.PortableApps_IsCommercialUse = Convert.ToBoolean(data["License"]["Freeware"]);

                item.Language = data["Details"]["Language"];

                item.PortableApps_PackageVersion = data["Version"]["PackageVersion"];
                item.PortableApps_DisplayVersion = data["Version"]["DisplayVersion"];

                //lets try to get a higer quality image
                if (File.Exists(Path.Combine(found.ExecutableParentDirectoryPath, "App", "AppInfo", "appicon_128.png")))
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.UriSource = new System.Uri(Path.Combine(found.ExecutableParentDirectoryPath, "App", "AppInfo", "appicon_128.png"));
                    item.AppImageSourcePath = Path.Combine(found.ExecutableParentDirectoryPath, "App", "AppInfo", "appicon_128.png");
                    item.SourceType = Enums.AppImageSourceType.File;
                }
                else
                {
                    item.SourceType = Enums.AppImageSourceType.Executable;
                }
            }
            if (item.Tags.Count == 0)
            {
                if (found.IsInGameDir)
                {
                    item.Tags.Add("Games");
                }
            }

            return item;
        }

        public enum DrillerFoundAppSource
        {
            Unknown,
            PortableApps
        }
        public class DrillerFoundApp
        {
            public DrillerFoundAppSource Source;
            public string ExecutableParentDirectoryPath { get; set; }
            public string ExecutablePath { get; set; }
            public bool IsInGameDir { get; set; } = false;
        }

        private bool IsDirOnExceptionList(string Path, List<string> ExceptionsList)
        {
            bool returnVal = false;
            foreach (var item in ExceptionsList)
            {
                if (Path == item)
                {
                    returnVal = true;
                    break;
                }
            }
            return returnVal;
        }

        public List<DrillerFoundApp> GetAllAppsInsideDirectory(string DirectoryPath, List<string> Exceptions)
        {
            List<DrillerFoundApp> apps = new List<DrillerFoundApp>();

            List<string> NonPortableAppsDirs = new List<string>();

            ///Structure of portableapps directory
            ///AppName+Portable
            ///-App (dir)
            ///-Data (dir)
            ///-Other (dir)
            ///-help.html (html file)
            ///-execuable file (exe)

            //lets scan the provided directory for any subdirectories that have the above structure
            string[] dirs = Directory.GetDirectories(DirectoryPath);
            foreach (var parentDirectory in dirs)
            {
                if (IsDirOnExceptionList(parentDirectory, Exceptions))
                {
                    continue;
                }
                bool foundAppDir = false;
                bool foundDataDir = false;
                bool foundOtherDir = false;
                foreach (string dir in Directory.GetDirectories(parentDirectory))
                {
                    if (Path.GetFileName(dir) == "App")
                    {
                        foundAppDir = true;
                    }
                    if (Path.GetFileName(dir) == "Data")
                    {
                        foundDataDir= true;
                    }
                    if (Path.GetFileName(dir) == "Other")
                    {
                        foundOtherDir = true;
                    }
                }

                if (!BooleanHelper.CheckAllTrue(foundAppDir, foundDataDir, foundOtherDir))
                {
                    //it is probably not a portableapps app, we will add this to the correct list
                    //and check in a different way

                    NonPortableAppsDirs.Add(parentDirectory);

                    continue;
                }

                //we are sure that this is a portableapps app, lets get the exe path and save it
                foreach (var file in Directory.GetFiles(parentDirectory))
                {
                    //MessageBox.Show(Path.GetExtension(Path.GetFileName(file)), "portableapps loop");
                    if (Path.GetExtension(Path.GetFileName(file)) == ".exe")
                    {
                        //MessageBox.Show(Path.GetExtension(Path.GetFileName(file)));
                        // it is probably the portable exe
                        DrillerFoundApp app = new DrillerFoundApp();
                        app.ExecutableParentDirectoryPath = parentDirectory;
                        app.ExecutablePath  = file;
                        app.Source = DrillerFoundAppSource.PortableApps;

                        apps.Add(app);
                        break;
                    }
                }
            }


            //now, lets find all the remaining directories that possbly are not portableapps
            foreach (var item in NonPortableAppsDirs)
            {
                if (IsDirOnExceptionList(item, Exceptions))
                {
                    continue;
                }
                foreach (var exe in Directory.GetFiles(item))
                {
                    //MessageBox.Show(Path.GetExtension(Path.GetFileName(exe)), "other loop");
                    if (Path.GetExtension(Path.GetFileName(exe)) == ".exe" && !Path.GetFileName(exe).Contains(".paf.exe")) //ignore paf files as theyre installers
                    {
                        DrillerFoundApp app = new DrillerFoundApp();
                        app.ExecutableParentDirectoryPath = item;
                        app.ExecutablePath = exe;
                        app.Source = DrillerFoundAppSource.Unknown;

                        apps.Add(app);
                    }
                }
            }

            //clear the array to free up memory
            NonPortableAppsDirs.Clear();

            return apps;
        }
    }
}
