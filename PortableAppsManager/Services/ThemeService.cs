using Microsoft.UI.Xaml;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Services
{
    public class ThemeService
    {
        //from https://github.com/microsoft/microsoft-ui-xaml/issues/7009
        private static void SetCapitionButtonColorForWin10()
        {
            var res = Microsoft.UI.Xaml.Application.Current.Resources;
            Action<Windows.UI.Color> SetTitleBarButtonForegroundColor = (Windows.UI.Color color) => { res["WindowCaptionForeground"] = color; };
            var currentTheme = ((FrameworkElement)Globals.m_window.Content).ActualTheme;
            if (currentTheme == ElementTheme.Dark)
            {
                SetTitleBarButtonForegroundColor(Colors.White);
            }
            else if (currentTheme == ElementTheme.Light)
            {
                SetTitleBarButtonForegroundColor(Colors.Black);
            }
            else
            {
                if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    SetTitleBarButtonForegroundColor(Colors.White);
                }
                else
                {
                    SetTitleBarButtonForegroundColor(Colors.Black);
                }
            }
        }
        public static void ChangeTheme(ElementTheme theme)
        {
            if (Globals.m_window.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = theme;
            }

            SetCapitionButtonColorForWin10();
        }
    }
}
