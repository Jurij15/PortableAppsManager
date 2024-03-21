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
using PortableAppsManager.Classes;
using IniParser.Model;
using IniParser;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Text.RegularExpressions;
using Windows.Web.Http.Diagnostics;
using PortableAppsManager.Services;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddAppDialog : Page
    {
        public ContentDialog ParentDialog { get; private set; }
        public string _title;

        public bool HasCanceledAddingApp {  get; private set; }
        public AppItem ModifiedAppItem { get; private set; }
        public AddAppDialog(ContentDialog parent)
        {
            this.InitializeComponent();
            ParentDialog = parent;

            _title = $"Add App";
        }


        private void DialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            HasCanceledAddingApp = true;
            ParentDialog.Hide();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AppPathBox.Text is null or "")
            {
                HasCanceledAddingApp = true;
            }
            ParentDialog.Hide();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //ParentDialog.Resources["ContentDialogMinWidth"] = 900;
            ParentDialog.UpdateLayout();
        }

        private void AppPathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(AppPathBox.Text))
            {
                if (Path.GetExtension(AppPathBox.Text) != ".exe")
                {
                    return;
                }
            }
            else
            {
                return;
            }
            AppItem item = new AppItem();

            string n = Path.GetFileNameWithoutExtension(AppPathBox.Text);
            string res = Regex.Replace(n, @"Portable", "", RegexOptions.IgnoreCase);
            item.ExePath = AppPathBox.Text;
            item.Tags = new List<string>();

            //reset the portableapps variables
            item.PortableApps_IsShareable = false;
            item.PortableApps_IsFreeware = false;
            item.PortableApps_IsOpenSource = false;
            item.PortableApps_DisplayVersion = "";
            item.PortableApps_IsCommercialUse = false;
            item.PortableApps_Homepage = "https://portableapps.com/";
            item.PortableApps_PackageVersion = "";
            //item.ImgSource = ImageHelper.ConvertIconToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(found.ExecutablePath));
            if (File.Exists(Path.Combine(Path.GetDirectoryName(AppPathBox.Text), "App", "AppInfo", "appinfo.ini")))
            {
                //this is a PortableAppsApp, we can get as much info as possible
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(Path.Combine(AppPathBox.Text, "App", "AppInfo", "appinfo.ini"));

                item.AppName = Regex.Replace(data["Details"]["Name"], @"Portable", "", RegexOptions.IgnoreCase);
                item.Author = data["Details"]["Publisher"];
                item.Description = data["Details"]["Description"];
                item.PortableApps_Homepage = data["Details"]["Homepage"];
                if (!item.PortableApps_Homepage.Contains("http")) //some apps do not have the correct url
                {
                    item.PortableApps_Homepage = "https://" + item.PortableApps_Homepage;
                }

                item.Tags.Add(data["Details"]["Category"]);

                item.PortableApps_IsOpenSource = Convert.ToBoolean(data["License"]["OpenSource"]);
                item.PortableApps_IsFreeware = Convert.ToBoolean(data["License"]["Freeware"]);

                item.PortableApps_IsShareable = Convert.ToBoolean(data["License"]["Freeware"]);
                item.PortableApps_IsCommercialUse = Convert.ToBoolean(data["License"]["Freeware"]);

                item.Language = data["Details"]["Language"];

                item.PortableApps_PackageVersion = data["Version"]["PackageVersion"];
                item.PortableApps_DisplayVersion = data["Version"]["DisplayVersion"];

                //lets try to get a higer quality image
                if (File.Exists(Path.Combine(AppPathBox.Text, "App", "AppInfo", "appicon_128.png")))
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.UriSource = new System.Uri(Path.Combine(AppPathBox.Text, "App", "AppInfo", "appicon_128.png"));
                    item.AppImageSourcePath = Path.Combine(AppPathBox.Text, "App", "AppInfo", "appicon_128.png");
                    item.SourceType = Enums.AppImageSourceType.File;
                }
                else
                {
                    item.SourceType = Enums.AppImageSourceType.Executable;
                }
            }

            ModifiedAppItem = item;
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await DialogService.OpenFilePickerToSelectSingleFile(Windows.Storage.Pickers.PickerViewMode.Thumbnail) as StorageFile;

            AppPathBox.Text = file.Path;
        }
    }
}
