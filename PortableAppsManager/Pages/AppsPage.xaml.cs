using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Core;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using PortableAppsManager.Interop;
using PortableAppsManager.Helpers;
using PortableAppsManager.Classes;
using Windows.Devices.Display.Core;
using PortableAppsManager.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Diagnostics;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsPage : Page
    {
        bool AppsItemsLoaded = false;
        public class DisplayApp
        {
            public DisplayApp(string name, ImageSource imageSource, AppItem appItem)
            {
                Name = name;
                ImgSrc = imageSource;
                OAppItem = appItem;
            }
            public string Name { get; set; }
            public ImageSource ImgSrc { get; set; }

            public AppItem OAppItem { get; set; }
        }

        List<DisplayApp> Apps;
        AppItemControl CachedItem;
        public AppsPage()
        {
            this.InitializeComponent();

            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //MessageBox.Show(e.SourcePageType.ToString(), "NavigatedTo Fired!");
            base.OnNavigatedTo(e);
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void AppItems_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppsItemsLoaded)
            {
                AppItems.Items.Clear();
                foreach (var item in Globals.Settings.Apps)
                {
                    if (item.Tags != null)
                    {
                        item.Tags.Add("Add Tags!");
                    }
                    else
                    {
                        item.Tags = new List<string> { "Add Tags!" };
                    }
                    AppItemControl control = new AppItemControl();
                    control.AppItem = item;
                    control.AppName = item.AppName;
                    control.IMAGEControl.Source = item.ImgSource;
                    control.Width = 315;
                    if (Launcher.IsAppLaunchAvailable(item.ExePath))
                    {
                        control.LabelText = "Open";
                    }
                    else
                    {
                        control.LabelText = "Info";
                    }
                    control.APPLABEL.Tag = control;
                    control.CardLabelBtn_Clicked += Control_CardLabelBtn_Clicked;

                    control.PointerReleased += OnPointerReleased;

                    AppItems.Items.Add(control);
                }
                AppsItemsLoaded = true;
            }
            else
            {
                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackConnectedAnimation");
                if (anim != null)
                {
                    anim.TryStart(CachedItem);
                }

                var imganim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackImageAnim");
                if (imganim != null)
                {
                    imganim.TryStart(CachedItem.IMAGEControl);
                }

                var textanim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackTextAnim");
                if (textanim != null)
                {
                    textanim.TryStart(CachedItem.APPNAMEBlock);
                }
            }
        }

        private async void Control_CardLabelBtn_Clicked(object sender, RoutedEventArgs e)
        {
            AppItemControl control = (sender as Button).Tag as AppItemControl;
            if (Launcher.IsAppLaunchAvailable(control.AppItem.ExePath))
            {
                control.PlayLaunchAnimationOnLabel();
                await Task.Delay(1000);
                control.StopLaunchAnimationOnLabel(true, 900);
            }
            else
            {
                //OnPointerReleased((sender as Button).Tag, null);
            }
        }

        private async void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            AppItemControl source = sender as AppItemControl;

            var imageanim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardImageAnim", source.IMAGEControl);
            var animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", source);
            var textanim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardTextAnim", source.APPNAMEBlock);

            source.APPLABEL.Visibility = Visibility.Collapsed;
            source.MetadataControl.Visibility = Visibility.Collapsed;
            await Task.Delay(100);

            NavigationService.NavigationService.MainFrame.Navigate(typeof(AppInfoPage), source.AppItem, new SuppressNavigationTransitionInfo());

            source.APPLABEL.Visibility = Visibility.Visible;
            source.MetadataControl.Visibility = Visibility.Visible;

            CachedItem = source;
        }
    }
}
