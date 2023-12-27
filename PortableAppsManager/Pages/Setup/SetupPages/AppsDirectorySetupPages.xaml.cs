using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserDataTasks;
using Windows.Media.AppBroadcasting;
using Windows.Storage;
using PortableAppsManager.Services;
using PortableAppsManager.Core;
using PortableAppsManager.Classes;
using PortableAppsManager.Helpers;
using PortableAppsManager.Interop;
using System.Text.RegularExpressions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages.Setup.SetupPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsDirectorySetupPages : Page
    {
        List<AppItem> PortableAppsApps;
        List<AppItem> OtherApps;

        List<string> ExceptionsDirectories;
        public AppsDirectorySetupPages()
        {
            this.InitializeComponent();
            this.DataContext = this;

            ExceptionsDirectories = new List<string>();
        }

        private async void ScanNowBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            PickDirectoryCard.IsEnabled = false;
            ScanNowCard.Description = "Scan in progress...";
            ProgressRing r = new ProgressRing() { IsIndeterminate = true };
            r.Height = 25;
            r.Width = 25;
            (sender as Button).Content = r;

            await Task.Delay(900);

            PortableAppsApps = new List<AppItem>();
            OtherApps = new List<AppItem>();
            try
            {
                Driller d = new Driller();
                List<Driller.DrillerFoundApp> foundapps = d.GetAllAppsInsideDirectory(PathTextBox.Text,ExceptionsDirectories.ToArray());
                foreach (var found in foundapps)
                {
                    AppItem item = new AppItem();
                    item.ID = Guid.NewGuid().ToString();
                    string n = Path.GetFileNameWithoutExtension(found.ExecutablePath);
                    string res = Regex.Replace(n, @"Portable", "", RegexOptions.IgnoreCase);
                    item.AppName = res; 
                    item.ExePath = found.ExecutablePath;
                    item.ImgSource = ImageHelper.ConvertIconToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(found.ExecutablePath));
                    item.IsIncluded = true;
                    if (found.Source == Driller.DrillerFoundAppSource.PortableApps)
                    {
                        item.Setup_IsPortableAppsCom = true;
                        PortableAppsApps.Add(item);
                    }
                    else
                    {
                        item.Setup_IsPortableAppsCom = false;
                        OtherApps.Add(item);
                    }
                    ScanNowCard.Description = $"Found {item.AppName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            await Task.Delay(10);

            PortableAppsComAppsGrid.ItemsSource = PortableAppsApps;
            OtherAppsGrid.ItemsSource = OtherApps;

            AppsExpander.IsEnabled = true;
            AppsExpander.IsExpanded = true;
            await Task.Delay(100);
            PrepGrid.Visibility = Visibility.Collapsed;
            PortableAppsComAppsGrid.SelectAll();

            othertext.Visibility = Visibility.Visible;
            portableappsppstext.Visibility = Visibility.Visible;
            ContinueSetupCard.Visibility = Visibility.Visible;
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScanNowCard.IsEnabled = true;
        }

        private async void PickPathBtn_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await DialogService.OpenFolderPickerToSelectSingleFolder(Windows.Storage.Pickers.PickerViewMode.List);
            PathTextBox.Text = folder.Path;
        }

        private void ContinueSetupCard_Click(object sender, RoutedEventArgs e)
        {
            //ShellSetupPage.RootSetupFrame.Navigate()
        }

        private async void AddException_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await DialogService.OpenFolderPickerToSelectSingleFolder(Windows.Storage.Pickers.PickerViewMode.List);
            string path = folder.Path;

            ExceptionsDirectories.Add(path);

            ExceptionsFlyout.SystemBackdrop = new MicaBackdrop();
            ExceptionItems.ItemsSource = ExceptionsDirectories;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ExceptionsDirectories.Clear();
            ExceptionItems.ItemsSource = ExceptionsDirectories;
        }

        private void ExceptionItems_Loaded(object sender, RoutedEventArgs e)
        {
            ExceptionItems.ItemsSource = ExceptionsDirectories;
        }
    }
}
