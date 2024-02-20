using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Classes;
using PortableAppsManager.Interop;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class HomePage : Page
    {
        List<AppItem> PinnedApps;
        public HomePage()
        {
            this.InitializeComponent();

            this.DataContext = this;

            PinnedApps = new List<AppItem>();
        }

        private void PinnedAppsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            PinnedApps = Globals.library.GetPinnedApps();
            PinnedAppsPanel.ItemsSource = PinnedApps;
        }

        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
        }

        private void PinnedAppsPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PinnedAppsPanel.DeselectAll();
        }
    }
}
