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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditAppDialogContent : Page
    {
        public ContentDialog ParentDialog { get; private set; }
        public string _title;

        public AppItem ModifiedAppItem { get; private set; }
        public EditAppDialogContent(AppItem Item, ContentDialog parentDialog)
        {
            this.InitializeComponent();
            this.DataContext = this;
            ModifiedAppItem = Item;

            _title = $"Edit {ModifiedAppItem.AppName} details";
            ParentDialog = parentDialog;
            parentDialog.UpdateLayout();
        }

        private void DialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            ParentDialog.Hide();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            ParentDialog.Hide();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //ParentDialog.Resources["ContentDialogMinWidth"] = 900;
            ParentDialog.UpdateLayout();
        }
    }
}
