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
using System.Threading.Tasks;
using Windows.ApplicationModel.UserDataTasks;
using Windows.Media.AppBroadcasting;
using Windows.Storage;
using PortableAppsManager.Services;
using PortableAppsManager.Core;
using PortableAppsManager.Classes;
using PortableAppsManager.Helpers;
using PortableAppsManager.Interop;
using System.Text.RegularExpressions;
using PortableAppsManager.Dialogs;
using CommunityToolkit.WinUI;
using System.Diagnostics;
using System.Collections;
using Microsoft.UI.Xaml.Media.Animation;
using WinRT;
using IniParser;
using IniParser.Model;
using Microsoft.UI.Xaml.Media.Imaging;
using ABI.Windows.AI.MachineLearning;
using PortableAppsManager.Pages.Settings;
using PortableAppsManager.Structs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages.Setup.SetupPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsDirectorySetupPages : Page
    {
        public AppsDirectorySetupPages()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Directory.Exists(PathTextBox.Text))
            {
                ContinueSetupCard.IsEnabled = true;
            }
        }

        private async void PickPathBtn_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await DialogService.OpenFolderPickerToSelectSingleFolder(Windows.Storage.Pickers.PickerViewMode.List);
            PathTextBox.Text = folder.Path;
        }

        private void ContinueSetupCard_Click(object sender, RoutedEventArgs e)
        {
            Globals.Settings.PortableAppsDirectory = PathTextBox.Text;
            Globals.library = new Managers.LibraryManager(Globals.Settings.PortableAppsDirectory);

            ShellSetupPage.RootSetupFrame.Navigate(typeof(AppLibraryManagementPage), new AppLibraryManagementPageNavigationParams() { NavigateToWhenSaved = typeof(OtherSettingsSetupPage), IgnoreAlreadyExisting = false, NavigationFrame = ShellSetupPage.RootSetupFrame, NavigationTransitionNfo = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight } }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
