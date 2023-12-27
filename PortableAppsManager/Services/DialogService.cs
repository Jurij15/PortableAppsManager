using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace PortableAppsManager.Services
{
    public class DialogService
    {
        public static async void ShowSimpleDialog(object Content, string CloseButtonText = null, string Title = "")
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = MainWindow.MainWindowXamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = Title;
            dialog.Content = Content;

            dialog.CloseButtonText = "OK";

            dialog.DefaultButton = ContentDialogButton.Close;

            dialog.ShowAsync();
        }
        public static async Task<object> OpenFilePickerToSelectSingleFile(PickerViewMode ViewMode)
        {
            // Create a file picker
            FileOpenPicker openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = Globals.m_window;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = ViewMode;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a file
            return await openPicker.PickSingleFileAsync();

        }

        public static async Task<StorageFolder> OpenFolderPickerToSelectSingleFolder(PickerViewMode ViewMode)
        {
            // Create a file picker
            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = Globals.m_window;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = ViewMode;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a file
            return await openPicker.PickSingleFolderAsync();

        }
    }
}
