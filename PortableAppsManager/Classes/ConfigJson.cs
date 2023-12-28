using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Classes
{
    public class ConfigJson
    {
        public ElementTheme Theme { get; set; }

        public string PortableAppsDirectory { get; set; }

        public List<AppItem> Apps {  get; set; }
        public List<string> Tags { get; set; }
    }
}
