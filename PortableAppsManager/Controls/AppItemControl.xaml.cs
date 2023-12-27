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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Controls
{
    public sealed partial class AppItemControl : UserControl
    {
        public static readonly DependencyProperty AppNameProperty =
                 DependencyProperty.Register("AppName", typeof(string), typeof(AppItemControl), new PropertyMetadata(null));
        public string AppName
        {
            get { return (string)GetValue(AppNameProperty); }
            set { SetValue(AppNameProperty, value); }
        }

        public static readonly DependencyProperty AppNameSubTextProperty =
                 DependencyProperty.Register("AppNameSubText", typeof(string), typeof(AppItemControl), new PropertyMetadata(null));
        public string AppNameSubText
        {
            get { return (string)GetValue(AppNameSubTextProperty); }
            set { SetValue(AppNameSubTextProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
                 DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(AppItemControl), new PropertyMetadata(null));
        public ImageSource ImgSource
        {
            get { return (ImageSource)GetValue(AppNameSubTextProperty); }
            set { SetValue(AppNameSubTextProperty, value);}
        }

        public Image IMAGEControl;
        public TextBlock APPNAMEBlock { get; set; }
        public TextBlock APPNAMESUBTEXTBlock;
        public AppItemControl()
        {
            this.InitializeComponent();

            this.DataContext = this;

            IMAGEControl = IMG;
            APPNAMEBlock = AppNameTextBlockO;
            APPNAMESUBTEXTBlock = AppNameSubTextBlockO;
        }

        private void SetPointerNormalState(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private void SetPointerOverState(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        private void SetPointerPressedState(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed", true);
        }
    }
}
