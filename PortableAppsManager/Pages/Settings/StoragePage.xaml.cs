using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Helpers;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StoragePage : Page
    {
        StorageService storageService;

        string _totalDiskSize = "Calculating...";
        string _totalFolderSize = "Calculating...";
        string _freedrivespace = "Calculating...";
        string _useddiskspace = "Calculating...";

        string _libraryPath = "Loading...";
        public StoragePage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            storageService = new StorageService();

            _libraryPath = Globals.Settings.PortableAppsDirectory;
        }

        private async void UsagePercentageGauge_Loaded(object sender, RoutedEventArgs e)
        {
            _totalDiskSize = StorageHelper.BytesToHumanReadable(storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory));

            long dirSize = await Task.Run(() => storageService.GetDirSize(new DirectoryInfo(Globals.Settings.PortableAppsDirectory)));

            double percentageUsed = (double)dirSize / storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory) * 100;

            //MessageBox.Show(Convert.ToInt32(percentageUsed).ToString(), storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory).ToString());

            UsagePercentageGauge.Maximum = 100;
            UsagePercentageGauge.Value = Convert.ToInt32(percentageUsed);

            _totalFolderSize = StorageHelper.BytesToHumanReadable(Convert.ToInt64(dirSize));
            _freedrivespace = StorageHelper.BytesToHumanReadable(Convert.ToInt64(storageService.GetTotalFreeDriveSpace(Path.GetPathRoot(Globals.Settings.PortableAppsDirectory))));

            Bindings.Update();

            //seperate this one, as it takes a VERY long time
            _useddiskspace = StorageHelper.BytesToHumanReadable(Convert.ToInt64(await Task.Run(() => storageService.CalculateUsedDiskSpaceAsync(Path.GetPathRoot(Globals.Settings.PortableAppsDirectory)))));

            Bindings.Update();
        }
    }
}
