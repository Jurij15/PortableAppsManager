using IniParser.Model;
using IniParser;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using PortableAppsManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using static PortableAppsManager.Core.Driller;

namespace PortableAppsManager.Classes
{
    public class AppItem
    {
        public string ID { get; set; }

        public string ExePath { get; set; } //

        public string AppName { get; set; } = ""; //
        public string Author { get; set; } = ""; //
        public string Description { get; set; } = ""; //
        public string Language { get; set; }

        public AppImageSourceType SourceType { get; set; } = AppImageSourceType.Executable;
        public string AppImageSourcePath { get; set; } = null;
        //public ImageSource ImgSource { get; set; } //removed because cannot be converted to json

        public List<string> Tags { get; set; } = new List<string>();

        public string LaunchArguments { get; set; } = ""; 
        public bool LaunchAsAdmin { get; set; } = false;
        public bool UseShellExecute { get; set; } = false;

        public bool TEMP_ISIncludedInSetup {  get; set; } = false;
        //PortableApps.com Properties

        /// <summary>
        /// PortableApps IsOpenSource variable
        /// </summary>
        public bool PortableApps_IsOpenSource { get; set; } = false;

        /// <summary>
        /// PortableApps IsFreeware variable
        /// </summary>
        public bool PortableApps_IsFreeware { get; set; } = false;

        public bool PortableApps_IsShareable { get; set; } = false;

        public bool PortableApps_IsCommercialUse { get; set; } = false;

        public string PortableApps_Homepage { get; set; } = "";

        //portableapps apps

        /// <summary>
        /// DO NOT CHANGE THIS, IT WILL BREAK SOME THINGS
        /// </summary>
        public bool Setup_IsPortableAppsCom { get; set; } = false;

        /// <summary>
        /// PortableApps Package Version
        /// </summary> 
        public string PortableApps_PackageVersion { get; set; } = "";

        /// <summary>
        /// PortableApps Display Version (idk what is the difference)
        /// </summary>
        public string PortableApps_DisplayVersion { get; set; } = "";

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
                //default include portableapps apps
                item.TEMP_ISIncludedInSetup = true;
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
    }
}
