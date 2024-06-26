using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Interop;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            this.InitializeComponent();

            if (Globals.Settings.PortableAppsDirectory is not null)
            {
                Globals.library = new Managers.LibraryManager(Globals.Settings.PortableAppsDirectory);
            }
            else
            {
                //it is null, bad
                MessageBox.Show("PortableAppsDirectory is null", "Failed to initialize library");
            }
        }

        private void NavigationViewControl_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {

        }

        private void MainFrame_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigationService.Initialize(MainNavigation, MainBreadcrum, MainFrame);
            NavigationService.NavigationService.Navigate(typeof(HomePage), NavigationService.NavigationService.NavigateAnimationType.Entrance);

            if (!Globals.library.IsLibraryAvailable())
            {
                //library unavailable
                DialogService.ShowSimpleDialog("Library directory not found or is unavailable. Most app functions will not work!", "OK", "Library Unavailable");
            }
        }
    }
}
