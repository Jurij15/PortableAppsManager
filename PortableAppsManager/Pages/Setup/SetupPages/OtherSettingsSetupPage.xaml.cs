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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Media.Animation;
using NavigationService;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages.Setup.SetupPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OtherSettingsSetupPage : Page
    {
        public OtherSettingsSetupPage()
        {
            this.InitializeComponent();
        }

        private async void FinishSetupCard_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RootAppFrame.Navigate(typeof(ShellPage), null, new DrillInNavigationTransitionInfo());

            await Task.Delay(100); //wait for shell to load?

            NavigationService.NavigationService.Navigate(typeof(AppsPage), NavigationService.NavigationService.NavigateAnimationType.Entrance);
        }
    }
}
