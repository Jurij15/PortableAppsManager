using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PortableAppsManager.Core;
using PortableAppsManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using PortableAppsManager.Interop;
using PortableAppsManager.Helpers;
using PortableAppsManager.Classes;
using Windows.Devices.Display.Core;
using PortableAppsManager.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Diagnostics;
using System.Threading.Tasks;
using PortableAppsManager.Enums;
using Microsoft.UI;
using PortableAppsManager.Dialogs;
using CommunityToolkit.WinUI.UI.Triggers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PortableAppsManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsPage : Page
    {
        Launcher launcher;
        TagsService tags;
        bool AppsItemsLoaded = false;

        List<string> _currentTags;

        public class DisplayApp
        {
            public DisplayApp(string name, ImageSource imageSource, AppItem appItem)
            {
                Name = name;
                ImgSrc = imageSource;
                OAppItem = appItem;
            }
            public string Name { get; set; }
            public ImageSource ImgSrc { get; set; }

            public AppItem OAppItem { get; set; }
        }

        AppItemControl CachedItem;

        AppItemModificationType AppWasModified;
        public AppsPage()
        {
            this.InitializeComponent();

            this.DataContext = this;
            launcher = new Launcher();
            tags = new TagsService();
        }

        private bool IsItemInArray(string item, HashSet<string> array)
        {
            foreach (var element in array)
            {
                if (element == item)
                {
                    return true;
                }
            }
            return false;
        }

        private void LoadApps(HashSet<string> Tags, string ContainingName)
        {
            AppItems.Items.Clear();

            AppItems.Visibility = Visibility.Collapsed;
            LoadingGrid.Visibility = Visibility.Visible;

            foreach (var item in Globals.library.GetAllApps())
            {
                bool ShouldAddApp = true;
                AppItemControl control = new AppItemControl();
                control.AppItem = item;
                if (!string.IsNullOrEmpty(ContainingName) || !string.IsNullOrWhiteSpace(ContainingName))
                {
                    if (item.AppName.Contains(ContainingName))
                    {
                        ShouldAddApp = true;
                    }
                    else
                    {
                        ShouldAddApp = false;
                    }
                }
                else
                {
                    control.AppName = item.AppName;
                }
                control.AppName = item.AppName;
                control.IMAGEControl.Source = ImageHelper.GetImageSource(item);
                control.Width = 315;
                if (Launcher.IsAppLaunchAvailable(item.ExePath))
                {
                    control.LabelText = "Open";
                }
                else
                {
                    control.LabelText = "Info";
                }
                control.APPLABEL.Tag = control;
                control.CardLabelBtn_Clicked += Control_CardLabelBtn_Clicked;

                control.PointerReleased += OnPointerReleased;

                foreach (var tag in item.Tags)
                {
                    if (tag == "Games")
                    {
                        ShouldAddApp = false;
                    }
                    continue;
                }

                if (Tags != null)
                {
                    int containingTags = 0;
                    foreach (var tag in item.Tags)
                    {
                        if (IsItemInArray(tag, Tags))
                        {
                            containingTags++;
                        }
                    }

                    if (containingTags > 0)
                    {
                        ShouldAddApp = true;
                    }
                    else
                    {
                        ShouldAddApp = false;
                    }
                }

                if (ShouldAddApp)
                {
                    AppItems.Items.Add(control);
                }
            }

            if (CachedItem is not null)
            {
                foreach (AppItemControl item in AppItems.Items)
                {
                    if (item.AppItem.ID == CachedItem.AppItem.ID)
                    {
                        AppItems.ScrollIntoView(item, ScrollIntoViewAlignment.Default);
                        break;
                    }
                }
            }

            AppItems.Visibility = Visibility.Visible;
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //MessageBox.Show(e.SourcePageType.ToString(), "NavigatedTo Fired!");
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                AppWasModified = (AppItemModificationType)e.Parameter;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private async void AppItems_Loaded(object sender, RoutedEventArgs e)
        {
            AppItems.ScrollIntoView(CachedItem);

            if (AppWasModified == AppItemModificationType.Modified)
            {
                AppItem item = null;
                foreach (var listitem in Globals.library.GetAllApps())
                {
                    if (listitem.ID == CachedItem.AppItem.ID)
                    {
                        item = listitem;
                    }
                }

                if (item != null)
                {
                    CachedItem.AppItem = item;
                    CachedItem.AppName = item.AppName;
                    CachedItem.IMAGEControl.Source = ImageHelper.GetImageSource(item);
                    CachedItem.Width = 315;
                    if (Launcher.IsAppLaunchAvailable(item.ExePath))
                    {
                        (CachedItem.APPLABEL.Tag as AppItemControl).LabelText = "Open";
                    }
                    else
                    {
                        (CachedItem.APPLABEL.Tag as AppItemControl).LabelText = "Info";
                    }
                }
            }
            else if (AppWasModified == AppItemModificationType.Deleted)
            {
                AppItems.Items.Remove(CachedItem);
            }

            //ConnectedAnimationService.GetForCurrentView().DefaultDuration = new TimeSpan(0, 0, 0, 4, 400);
            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(CachedItem);
            }

            var imganim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackImageAnim");
            if (imganim != null)
            {
                imganim.TryStart(CachedItem.IMAGEControl);
            }

            var textanim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackTextAnim");
            if (textanim != null)
            {
                textanim.TryStart(CachedItem.APPNAMEBlock);
            }

            var buttonanim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackButtonAnim");
            if (buttonanim != null)
            {
                buttonanim.TryStart(CachedItem.APPLABEL);
            }

            await Task.Delay(70);

            LoadApps(null, "");
        }

        private async void Control_CardLabelBtn_Clicked(object sender, RoutedEventArgs e)
        {
            AppItemControl control = (sender as Button).Tag as AppItemControl;

            bool res = Launcher.IsAppLaunchAvailable(control.AppItem.ExePath);
            if (res)
            {
                control.PlayLaunchAnimationOnLabel();

                launcher.Launch(control.AppItem);

                await Task.Delay(1000);
                control.StopLaunchAnimationOnLabel(true, 900);
            }
            else if (!res)
            {
                ContentDialog dialog = DialogService.CreateBlankContentDialog(true);

                dialog.Content = $"Executable Path: {control.AppItem.ExePath} \n";
                dialog.Title = "Saved Program Information";

                dialog.DefaultButton = ContentDialogButton.Close;
                dialog.CloseButtonText = "Close";

                dialog.PrimaryButtonText = "Show More";

                dialog.PrimaryButtonClick += (o, i) =>
                {
                    dialog.Hide();
                    NavigateToMoreDetails(control, true);
                };

                await dialog.ShowAsync();
            }
            else
            {
                //OnPointerReleased((sender as Button).Tag, null);
            }
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            AppItemControl source = sender as AppItemControl;
            NavigateToMoreDetails(source, false);
        }

        private async void NavigateToMoreDetails(AppItemControl source, bool IgnoreMissingExe = false)
        {
            if (!Launcher.IsAppLaunchAvailable(source.AppItem.ExePath) && !IgnoreMissingExe)
            {
                DialogService.ShowSimpleDialog("Cannot find program executable. Please check the path or restart the app!", "OK", "Executable Missing");
                return;
            }

            ConnectedAnimationService.GetForCurrentView().DefaultDuration = new TimeSpan(0, 0, 0, 0, 400);
            //ConnectedAnimationService.GetForCurrentView().DefaultDuration = new TimeSpan(0,0,0,4, 400);

            AppItems.ScrollIntoView(source);

            var imageanim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardImageAnim", source.IMAGEControl);
            var animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", source);
            var textanim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardTextAnim", source.APPNAMEBlock);
            var buttonanim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardButtonAnim", source.APPLABEL);

            //source.APPLABEL.Visibility = Visibility.Collapsed;
            source.MetadataControl.Visibility = Visibility.Collapsed;
            await Task.Delay(100);

            NavigationService.NavigationService.MainFrame.Navigate(typeof(AppInfoPage), source.AppItem, new SuppressNavigationTransitionInfo());

            source.APPLABEL.Visibility = Visibility.Visible;
            source.MetadataControl.Visibility = Visibility.Visible;

            CachedItem = source;
        }

        private void SearchBox_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void SearchBox_TokenItemAdding(CommunityToolkit.WinUI.Controls.TokenizingTextBox sender, CommunityToolkit.WinUI.Controls.TokenItemAddingEventArgs args)
        {

        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            foreach (var item in tags.GetAllTags())
            {
                if (item == SearchBox.Text)
                {
                    _currentTags.Add(item);
                }
            }

            //SearchBox.SuggestedItemsSource = _currentTags;

            LoadApps(null, SearchBox.Text);
        }

        private void MoreOptionsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void AddAppBtn_Click(object sender, RoutedEventArgs e)
        {
            MoreOptionsFlyout.Hide();

            MenuFlyoutItem parent = sender as MenuFlyoutItem;
            ContentDialog adddialog = DialogService.CreateBlankContentDialog(false);
            AddAppDialog content = new AddAppDialog(adddialog);

            adddialog.Content = content;
            adddialog.UpdateLayout();

            await adddialog.ShowAsync();
            adddialog.UpdateLayout();

            if (!content.HasCanceledAddingApp)
            {
                Globals.library.AddAppToLibrary(content.ModifiedAppItem);
            }

            LoadApps(null, null);
        }

        private void TagsCombo_Loaded(object sender, RoutedEventArgs e)
        {
            TagsCombo.ItemsSource = tags.GetAllTags();
        }

        private void TagsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearSortBtn.Visibility = Visibility.Visible;
            try
            {
                if (TagsCombo.SelectedItem.ToString() is null | TagsCombo.SelectedItem.ToString() is "")
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
           
            LoadApps(new HashSet<string>() { TagsCombo.SelectedItem.ToString() }, null);
        }

        private void ClearSortBtn_Click(object sender, RoutedEventArgs e)
        {
            TagsCombo.SelectedItem = null;
            TagsCombo.ItemsSource = null;
            TagsCombo.SelectedItem = null;
            TagsCombo_Loaded(null, null);
            TagsCombo.SelectedItem = null;
            TagsCombo.Text = "";

            //reload apps again
            LoadApps(null, null);

            ClearSortBtn.Visibility = Visibility.Collapsed;
        }
    }
}
