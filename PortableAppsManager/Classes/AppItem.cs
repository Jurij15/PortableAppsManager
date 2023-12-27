using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Classes
{
    public class AppItem
    {
        public string ID {  get; set; }

        public string ExePath { get; set; }
        public bool IsIncluded { get; set; }

        public string AppName { get; set; } = "";
        public string Author { get; set; } = "";
        public string Description { get; set; } = "";
        public ImageSource ImgSource { get; set; }

        public List<string> Tags { get; set; }

        public bool Setup_IsPortableAppsCom { get; set; } = false;
    }
}
