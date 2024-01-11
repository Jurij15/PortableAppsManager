using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Pages;
using PortableAppsManager.Pages.Setup;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
        public static Frame RootAppFrame;
        public static XamlRoot MainWindowXamlRoot;

        public static Button AppTitleBarBackButton;
        public MainWindow()
        {
            this.InitializeComponent();

            this.SystemBackdrop = new MicaBackdrop();
            this.Title = "Portable Apps Manager";

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            this.SetWindowSize(1210, 665);
        }

        private void RootFrame_Loaded(object sender, RoutedEventArgs e)
        {
            RootAppFrame = RootFrame;

            RootAppFrame.Navigate(typeof(StartupPage));

            MainWindowXamlRoot = RootGrid.XamlRoot;

            AppTitleBarBackButton = TitleBarBackButton;
        }
    }
}
