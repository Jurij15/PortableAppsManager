using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using PortableAppsManager.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Structs
{
    public struct AppLibraryManagementPageNavigationParams
    {
        public Type NavigateToWhenSaved;
        public bool IgnoreAlreadyExisting;

        //only needed for first time setup
        public Frame NavigationFrame;
        public NavigationTransitionInfo NavigationTransitionNfo;

        public AppLibraryManagementPageNavigationParams() 
        { 
            NavigateToWhenSaved = typeof(AppsPage);
            IgnoreAlreadyExisting = true;

            NavigationFrame = null;
            NavigationTransitionNfo = null;
        }
    }
}
