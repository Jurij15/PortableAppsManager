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
using PortableAppsManager.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public sealed partial class AppInfoPage : Page
    {
        AppItem item;
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

            //comment these two lines for a cool effect
            AppIconImage.Source = item.ImgSource;
            AppNameBlock.Text = item.AppName;
            AuthorBox.Text = item.Author;
            QuickAppDesc.Text = item.Description;

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
        }

        bool BackButtonPressed;
        private void AppTitleBarBackButton_Click(object sender, RoutedEventArgs e)
        {
            //hide it and navigate back
            BackButtonPressed = true;
            MainWindow.AppTitleBarBackButton.Click -= AppTitleBarBackButton_Click;
            NavigationService.NavigationService.Navigate(typeof(AppsPage), NavigationService.NavigationService.NavigateAnimationType.NoAnimation);
        }

        #region Anims
        private void Imganim_Completed(ConnectedAnimation sender, object args)
        {
            AppIconImage.Source = item.ImgSource;
        }
        private void Anim_Completed(ConnectedAnimation sender, object args)
        {
            AppInfoPane.Visibility = Visibility.Visible;
        }

        private async void Textanim_Completed(ConnectedAnimation sender, object args)
        {
            AppNameBlock.Text = item.AppName;
            //this is the last anim, show the page content
            await Task.Delay(50);
            ContentGrid.Visibility = Visibility.Visible;

        }
        #endregion

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
        public AppInfoPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchText.Visibility = Visibility.Collapsed;
            LoadingIcon.Visibility = Visibility.Visible;

            (sender as Button).IsEnabled = false;

            Launcher.Launch(item);

            await Task.Delay(500); //delay for good effect

            LaunchText.Visibility = Visibility.Visible;
            LoadingIcon.Visibility = Visibility.Collapsed;

            (sender as Button).IsEnabled = true;
        }
    }
}
