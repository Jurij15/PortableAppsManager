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
using PortableAppsManager.Structs;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppLibraryManagementPage : Page
    {
        private AppLibraryManagementPageNavigationParams NavigationParams;
        List<AppItem> Apps { get; set; }
        AppItem _selectedItem { get; set; }
        HashSet<string> AllTags;

        TagsService tagsService;

        private void UpdateItemsSources(int newindex = 0, bool ScrollBack = true)
        {
            object currentobj = AppList.ContainerFromIndex(newindex);

            AppList.ItemsSource = null;
            AppList.ItemsSource = Apps;
            AppList.SelectedIndex = newindex;
            //MessageBox.Show(Apps.Count.ToString());

            if (ScrollBack)
            {
                AppList.ScrollIntoView(currentobj);
            }
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

        #region Navigation Funcs
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not null)
            {
                NavigationParams = (AppLibraryManagementPageNavigationParams)e.Parameter;
            }
            else
            {
                NavigationParams = new AppLibraryManagementPageNavigationParams() { NavigateToWhenSaved = typeof(AppsPage)};
            }
            base.OnNavigatedTo(e);
        }
        #endregion

        public AppLibraryManagementPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            Apps = new List<AppItem>();
            tagsService = new TagsService();
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

        private void LoadApps(List<Driller.DrillerFoundApp> foundapps)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (var found in foundapps)
                {
                    AppItem item = new AppItem();
                    item = item.DrillerFoundAppToAppItem(found);
                    if (found.Source == Driller.DrillerFoundAppSource.PortableApps)
                    {
                        item.Setup_IsPortableAppsCom = true;
                    }
                    else
                    {
                        item.Setup_IsPortableAppsCom = false;
                    }

                    Apps.Add(item);
                }
            });
        }

        private async void ScanNowBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            ScanExpander.IsEnabled = false;
            ScanExpander.Description = "Scan in progress...";
            ProgressRing r = new ProgressRing() { IsIndeterminate = true };
            r.Height = 20;
            r.Width = 20;
            //(sender as Button).Content = r;

            await Task.Delay(500);

            try
            {

                List<Driller.DrillerFoundApp> foundapps = await Globals.library.IndexLibrary(Globals.Settings.PortableAppsDirectory, GetAllExceptionsDirectories(), Convert.ToBoolean(IgnoreExistingAppsCheck.IsChecked), ScannerStarting, ScannerDirectoryChanged);

                ScannerValuesCard.Description = "Preparing...";
                ScanneerDirectoriesProgress.IsIndeterminate =true;
                ProgressText.Visibility = Visibility.Collapsed;

                await Task.Run(() => LoadApps(foundapps));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            await Task.Delay(2000);//waiting a moment seems to help reduce scrolling lag

            UpdateItemsSources();

            await Task.Delay(10);
            AppsGrid.Visibility = Visibility.Visible;
            ScanExpander.Visibility = Visibility.Collapsed;
            ProgressText.Visibility = Visibility.Visible;

            if (Apps.Count > 0)
            {
                AppList.SelectedIndex = 0;
            }
        }

        private async void ScannerStarting(int e)
        {
            ScanOptions.Visibility = Visibility.Collapsed;
            ScannerValuesCard.Visibility = Visibility.Visible;
            ScanneerDirectoriesProgress.Maximum = e;
            ScanneerDirectoriesProgress.Width = 130;

            TotalDirectoryNumberBlock.Text = e.ToString();

            await Task.Delay(100);
        }

        private async void ScannerDirectoryChanged(Scanner.ScannerDirectoryChangedEventArgs e)
        {
            ScannerValuesCard.Description = e.CurrentDirectory;
            ScanneerDirectoriesProgress.Value = e.ScannedDirsTotal;

            CurrentDirectoryNumberBlock.Text = e.ScannedDirsTotal.ToString();

            await Task.Delay(50);
        }

        private void UpdateModifiedApp(AppItem App)
        {
            int currentindex = 0;
            foreach (var item in Apps)
            {
                if (item.ID == App.ID)
                {
                    int index = Apps.IndexOf(item);
                    currentindex = index;
                    Apps.RemoveAt(index);
                    Apps.Insert(index, item);
                    break;
                }
            }

            UpdateItemsSources(currentindex);
        }

        private async void EditAppInfo_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton parent = sender as AppBarButton;
            ContentDialog editdialog = DialogService.CreateBlankContentDialog(false);
            EditAppDialogContent content = new EditAppDialogContent(_selectedItem, editdialog);

            editdialog.Content = content;
            editdialog.UpdateLayout();

            await editdialog.ShowAsync();
            editdialog.UpdateLayout();

            AppItem modified = content.ModifiedAppItem;
            UpdateModifiedApp(modified);
        }

        private void AppList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedItem = (sender as ListView).SelectedItem as AppItem;

            if (_selectedItem is not null)
            {
                //update the app display
                AppImage.Source = ImageHelper.GetImageSource(_selectedItem);
                AppNameBlock.Text = _selectedItem.AppName;

                AppExePathBlock.Text = _selectedItem.ExePath;
                AppAuthorBox.Text = _selectedItem.Author;
                AppDescriptionBox.Text = _selectedItem.Description;
                AppLanguageBox.Text = _selectedItem.Language;
                AppLaunchAsAdminBox.Text = _selectedItem.LaunchAsAdmin.ToString();
            }
        }

        private void AppItemCheck_Click(object sender, RoutedEventArgs e)
        {
            bool selected = Convert.ToBoolean((sender as CheckBox).IsChecked);

            if (_selectedItem == null)
            {
                return;
            }

            _selectedItem.TEMP_ISIncludedInSetup = selected;
        }

        //tags things

        private void DeletTagBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedItem.Tags.Remove((sender as Button).Tag.ToString());
            TagsGrid.ItemsSource = null;
            TagsGrid.ItemsSource = _selectedItem.Tags;
        }

        private void ConfirmAddTagButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTagNameBox.Text) && !string.IsNullOrWhiteSpace(NewTagNameBox.Text))
            {
                _selectedItem.Tags.Add(NewTagNameBox.Text);
                TagsGrid.ItemsSource = null;
                TagsGrid.ItemsSource = _selectedItem.Tags;
            }

            AddTagFlyout.Hide();
        }

        private void TagsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (_selectedItem is null)
            {
                return;
            }
            TagsGrid.ItemsSource = _selectedItem.Tags;
        }

        private void AllTagsList_Loaded(object sender, RoutedEventArgs e)
        {
            AllTagsList.ItemsSource = tagsService.GetAllTags();
        }

        private void AllTagsList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            string tag = AllTagsList.SelectedItem.ToString();
            AllTagsList.ItemsSource = null;
            AddTagFlyout.Hide();

            _selectedItem.Tags.Add(tag);
            TagsGrid.ItemsSource = new List<string>();
            TagsGrid.ItemsSource = _selectedItem.Tags;

            ConfigJson.SaveSettings();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Apps)
            {
                if (item.TEMP_ISIncludedInSetup)
                {
                   Globals.library.AddAppToLibrary(item);
                }
            }

            if (NavigationParams.NavigationFrame is not null & NavigationParams.NavigationTransitionNfo is not null)
            {
                NavigationParams.NavigationFrame.Navigate(NavigationParams.NavigateToWhenSaved, null, NavigationParams.NavigationTransitionNfo);
            }
            else
            {
                NavigationService.NavigationService.Navigate(NavigationParams.NavigateToWhenSaved, NavigationService.NavigationService.NavigateAnimationType.Entrance);
            }
        }

        private async void AddCustomAppBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog adddialog = DialogService.CreateBlankContentDialog(false);
            AddAppDialog content = new AddAppDialog(adddialog);

            adddialog.Content = content;
            adddialog.UpdateLayout();

            await adddialog.ShowAsync();
            adddialog.UpdateLayout();

            if (!content.HasCanceledAddingApp)
            {
                content.ModifiedAppItem.TEMP_ISIncludedInSetup = true;
                Apps.Add(content.ModifiedAppItem);
                Globals.library.AddAppToLibrary(content.ModifiedAppItem);
            }

            UpdateItemsSources(AppList.SelectedIndex);
        }

        private void UnselectAllBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Apps)
            {
                item.TEMP_ISIncludedInSetup = false;
            }

            UpdateItemsSources(AppList.SelectedIndex);
        }

        private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Apps)
            {
                item.TEMP_ISIncludedInSetup = true;
            }

            UpdateItemsSources(AppList.SelectedIndex);
        }

        private void IgnoreExistingAppsCheck_Loaded(object sender, RoutedEventArgs e)
        {
            IgnoreExistingAppsCheck.IsChecked = NavigationParams.IgnoreAlreadyExisting;
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Since selecting an item will also change the text,
            // only listen to changes caused by user entering text.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                foreach (var app in Apps)
                {
                    if (Path.GetFileNameWithoutExtension(app.ExePath).ToLower().Contains(sender.Text.ToLower()))
                    {
                        suitableItems.Add(Path.GetFileNameWithoutExtension(app.ExePath));
                    }
                }
                if (suitableItems.Count == 0)
                {
                    suitableItems.Add("No results found");
                }
                sender.ItemsSource = suitableItems;
            }
        }

        private async void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem.ToString() == "No results found")
            {
                return;
            }

            await Task.Delay(50); //delay the load, so that the suggestion is written to the sender

            foreach (var app in Apps)
            {
                if (Path.GetFileNameWithoutExtension(app.ExePath) == sender.Text)
                {
                    AppList.ScrollIntoView(app, ScrollIntoViewAlignment.Leading);
                    AppList.SelectedIndex = Apps.IndexOf(app); 
                    break;
                }
            }
        }
    }
}
