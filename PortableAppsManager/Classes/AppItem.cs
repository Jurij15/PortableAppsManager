﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using PortableAppsManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
