using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Classes;
using PortableAppsManager.Core;
using PortableAppsManager.Helpers;
using PortableAppsManager.Interop;
using PortableAppsManager.Pages.Settings;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx.Messaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        List<AppItem> PinnedApps;
        Launcher launcher;
        StorageService storageService;

        string _totalDiskSize = "Calculating...";
        string _totalFolderSize = "Calculating...";
        public HomePage()
        {
            this.InitializeComponent();

            this.DataContext = this;

            PinnedApps = new List<AppItem>();
            launcher = new Launcher();
            storageService = new StorageService();
        }

        private void PinnedAppsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            PinnedApps = Globals.library.GetPinnedApps();
            PinnedAppsPanel.ItemsSource = PinnedApps;
        }

        bool showContextMenu = false;
        private async void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (showContextMenu)
            {

                AppItem app = (sender as Grid).Tag as AppItem;

                //spawn a menuflyout at the button
                MenuFlyout flyout = new MenuFlyout();

                MenuFlyoutItem unpinitem = new MenuFlyoutItem();
                unpinitem.Tag = app;
                unpinitem.Text = "Unpin";

                FontIcon icon = new FontIcon();
                icon.Glyph = "\uE734";

                unpinitem.Icon = icon;
                unpinitem.Click += Unpinitem_Click;

                flyout.Items.Add(unpinitem);

                flyout.ShowAt(sender as DependencyObject, new FlyoutShowOptions() { });
                showContextMenu = false;
            }
            else
            {
                Image appimage = null;
                ProgressRing ring = null;

                //find the controls
                foreach (var item in (sender as Grid).Children)
                {
                    if (item.GetType() == typeof(StackPanel))
                    {
                        foreach (var elements in (item as StackPanel).Children)
                        {
                            if (elements.GetType() == typeof(Image))
                            {
                                appimage = elements as Image;
                            }
                            else if (elements.GetType() == typeof(ProgressRing))
                            {
                                ring = elements as ProgressRing;
                            }
                        }
                    }
                }

                appimage.Visibility = Visibility.Collapsed;
                ring.Visibility = Visibility.Visible;

                try
                {
                    launcher.Launch(ring.Tag as AppItem);
                }
                catch (Exception ex)
                {
                    DialogService.ShowSimpleDialog(ex.Message, "Close", "An error occured while launching");
                }
                await Task.Delay(1500);

                appimage.Visibility = Visibility.Visible;
                ring.Visibility = Visibility.Collapsed;
            }
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            showContextMenu = true;
        }

        private void Unpinitem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem o = (MenuFlyoutItem)sender;
            AppItem item = (AppItem)o.Tag;

        }

        private void PinnedAppsPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PinnedAppsPanel.DeselectAll();
            SpacePanel.DeselectAll();
        }

        private async void UsagePercentageGauge_Loaded(object sender, RoutedEventArgs e)
        {
            _totalDiskSize = StorageHelper.BytesToHumanReadable(storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory));

            long dirSize = await Task.Run(() => storageService.GetDirSize(new DirectoryInfo( Globals.Settings.PortableAppsDirectory)));

            double percentageUsed = (double)dirSize / storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory) * 100;

            //MessageBox.Show(Convert.ToInt32(percentageUsed).ToString(), storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory).ToString());

            UsagePercentageGauge.Maximum = 100;
            UsagePercentageGauge.Value = Convert.ToInt32(percentageUsed);

            _totalFolderSize = StorageHelper.BytesToHumanReadable(Convert.ToInt64(dirSize));

            Bindings.Update();
        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            NavigationService.NavigationService.Navigate(typeof(SettingsPage), NavigationService.NavigationService.NavigateAnimationType.Entrance); 
            NavigationService.NavigationService.Navigate(typeof(StoragePage), NavigationService.NavigationService.NavigateAnimationType.SlideFromLeft);
        }
    }
}
