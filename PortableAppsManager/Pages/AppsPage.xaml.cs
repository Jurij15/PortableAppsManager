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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsPage : Page
    {
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
        public AppsPage()
        {
            this.InitializeComponent();

            this.DataContext = this;
        }

        private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void AppItems_Loaded(object sender, RoutedEventArgs e)
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
        }

        private void Control_CardLabelBtn_Clicked(object sender, RoutedEventArgs e)
        {
            AppItemControl control = (sender as Button).Tag as AppItemControl;
            if (Launcher.IsAppLaunchAvailable(control.AppItem.ExePath))
            {
                control.PlayLaunchAnimationOnLabel();
            }
            else
            {
                //OnPointerReleased((sender as Button).Tag, null);
            }
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            AppItemControl source = sender as AppItemControl;
            source.APPLABEL.Visibility = Visibility.Collapsed;

            var imageanim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardImageAnim", source.IMAGEControl);
            var animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", source);
            var textanim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardTextAnim", source.APPNAMEBlock);

            NavigationService.NavigationService.MainFrame.Navigate(typeof(AppInfoPage), source.AppItem, new SuppressNavigationTransitionInfo());
        }
    }
}
