using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Services
{
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

    }
}
