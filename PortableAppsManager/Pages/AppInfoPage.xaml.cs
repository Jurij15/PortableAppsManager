using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Classes;
using PortableAppsManager.Core;
using PortableAppsManager.Dialogs;
using PortableAppsManager.Enums;
using PortableAppsManager.Helpers;
using PortableAppsManager.Interop;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using WinUIEx.Messaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppInfoPage : Page
    {
        AppItem item;
        private AppItemModificationType AppWasModified { get; set; } = AppItemModificationType.None;

        Launcher Launcher;
        TagsService tagsService;
        #region Anims and Navigation
        #region Navigation

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AppInfoPane.Visibility = Visibility.Visible;
            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardConnectedAnimation");
            anim.Completed += Anim_Completed;
            if (anim != null)
            {
                anim.TryStart(AppInfoPane);
            }

            var imganim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardImageAnim");
            imganim.Completed += Imganim_Completed;
            if (imganim != null)
            {
                imganim.TryStart(AppIconImage);
            }

            var textanim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardTextAnim");
            textanim.Completed += Textanim_Completed;
            if (textanim != null)
            {
                textanim.TryStart(AppNameBlock);
            }

            var buttonanim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardButtonAnim");
            if (buttonanim != null)
            {
                buttonanim.TryStart(LaunchButton);
            }

            item = e.Parameter as AppItem;

            //comment these lines for a cool effect
            //AppIconImage.Source = ImageHelper.GetImageSource(item);
            //AppNameBlock.Text = item.AppName;
            //AuthorBox.Text = item.Author;
            //AppDesc.Text = item.Description;

            base.OnNavigatedTo(e);

            NavigationService.NavigationService.ChangeBreadcrumbVisibility(false);
            //just a bug in the service, this just fixes it temporarely
            NavigationService.NavigationService.MainNavigation.AlwaysShowHeader = false;

            //show the title bar back button and hook the back event
            MainWindow.AppTitleBarBackButton.Visibility = Visibility.Visible;
            MainWindow.AppTitleBarBackButton.Click += AppTitleBarBackButton_Click;

            if (string.IsNullOrEmpty(item.Author) || string.IsNullOrWhiteSpace(item.Author))
            {
                AuthorBox.Visibility = Visibility.Collapsed;
            }

            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
                NavigationService.NavigationService.Navigate(typeof(AppsPage));
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            NavigationService.NavigationService.ChangeBreadcrumbVisibility(true);
            //just a bug in the service, this just fixes it temporarely
            NavigationService.NavigationService.MainNavigation.AlwaysShowHeader = true;
            MainWindow.AppTitleBarBackButton.Visibility = Visibility.Collapsed;

            if (BackButtonPressed)
            {
                //ConnectedAnimationService.GetForCurrentView().DefaultDuration = new TimeSpan(0, 0, 0, 4);
                var anim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackConnectedAnimation", AppInfoPane);
                anim.Configuration = new DirectConnectedAnimationConfiguration();
                var image = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackImageAnim", AppInfoPane).Configuration = new DirectConnectedAnimationConfiguration();
                var teext = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackTextAnim", AppInfoPane).Configuration = new DirectConnectedAnimationConfiguration();
                var button = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackButtonAnim", AppInfoPane).Configuration = new DirectConnectedAnimationConfiguration();
                BackButtonPressed = false;
            }

            base.OnNavigatingFrom(e);
        }

        bool BackButtonPressed;
        private void AppTitleBarBackButton_Click(object sender, RoutedEventArgs e)
        {
            //hide it and navigate back
            BackButtonPressed = true;
            MainWindow.AppTitleBarBackButton.Click -= AppTitleBarBackButton_Click;
            NavigationService.NavigationService.Navigate(typeof(AppsPage), NavigationService.NavigationService.NavigateAnimationType.NoAnimation, AppWasModified);
        }
        #endregion
        #region Anims
        private void Imganim_Completed(ConnectedAnimation sender, object args)
        {
            //AppIconImage.Source = ImageHelper.GetImageSource(item);
        }
        private void Anim_Completed(ConnectedAnimation sender, object args)
        {
            //AppInfoPane.Visibility = Visibility.Visible;
        }

        private async void Textanim_Completed(ConnectedAnimation sender, object args)
        {
            //AppNameBlock.Text = item.AppName;
            //this is the last anim, show the page content
            await Task.Delay(50);
            ContentGrid.Visibility = Visibility.Visible;
            if (!item.Setup_IsPortableAppsCom)
            {
                PortableAppsPane.Visibility = Visibility.Collapsed;
            }

        }
        #endregion
        #endregion

        public AppInfoPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            tagsService = new TagsService();
        }

        void Initialize()
        {
            if (item != null)
            {
                Launcher = new Launcher(item);
            }
            else
            {
                throw new ArgumentNullException(nameof(item), "AppItem was null");
            }
        }

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchText.Visibility = Visibility.Collapsed;
            LoadingIcon.Visibility = Visibility.Visible;

            (sender as Button).IsEnabled = false;

            try
            {
                Launcher.Launch();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("elevation"))
                {
                    DialogService.ShowSimpleDialog("This app requires elevated (administrator) privileges to run. Edit the application and toggle Launch as Admin", "OK", "Error");
                }
            }

            await Task.Delay(1500); //delay to make sure the porcess started

            LaunchText.Visibility = Visibility.Visible;
            LoadingIcon.Visibility = Visibility.Collapsed;

            LaunchSuccessfulInfoBar.IsOpen = true;

            (sender as Button).IsEnabled = true;
        }

        private void DeletTagBtn_Click(object sender, RoutedEventArgs e)
        {
            item.Tags.Remove((sender as Button).Tag.ToString());
            TagsGrid.ItemsSource = null;
            TagsGrid.ItemsSource = item.Tags;
        }

        private void ConfirmAddTagButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTagNameBox.Text) && !string.IsNullOrWhiteSpace(NewTagNameBox.Text))
            {
                item.Tags.Add(NewTagNameBox.Text);
                TagsGrid.ItemsSource = null;
                TagsGrid.ItemsSource = item.Tags;
            }

            AddTagFlyout.Hide();
        }

        private void TagsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            TagsGrid.ItemsSource = item.Tags;
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

            item.Tags.Add(tag);
            TagsGrid.ItemsSource = new List<string>();
            TagsGrid.ItemsSource = item.Tags;

            ConfigJson.SaveSettings();
        }

        private async void EditAppBtn_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem parent = sender as MenuFlyoutItem;
            ContentDialog editdialog = DialogService.CreateBlankContentDialog(false);
            EditAppDialogContent content = new EditAppDialogContent(item, editdialog);

            editdialog.Content = content;
            editdialog.UpdateLayout();

            await editdialog.ShowAsync();
            editdialog.UpdateLayout();

            AppItem modified = content.ModifiedAppItem;

            item = null;
            item = modified;

            foreach (var iitem in Globals.Settings.Apps)
            {
                if (iitem.ID == item.ID)
                {
                    Globals.Settings.Apps[Globals.Settings.Apps.IndexOf(iitem)] = modified;
                    break;
                }
            }

            AppWasModified = AppItemModificationType.Modified;

            RefreshAppToUpdate.IsOpen = true;
        }

        private void RemoveAppBtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            foreach (var app in Globals.Settings.Apps)
            {
                if (app.ID == item.ID)
                {
                    Globals.Settings.Apps.Remove(app);
                    break;
                }
            }

            ConfigJson.SaveSettings();

            AppWasModified = AppItemModificationType.Deleted;

            NavigationService.NavigationService.Navigate(typeof(AppsPage), NavigationService.NavigationService.NavigateAnimationType.SlideFromLeft);
        }

        private void LaunchButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Launcher.IsAppLaunchAvailable(item.ExePath))
            {
                AppExeMissingInfoBar.IsOpen = true;
                (sender as Button).IsEnabled = false;
            }
            else
            {
                AppExeMissingInfoBar.IsOpen = false;
            }
        }
    }
}
