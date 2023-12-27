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
using Microsoft.UI.Xaml.Media.Animation;
using PortableAppsManager.Pages.Setup.SetupPages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellSetupPage : Page
    {
        public static Frame RootSetupFrame;
        public ShellSetupPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("SetupTitleAnimation");
            if (anim != null)
            {
                anim.TryStart(TitleGridDest, new UIElement[] { SetupFrame});
            }
        }

        private void SetupFrame_Loaded(object sender, RoutedEventArgs e)
        {
            RootSetupFrame = SetupFrame;
            ((Frame)sender).Navigate(typeof(AppsDirectorySetupPages));
        }
    }
}
