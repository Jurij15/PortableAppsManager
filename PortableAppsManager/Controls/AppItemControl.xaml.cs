using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Controls
{
    public sealed partial class AppItemControl : UserControl
    {
        public static readonly DependencyProperty AppItemProperty =
                 DependencyProperty.Register("AppItem", typeof(AppItem), typeof(AppItemControl), new PropertyMetadata(null));
        public AppItem AppItem
        {
            get { return (AppItem)GetValue(AppItemProperty); }
            set { SetValue(AppItemProperty, value); }
        }

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

        public static readonly DependencyProperty LabelTextProperty =
                 DependencyProperty.Register("LabelText", typeof(string), typeof(AppItemControl), new PropertyMetadata(null));
        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
                 DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(AppItemControl), new PropertyMetadata(null));
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value);}
        }

        public Image IMAGEControl;
        public TextBlock APPNAMEBlock { get; set; }
        public TextBlock APPNAMESUBTEXTBlock;
        public Button APPLABEL;
        public MetadataControl MetadataControl;

        List<MetadataItem> DisplayTags { get; set; }
        public AppItemControl()
        {
            this.InitializeComponent();

            this.DataContext = this;

            IMAGEControl = IMG;
            APPNAMEBlock = AppNameTextBlockO;
            APPNAMESUBTEXTBlock = AppNameSubTextBlockO;
            APPLABEL = CardBtn;
            MetadataControl = Tags;
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

        public void PlayLaunchAnimationOnLabel()
        {
            LoadingRing.Visibility = Visibility.Visible;
            LabelTextBlock.Visibility = Visibility.Collapsed;
        }
        public async void StopLaunchAnimationOnLabel(bool Success, int StatusDisplayMiliseconds)
        {
            LoadingRing.Visibility = Visibility.Collapsed;
            LabelTextBlock.Visibility = Visibility.Collapsed;

            if (StatusDisplayMiliseconds != 0)
            {
                if (Success)
                {
                    SuccessIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    FailIcon.Visibility = Visibility.Visible;
                }

                await Task.Delay(StatusDisplayMiliseconds);
            }

            SuccessIcon.Visibility = Visibility.Collapsed;
            FailIcon.Visibility = Visibility.Collapsed;

            LabelTextBlock.Visibility = Visibility.Visible;
        }

        private void Tags_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayTags = new List<MetadataItem>();
            if (AppItem.Tags != null)
            {
                foreach (string item in AppItem.Tags)
                {
                    DisplayTags.Add(new MetadataItem() { AccessibleLabel = item, Label = item });
                }
            }

            Tags.Items = DisplayTags;
        }

        public event RoutedEventHandler CardLabelBtn_Clicked;

        private void CardBtn_Click(object sender, RoutedEventArgs e)
        {
            CardLabelBtn_Clicked?.Invoke(sender, e);
        }

    }
}
