using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Media.Animation;
using PortableAppsManager.Services;
using PortableAppsManager.Pages.Setup;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartupPage : Page
    {
        public StartupPage()
        {
            this.InitializeComponent();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(800); //delay of 250 ms

            bool SetupNeeded = Globals.IsFirstTimeRun;
            if (SetupNeeded)
            {
                var anim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("SetupImageAnim", LauncherImage);
                anim.Configuration = new BasicConnectedAnimationConfiguration();
                MainWindow.RootAppFrame.Navigate(typeof(StartupSetupPage), null, new SuppressNavigationTransitionInfo());
            }
            else 
            {
                MainWindow.RootAppFrame.Navigate(typeof(ShellPage), null, new EntranceNavigationTransitionInfo());
            }
        }
    }
}
