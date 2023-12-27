using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Services
{
    public class BackdropService
    {
        private static Backdrops _previousbackdrop;
        public enum Backdrops
        {
            None,
            Mica,
            MicaAlt,
            Acrylic
        }

        public static void ChangeBackdrop(Backdrops RequestedBackdrop)
        {
            if (RequestedBackdrop == _previousbackdrop)
            {
                return; //do not set it again to prevent screen flashing
            }
            switch (RequestedBackdrop)
            {
                case Backdrops.None:
                    Globals.m_window.SystemBackdrop = null;
                    (Globals.m_window.Content as Grid).Background = App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;
                    break;
                case Backdrops.Mica:
                    Microsoft.UI.Xaml.Media.MicaBackdrop backdropm = new MicaBackdrop();
                    backdropm.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;

                    Globals.m_window.SystemBackdrop = backdropm;
                    break;
                case Backdrops.MicaAlt:
                    Microsoft.UI.Xaml.Media.MicaBackdrop backdropalt = new MicaBackdrop();
                    backdropalt.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt;

                    Globals.m_window.SystemBackdrop = backdropalt;
                    break;
                case Backdrops.Acrylic:
                    Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop backdropacry = new DesktopAcrylicBackdrop();
                    Globals.m_window.SystemBackdrop = backdropacry;
                    break;
                default:
                    //alt
                    Microsoft.UI.Xaml.Media.MicaBackdrop backdropaltdef = new MicaBackdrop();
                    backdropaltdef.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt;

                    Globals.m_window.SystemBackdrop = backdropaltdef;
                    break;
            }
            _previousbackdrop = RequestedBackdrop;
        }
    }
}
