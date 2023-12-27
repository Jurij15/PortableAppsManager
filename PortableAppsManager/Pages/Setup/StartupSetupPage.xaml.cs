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
using PortableAppsManager.Services;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Services.Store;
using WinUIEx.Messaging;
using PortableAppsManager.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartupSetupPage : Page
    {
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("SetupImageAnim");
            anim.Completed += Anim_Completed;
            if (anim != null)
            {
                //BigTitle.Visibility = Visibility.Collapsed;
                anim.TryStart(LogoImage);
            }
            //await Task.Delay(10);
            //BigTitle.Visibility = Visibility.Visible;
        }

        private async void Anim_Completed(ConnectedAnimation sender, object args)
        {
            TitleGrid.ChildrenTransitions.Clear();
            TitleGrid.ChildrenTransitions.Add(new AddDeleteThemeTransition());
            //MessageBox.Show(TitleGrid.ChildrenTransitions[0].ToString());
            await Task.Delay(100);

            Setup.Visibility = Visibility.Visible;

            BigTitle.Text = "Portable Apps Manager";
            Setup.Text = "Setup";
        }

        public StartupSetupPage()
        {
            this.InitializeComponent();

            BackdropService.ChangeBackdrop(BackdropService.Backdrops.Mica);
        }

        private async void LogoImage_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1300);

            var imageanim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("SetupTitleAnimation", TitleGrid);
            imageanim.Configuration = new BasicConnectedAnimationConfiguration();

            MainWindow.RootAppFrame.Navigate(typeof(ShellSetupPage), null, new SuppressNavigationTransitionInfo());
        }
    }
}
