using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Classes;
using PortableAppsManager.Interop;
using PortableAppsManager.Managers;
using PortableAppsManager.Pages.Settings;
using PortableAppsManager.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public enum Theme
        {
            //
            // Summary:
            //     Use the Application.RequestedTheme value for the element. This is the default.
            Default,
            //
            // Summary:
            //     Use the **Light** default theme.
            Light,
            //
            // Summary:
            //     Use the **Dark** default theme.
            Dark
        }

        List<AppItem> HomePinsReorder = new List<AppItem>();
        public SettingsPage()
        {
            this.InitializeComponent();

            this.DataContext = this;

            VersionBlock.Text = Globals.Version;
        }

        private void ThemeCombo_Loaded(object sender, RoutedEventArgs e)
        {
            var _themeenumval = Enum.GetValues(typeof(Theme)).Cast<Theme>();
            ThemeCombo.ItemsSource = _themeenumval;

            ThemeCombo.SelectedIndex = (int)Globals.Settings.Theme;
        }

        private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.Settings.Theme = (ElementTheme)ThemeCombo.SelectedIndex;
            ConfigJson.SaveSettings();
            ThemeService.ChangeTheme((ElementTheme)ThemeCombo.SelectedIndex);
        }

        private void LibRescanCard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigationService.Navigate(typeof(AppLibraryManagementPage), NavigationService.NavigationService.NavigateAnimationType.SlideFromLeft);
        }

        private void BackdropCombo_Loaded(object sender, RoutedEventArgs e)
        {
            var _themeenumval = Enum.GetValues(typeof(BackdropService.Backdrops)).Cast<BackdropService.Backdrops>();
            BackdropCombo.ItemsSource = _themeenumval;

            BackdropCombo.SelectedIndex = (int)Globals.Settings.Backdrops;
        }

        private void BackdropCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.Settings.Backdrops = (BackdropService.Backdrops)BackdropCombo.SelectedIndex;
            BackdropService.ChangeBackdrop((BackdropService.Backdrops)BackdropCombo.SelectedIndex);

            ConfigJson.SaveSettings();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ShowDescriptionCard_Checked(object sender, RoutedEventArgs e)
        {
            Globals.Settings.ShowDescriptionCard = Convert.ToBoolean((sender as CheckBox).IsChecked);
            ConfigJson.SaveSettings();
        }

        private void ShowDescriptionCard_Unchecked(object sender, RoutedEventArgs e)
        {
            Globals.Settings.ShowDescriptionCard = Convert.ToBoolean((sender as CheckBox).IsChecked);
            ConfigJson.SaveSettings();
        }

        private void ShowPortableAppsCard_Checked(object sender, RoutedEventArgs e)
        {
            Globals.Settings.ShowPortableAppsComCard = Convert.ToBoolean((sender as CheckBox).IsChecked);
            ConfigJson.SaveSettings();
        }

        private void ShowPortableAppsCard_Unchecked(object sender, RoutedEventArgs e)
        {
            Globals.Settings.ShowPortableAppsComCard = Convert.ToBoolean((sender as CheckBox).IsChecked);
            ConfigJson.SaveSettings();
        }

        private void ShowDescriptionCard_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).IsChecked = Globals.Settings.ShowDescriptionCard;
        }

        private void ShowPortableAppsCard_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).IsChecked = Globals.Settings.ShowPortableAppsComCard;
        }

        private void AboutManager_Expanded(object sender, EventArgs e)
        {
        }

        private void LibraryStorageCard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigationService.Navigate(typeof(StoragePage), NavigationService.NavigationService.NavigateAnimationType.SlideFromLeft);
        }

        private void AboutManager_Loaded(object sender, RoutedEventArgs e)
        {
            AboutManager.IsExpanded = true;
        }

        private void ReoderAppsFlyoutGridView_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Verbose("ReoderAppsFlyoutGridView_Loaded");
            HomePinsReorder = Globals.library.GetPinnedApps();
            ReoderAppsFlyoutGridView.ItemsSource = HomePinsReorder;
            Log.Verbose("Set ReoderAppsFlyoutGridView.ItemSource");
        }

        private void ReoderAppsFlyoutGridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            List<AppItem> newlist = new List<AppItem>();

            newlist = (List<AppItem>)ReoderAppsFlyoutGridView.ItemsSource;

            Globals.library.ClearSetPinnedApps(newlist.ToList());

            HomePinsReorder = newlist;

            ReoderAppsFlyoutGridView.ItemsSource = null;
            ReoderAppsFlyoutGridView.ItemsSource = HomePinsReorder.ToList();

            newlist.Clear(); //i think its a good idea to clear this
        }

        private void ReoderAppsFlyoutGridView_Unloaded(object sender, RoutedEventArgs e)
        {
            Log.Verbose("ReoderAppsFlyoutGridView_Unloaded");
        }
    }
}
