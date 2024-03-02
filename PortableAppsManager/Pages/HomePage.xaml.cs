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
using Serilog;
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
        List<AppItem> PinnedGames;
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
            PinnedGames = new List<AppItem>();
        }

        private void PinnedAppsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            PinnedApps = Globals.library.GetPinnedApps();

            List<AppItem> indexes = new List<AppItem>(); //indexes to remove
            //sort the pinned apps to seperate games (atleast apps with tags games)
            foreach (var item in PinnedApps)
            {
                if (item.Tags.Contains("Games"))
                {
                    indexes.Add(item);
                    PinnedGames.Add(item);
                }
            }

            foreach (var item in indexes)
            {
                PinnedApps.Remove(item);
            }

            Log.Verbose($"Pinned Apps: {PinnedApps.Count}");
            Log.Verbose($"Pinned Games: {PinnedGames.Count}");

            Bindings.Update();
        }


        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Log.Verbose("HomePage: Grid_PointerPressed on Grid!");

            Log.Verbose("PointerDeviceType: " + e.Pointer.PointerDeviceType.ToString());
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Touchpad)
            {
                var properties = e.GetCurrentPoint(sender as ContentControl).Properties;
                if (properties == null)
                {
                    Log.Verbose("Properties is null!");
                }

                if (properties.IsLeftButtonPressed)
                {
                    Log.Verbose("Left Pressed");
                    showContextMenu = false;
                }
                else if (properties.IsRightButtonPressed)
                {
                    Log.Verbose("Right Pressed");
                    showContextMenu = true;
                }
                else
                {
                    Log.Verbose("Nothing pressed?");
                }
            }

            e.Handled = false;

            Log.Verbose("showContextMenu is " + showContextMenu.ToString());
        }

        bool showContextMenu = false;
        private async void DGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Log.Verbose("HomePage: PointerReleased on Grid!");

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

                flyout.ShowAt(e.OriginalSource as DependencyObject, new FlyoutShowOptions() { Placement = FlyoutPlacementMode.TopEdgeAlignedLeft });
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

                //check if app exists and display error if not
                if (!Launcher.IsAppLaunchAvailable((ring.Tag as AppItem).ExePath))
                {
                    DialogService.ShowSimpleDialog("Cannot find program executable. Please check the path or restart the app!", "OK", "Executable Missing");
                    return;
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
            Log.Verbose("HomePage: RightTapped on Grid!");
        }

        private void Unpinitem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem o = (MenuFlyoutItem)sender;
            AppItem item = (AppItem)o.Tag;

            item.PinToHome = false;
            Globals.library.UpdateApp(item);

            PinnedApps = Globals.library.GetPinnedApps();

            Bindings.Update();
        }

        private void PinnedAppsPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PinnedAppsPanel.DeselectAll();
            SpacePanel.DeselectAll();
        }

        private async void UsagePercentageGauge_Loaded(object sender, RoutedEventArgs e)
        {
            //why am i doing it this way
            try
            {
                _totalDiskSize = StorageHelper.BytesToHumanReadable(await storageService.GetRemainingDiskQuota());

                long dirSize = await Task.Run(() => storageService.GetDirSize(new DirectoryInfo(Globals.Settings.PortableAppsDirectory)));

                double percentageUsed = (double)dirSize / storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory) * 100;

                //MessageBox.Show(Convert.ToInt32(percentageUsed).ToString(), storageService.GetDriveTotalSpaceAsync(Globals.Settings.PortableAppsDirectory).ToString());

                UsagePercentageGauge.Maximum = StorageHelper.BytesToGB(Globals.Settings.MaxDiskUsageBytes);
                UsagePercentageGauge.Value = StorageHelper.BytesToGB((Globals.Settings.MaxDiskUsageBytes - await storageService.GetRemainingDiskQuota()));

                _totalFolderSize = StorageHelper.BytesToHumanReadable(Convert.ToInt64(dirSize));

                if (await storageService.IsDirQuotaReached())
                {
                    LimitReached.Visibility = Visibility.Visible;
                }

                Bindings.Update();
            }
            catch (Exception ex) 
            {
                Log.Error(ex.Message);
                if (ex.GetType() == typeof(DirectoryNotFoundException))
                {
                    //directory not found, lets display that
                    DiskInfoGrid.Visibility = Visibility.Collapsed;
                    DiskInfoNotFound.Visibility = Visibility.Visible;
                }
            }
        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            NavigationService.NavigationService.Navigate(typeof(SettingsPage), NavigationService.NavigationService.NavigateAnimationType.Entrance); 
            NavigationService.NavigationService.Navigate(typeof(StoragePage), NavigationService.NavigationService.NavigateAnimationType.Entrance);
        }

        private async void PinnedGamesPanel_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
            PinnedGamesPanel.ItemsSource = PinnedGames;
            Log.Verbose((PinnedGamesPanel.ItemsSource as IList<AppItem>).Count.ToString());
            Bindings.Update();
        }

        private void PinnedGamesPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void PinnedAppsPanel_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void PinnedAppsPanel_Drop(object sender, DragEventArgs e)
        {
            Log.Verbose(e.Data.ToString());
        }

        private void PinnedAppsPanel_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            Log.Verbose(args.DropResult.ToString());
        }

        private void PinnedAppsPanel_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            Log.Verbose(args.Data.ToString());
        }

        private void PinnedAppsPanel_DragOver(object sender, DragEventArgs e)
        {
        }

        private void PinnedAppsPanel_DragLeave(object sender, DragEventArgs e)
        {
        }

        private void ReoderLinkClick_Click(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            NavigationService.NavigationService.Navigate(typeof(SettingsPage), NavigationService.NavigationService.NavigateAnimationType.Entrance);
        }
    }
}
