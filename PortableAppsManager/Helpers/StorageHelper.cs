using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Helpers
{
    public class StorageHelper
    {
        public static string BytesToHumanReadable(long bytes)
        {
            if (bytes >= 1024L * 1024L * 1024L) // More than 1 GB
            {
                double gb = bytes / (1024.0 * 1024.0 * 1024.0);
                return $"{gb:F2} GB";
            }
            else if (bytes >= 1024L * 1024L) // More than 1 MB
            {
                double mb = bytes / (1024.0 * 1024.0);
                return $"{mb:F2} MB";
            }
            else if (bytes >= 1024L) // More than 1 KB
            {
                double kb = bytes / 1024.0;
                return $"{kb:F2} KB";
            }
            else
            {
                return $"{bytes} bytes";
            }
        }
    }
}
