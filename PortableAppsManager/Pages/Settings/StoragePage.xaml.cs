using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Classes;
using PortableAppsManager.Helpers;
using PortableAppsManager.Interop;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx.Messaging;

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

        string _currentdiskusagequota = "Loading...";
        string _remainingquota = "Calculating...";
        string _usedquota = "Calculating...";

        List<StorageDirectorySize> storageDirectorySizes = new List<StorageDirectorySize>();

        private long dirSize = 0;
        public StoragePage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            storageService = new StorageService();

            _libraryPath = Globals.Settings.PortableAppsDirectory;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        void CheckUsageQuota()
        {
            _currentdiskusagequota = StorageHelper.BytesToHumanReadable(Globals.Settings.MaxDiskUsageBytes);
            var remaining = Globals.Settings.MaxDiskUsageBytes - dirSize;
            if (StorageHelper.BytesToHumanReadable(remaining).Contains("-"))
            {
                //max quota reached
                _remainingquota = StorageHelper.BytesToHumanReadable(0);
                QuotaReachedInfoBar.IsOpen = true;
            }
            else
            {
                _remainingquota = StorageHelper.BytesToHumanReadable(remaining);
                QuotaReachedInfoBar.IsOpen = false;
            }

            _usedquota = StorageHelper.BytesToHumanReadable((Globals.Settings.MaxDiskUsageBytes - remaining));

            Bindings.Update();
        }

        private async void UsagePercentageGauge_Loaded(object sender, RoutedEventArgs e)
        {
            //this is a bit of a mess

            //ModifyDiskUsageQuota.IsEnabled = false;

            DirectoryInfo d = new DirectoryInfo(Globals.Settings.PortableAppsDirectory);

            _totalDiskSize = StorageHelper.BytesToHumanReadable(storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory));

            dirSize = await Task.Run(() => storageService.GetDirSize(d));

            double percentageUsed = (double)dirSize / storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory) * 100;

            UsagePercentageGauge.Maximum = 100;
            UsagePercentageGauge.Value = Convert.ToInt32(percentageUsed);

            CheckUsageQuota();

            //MessageBox.Show(Convert.ToInt32(percentageUsed).ToString(), storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory).ToString());

            _totalFolderSize = StorageHelper.BytesToHumanReadable(Convert.ToInt64(dirSize));
            _freedrivespace = StorageHelper.BytesToHumanReadable(Convert.ToInt64(storageService.GetTotalFreeDriveSpace(Path.GetPathRoot(Globals.Settings.PortableAppsDirectory))));

            Bindings.Update();

            storageDirectorySizes = await (storageService.GetTopLevelDirectories(d));

            Bindings.Update();

            //seperate this one, as it takes a VERY long time
            _useddiskspace = StorageHelper.BytesToHumanReadable(Convert.ToInt64(await Task.Run(() => storageService.CalculateUsedDiskSpaceAsync(Path.GetPathRoot(Globals.Settings.PortableAppsDirectory)))));

            Bindings.Update();

            ModifyDiskUsageQuota.IsEnabled = true;
        }

        private void WhatsThisBtn_Click(object sender, RoutedEventArgs e)
        {
            WhatsThisTip.Target = WhatsThisBtn;
            WhatsThisTip.ShouldConstrainToRootBounds = false;
            WhatsThisTip.IsOpen = true;
        }

        bool _isQuotaLoaded = false;
        bool _isSliderLoaded = false;
        private void ModifyQuotaBox_Loaded(object sender, RoutedEventArgs e)
        {
            ModifyQuotaBox.Maximum = StorageHelper.BytesToGB(storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory));
            ModifyQuotaBox.Value = StorageHelper.BytesToGB(Globals.Settings.MaxDiskUsageBytes);

            _isQuotaLoaded = true;
        }

        private void ModifyQuotaSlider_Loaded(object sender, RoutedEventArgs e)
        {
            ModifyQuotaSlider.Maximum = StorageHelper.BytesToGB(Convert.ToInt64(storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory)));
            ModifyQuotaSlider.Value = StorageHelper.BytesToGB(Globals.Settings.MaxDiskUsageBytes);

            ModifyDiskUsageQuotaFlyoutPanel.UpdateLayout();

            _isSliderLoaded = true;
        }

        private void ModifyQuotaBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (!_isQuotaLoaded)
            {
                return;
            }
            ModifyQuotaSlider.Value = Convert.ToInt64(args.NewValue);

            Globals.Settings.MaxDiskUsageBytes = StorageHelper.GBToBytes(Convert.ToInt64(args.NewValue));
            ConfigJson.SaveSettings();

            CheckUsageQuota();
        }

        private void ModifyQuotaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_isSliderLoaded)
            {
                return;
            }
            ModifyQuotaBox.Value = Convert.ToInt64(e.NewValue);

            Globals.Settings.MaxDiskUsageBytes = StorageHelper.GBToBytes(Convert.ToInt64(e.NewValue));
            ConfigJson.SaveSettings();

            CheckUsageQuota();
        }
    }
}
