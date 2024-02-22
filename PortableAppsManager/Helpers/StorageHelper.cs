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

        public static double BytesToGB(long bytes)
        {
            const double bytesInGB = 1024 * 1024 * 1024;
            return (double)bytes / bytesInGB;
        }

        public static long GBToBytes(double gigabytes)
        {
            const long bytesInGB = 1024 * 1024 * 1024;
            return (long)(gigabytes * bytesInGB);
        }

        public static double BytesToMB(long bytes)
        {
            const double bytesInMB = 1024 * 1024;
            return (double)bytes / bytesInMB;
        }

        //just because of xaml folder view in storagepage
        public static string BytesToMBString(long bytes)
        {
            return Convert.ToInt32(BytesToMB(bytes)).ToString() + " MB";
        }
    }
}
