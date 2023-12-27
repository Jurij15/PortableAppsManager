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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsPage : Page
    {
        public AppsPage()
        {
            this.InitializeComponent();
        }

        private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFolder dir = await DialogService.OpenFolderPickerToSelectSingleFolder(Windows.Storage.Pickers.PickerViewMode.List);

            Driller driller = new Driller();
            List<Driller.DrillerFoundApp> list = driller.GetAllAppsInsideDirectory(dir.Path);
            foreach (Driller.DrillerFoundApp item in list)
            {
                //MessageBox.Show(item.Source + "\n" + item.ExecutablePath + "\n" + item.ExecutableParentDirectoryPath);
                AppItems.Items.Add(new Controls.AppItemControl() { AppName = item.ExecutablePath, AppNameSubText = item.ExecutableParentDirectoryPath, ImgSource = ImageHelper.ConvertIconToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(item.ExecutablePath)) });
            }

            
        }
    }
}
