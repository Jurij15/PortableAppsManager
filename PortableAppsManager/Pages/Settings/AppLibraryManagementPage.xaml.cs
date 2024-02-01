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
using PortableAppsManager.Dialogs;
using CommunityToolkit.WinUI;
using System.Diagnostics;
using System.Collections;
using Microsoft.UI.Xaml.Media.Animation;
using WinRT;
using IniParser;
using IniParser.Model;
using Microsoft.UI.Xaml.Media.Imaging;
using ABI.Windows.AI.MachineLearning;
using PortableAppsManager.Managers;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppLibraryManagementPage : Page
    {
        List<AppItem> PortableAppsApps { get; set; }
        List<AppItem> OtherApps { get; set; }

        HashSet<string> AllTags;

        LibraryManager manager;

        private void UpdateItemsSources()
        {
            PortableAppsComAppsGrid.ItemsSource = null;
            PortableAppsComAppsGrid.ItemsSource = PortableAppsApps;

            OtherAppsGrid.ItemsSource = null;
            OtherAppsGrid.ItemsSource = OtherApps;
        }

        private List<string> GetAllExceptionsDirectories()
        {
            List<string> exceptions = new List<string>();
            foreach (var item in ExceptionItems.Items)
            {
                exceptions.Add(item.ToString());
            }

            return exceptions;
        }

        public AppLibraryManagementPage()
        {
            this.InitializeComponent();
            this.DataContext = this; 

            manager = new LibraryManager(Globals.Settings.PortableAppsDirectory);
        }

        private async void AddException_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await DialogService.OpenFolderPickerToSelectSingleFolder(Windows.Storage.Pickers.PickerViewMode.List);
            string path = folder.Path;
            //MessageBox.Show(path);

            ExceptionItems.Items.Add(path);
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ExceptionItems.Items.Clear();
        }

        private async void ScanNowBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            ScanExpander.IsEnabled = false;
            ScanExpander.Description = "Scan in progress...";
            ProgressRing r = new ProgressRing() { IsIndeterminate = true };
            r.Height = 20;
            r.Width = 20;
            (sender as Button).Content = r;

            await Task.Delay(500);

            //PortableAppsApps = new List<AppItem>();
            //OtherApps = new List<AppItem>();
            try
            {
                Driller d = new Driller();
                List<Driller.DrillerFoundApp> foundapps = manager.IndexLibrary(Globals.Settings.PortableAppsDirectory, GetAllExceptionsDirectories(), Convert.ToBoolean(IgnoreExistingAppsCheck.IsChecked));
                foreach (var found in foundapps)
                {
                    AppItem item = new AppItem();
                    item = d.DrillerFoundAppToAppItem(found);
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
                    ScanExpander.Description = $"Found {item.AppName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            await Task.Delay(10);

            UpdateItemsSources();

            AppsExpander.IsEnabled = true;
            AppsExpander.IsExpanded = true;
            await Task.Delay(100);
            ScanExpander.Visibility = Visibility.Collapsed;
            PortableAppsComAppsGrid.SelectAll();

            othertext.Visibility = Visibility.Visible;
            portableappsppstext.Visibility = Visibility.Visible;
            SaveChangesBtn.Visibility = Visibility.Visible;
        }

        private void SaveChangesBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (AppItem item in PortableAppsComAppsGrid.SelectedItems)
            {
                Globals.Settings.Apps.Add(item);
            }
            foreach (AppItem item in OtherAppsGrid.SelectedItems)
            {
                Globals.Settings.Apps.Add(item);
            }

            ConfigJson.SaveSettings();

            NavigationService.NavigationService.Navigate(typeof(AppsPage), NavigationService.NavigationService.NavigateAnimationType.Entrance);
        }

        private AppItem GetAppItemFromID(string ID)
        {
            AppItem ReturnAppItem = null;
            bool IsInPortableApps = false;
            foreach (var item in PortableAppsApps)
            {
                if (item.ID == ID)
                {
                    IsInPortableApps = true;
                    ReturnAppItem = item;
                    break;
                }
            }

            if (!IsInPortableApps)
            {
                foreach (var item in OtherApps)
                {
                    if (item.ID == ID)
                    {
                        ReturnAppItem = item;
                        break;
                    }
                }
            }

            return ReturnAppItem;
        }

        private void UpdateModifiedApp(AppItem App)
        {
            bool IsInPortableApps = false;
            foreach (var item in PortableAppsApps)
            {
                if (item.ID == App.ID)
                {
                    IsInPortableApps = true;

                    PortableAppsApps[PortableAppsApps.IndexOf(item)] = App;
                    break;
                }
            }

            if (!IsInPortableApps)
            {
                foreach (var item in OtherApps)
                {
                    if (item.ID == App.ID)
                    {
                        IsInPortableApps = true;

                        OtherApps[OtherApps.IndexOf(item)] = App;
                        break;
                    }
                }
            }

            UpdateItemsSources();
        }

        private async void EditAppInfo_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem parent = sender as MenuFlyoutItem;
            ContentDialog editdialog = DialogService.CreateBlankContentDialog(false);
            EditAppDialogContent content = new EditAppDialogContent(GetAppItemFromID(parent.Tag.ToString()), editdialog);

            editdialog.Content = content;
            editdialog.UpdateLayout();

            await editdialog.ShowAsync();
            editdialog.UpdateLayout();

            AppItem modified = content.ModifiedAppItem;
            UpdateModifiedApp(modified);
        }

        private void SelectAllInPortableApps_Click(object sender, RoutedEventArgs e)
        {
            PortableAppsComAppsGrid.SelectAll();
        }

        private void SelectAllInOtherApps_Click(object sender, RoutedEventArgs e)
        {
            OtherAppsGrid.SelectAll();
        }

        private void DeSelectAllInPortableApps_Click(object sender, RoutedEventArgs e)
        {
            PortableAppsComAppsGrid.DeselectAll();
        }

        private void DeSelectAllInOtherApps_Click(object sender, RoutedEventArgs e)
        {
            OtherAppsGrid.DeselectAll();
        }

        private void TagSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = new List<string>() { $"Click to add {sender.Text}" };
            }

        }

        private ListView TargetListView;
        private void TagsList_Loaded(object sender, RoutedEventArgs e)
        {
            TargetListView = sender as ListView;
            ((ListView)sender).ItemsSource = AllTags;
        }

        private void IDK_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            string content = (((ListView)sender).ItemsSource as IList)[((ListView)sender).SelectedIndex].ToString();

            if (GetAppItemFromID((((ListView)sender).Tag as string)).Tags == null)
            {
                GetAppItemFromID((((ListView)sender).Tag as string)).Tags = new List<string>();
            }
            GetAppItemFromID((sender as ListView).Tag.ToString()).Tags.Add(content);
            //MessageBox.Show(GetAppItemFromID((sender as ListView).Tag.ToString()).Tags.Count.ToString());
            ((((sender as ListView).Parent as StackPanel).Parent as FlyoutPresenter).Parent as Popup).IsOpen = false;
        }

        private void TagSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            AllTags.Add(sender.Text);
            //MessageBox.Show(sender.Text);
            sender.Text = string.Empty;
            sender.ItemsSource = null;

            TargetListView.ItemsSource = null;
            TargetListView.ItemsSource = AllTags;

            sender.Text = string.Empty;
            sender.Text = string.Empty;
        }
    }
}
