using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Classes;
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
            item = e.Parameter as AppItem;

            //comment this for a cool effect
            AppIconImage.Source = item.ImgSource;
            AppNameBlock.Text = item.AppName;

            base.OnNavigatedTo(e);

            NavigationService.NavigationService.ChangeBreadcrumbVisibility(false);
            //just a bug in the service, this just fixes it temporarely
            NavigationService.NavigationService.MainNavigation.AlwaysShowHeader = false;
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

        private void Imganim_Completed(ConnectedAnimation sender, object args)
        {
            AppIconImage.Source = item.ImgSource;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            NavigationService.NavigationService.ChangeBreadcrumbVisibility(true);
            //just a bug in the service, this just fixes it temporarely
            NavigationService.NavigationService.MainNavigation.AlwaysShowHeader = true;

            base.OnNavigatingFrom(e);
        }
        public AppInfoPage()
        {
            this.InitializeComponent();
        }
    }
}