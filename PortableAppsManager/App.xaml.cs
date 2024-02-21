using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Classes;
using PortableAppsManager.Interop;
using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Text.RegularExpressions;
using Windows.Devices.Power;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            bool ShowConsole = true; //get the variable somehow
            if (ShowConsole)
            {
                ConsoleInterop.SetupConsole();

                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
                //comment bottom line to prevent file logging
                .WriteTo.File(Regex.Replace("RuntimeLog" + DateTime.Now.ToString() + ".log", $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", ""), outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj}{NewLine}{Exception}", restrictedToMinimumLevel: LogEventLevel.Verbose)
                .CreateLogger();
                Log.Information("PortableAppsManager by Jurij15, Version: " + Globals.Version);
            }

            ConfigJson.LoadSettings();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();

            Log.Verbose("MainWindow Activated");

            Globals.m_window = m_window;
        }

        private Window m_window;
    }
}
