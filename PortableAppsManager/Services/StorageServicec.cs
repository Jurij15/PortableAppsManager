using PortableAppsManager.Helpers;
using PortableAppsManager.Interop;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Portable;

namespace PortableAppsManager.Services
{
    //cannot be in storageservice class because of xaml
    public class StorageDirectorySize()
    {
        public string Path { get; set; } = "";
        public long SizeInBytes { get; set; } = 0;
        public long TotalParentDirectorySizeInBytes {  get; set; } = 0;
    }

    public class StorageService
    {
        public StorageService() 
        { 
        }

        public long GetDirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += GetDirSize(di);
            }
            return size;
        }

        public async Task<List<StorageDirectorySize>> GetTopLevelDirectories(DirectoryInfo d)
        {
            List<StorageDirectorySize> list = new List<StorageDirectorySize>();

            long TotalParentDirSize = await Task.Run(() => GetDirSize(d));

            DirectoryInfo[] dirs = await Task.Run(() => d.GetDirectories("*", SearchOption.TopDirectoryOnly));
            foreach (DirectoryInfo item in dirs)
            {
                StorageDirectorySize c = new StorageDirectorySize();
                c.Path = item.FullName;
                c.TotalParentDirectorySizeInBytes = TotalParentDirSize;
                c.SizeInBytes = await Task.Run(() => GetDirSize(item));

                Log.Verbose($"DirectorySize for {item.FullName}: {c.SizeInBytes}, in GB: {StorageHelper.BytesToGB(c.SizeInBytes)}");

                list.Add(c);
            }

            return list;
        }

        public long GetDriveTotalSpaceAsync(string folderPath)
        {
            // Get drive info
            DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(folderPath));

            // Check if drive info is available
            if (driveInfo == null)
            {
                throw new Exception("Drive information not available.");
            }

            // Get total available space
            return driveInfo.TotalSize;
        }

        public long GetTotalFreeDriveSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.AvailableFreeSpace;
                }
            }
            return -1;
        }

        public async Task<long> CalculateUsedDiskSpaceAsync(string path)
        {
            try
            {
                // Get the drive information of the specified path
                DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(path));

                // Check if the drive is ready
                if (driveInfo.IsReady)
                {
                    // If the path is a directory, calculate the total size of all files within it
                    if (Directory.Exists(Path.GetPathRoot(path)))
                    {
                        long totalSize = 0;
                        var f = await Task.Run(() => Directory.GetFiles(Path.GetPathRoot(path), "*", SearchOption.AllDirectories));
                        string[] files = f;
                        foreach (string file in files)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                        return totalSize;
                    }
                    // If the path is a file, return its size
                    else if (File.Exists(path))
                    {
                        return new FileInfo(path).Length;
                    }
                    // If the path doesn't exist, return -1 to indicate an error
                    else
                    {
                        return -1;
                    }
                }
                // If the drive is not ready, return -1 to indicate an error
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred: " + ex.Message);
                return -1;
            }
        }

        public async Task<bool> IsDirQuotaReached()
        {
            bool isQuotaReached = false;

            long dirSize = await Task.Run(() => GetDirSize(new DirectoryInfo(Globals.Settings.PortableAppsDirectory)));
            var remaining = Globals.Settings.MaxDiskUsageBytes - dirSize;
            if (StorageHelper.BytesToHumanReadable(remaining).Contains("-"))
            {
                //max quota reached
                isQuotaReached = true;
            }
            else
            {
                isQuotaReached = false;
            }

            return isQuotaReached;
        }

        public async Task<long> GetRemainingDiskQuota()
        {
            return Globals.Settings.MaxDiskUsageBytes - await Task.Run(() => GetDirSize(new DirectoryInfo(Globals.Settings.PortableAppsDirectory)));
        }
    }
}
