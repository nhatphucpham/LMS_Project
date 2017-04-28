using LMS_Project.Data;
using LMS_Project.Model;
using LMS_Project.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LMS_Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        NavMenu Menu;
        public MainPage()
        {
            this.InitializeComponent();
            //&#xE700; Hamburger button
            Menu = new Model.NavMenu();
            MenuItem.ItemsSource = Menu.MenuItems;
            MenuItem.SelectedIndex = 0;
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                ShowStatusBar();
<<<<<<< HEAD
                MySplitView.IsPaneOpen = false;
                HeaderButton.Visibility = Visibility.Visible;
                MySplitView.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            else
            {
                MySplitView.IsPaneOpen = false;
                HeaderButton.Visibility = Visibility.Collapsed;
                MySplitView.DisplayMode = SplitViewDisplayMode.CompactInline;
            }

=======
            }
>>>>>>> master
            ContentFrame.Navigated += OnNavigated;

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            if (ContentFrame.CanGoBack && ContentFrame.GetType() != typeof(HomePage))
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                HeaderButton.Visibility = Visibility.Visible;
=======
            HeaderButton.Visibility = Visibility.Visible;
>>>>>>> master
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void MenuItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuItem.SelectedIndex == -1) { return; }
            Frame current = ContentFrame;
            
            if (!current.GetType().Equals(((NavItem)MenuItem.SelectedItem).Page))
            {
                current.Navigate(((NavItem)MenuItem.SelectedItem).Page);
            }
        }

        private void MySplitView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MySplitView.IsPaneOpen = false;
        }

        private void MenuItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame current = ContentFrame;
            if(((NavItem)MenuItem.SelectedItem).Page.GetType() != current.GetType())
                current.Navigate(((NavItem)MenuItem.SelectedItem).Page);
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        { 
            //if (SourseManager.Instance().UpdateInfomartion())
            //{
            //    await new MessageDialog("Updated!").ShowAsync();
            //}
            //Frame current = ContentFrame;
            //if (((NavItem)MenuItem.SelectedItem).Page.GetType() != current.GetType())
            //    current.Navigate(((NavItem)MenuItem.SelectedItem).Page);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //if (AllChapterPage.ChapterSelected != null)
            //{
            //    SourseManager.Instance().DeleteChapter(AllChapterPage.ChapterSelected);
            //    AllChapterPage.Delete();
            //}
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = ContentFrame;

            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
                MenuItem.SelectedIndex = Menu.GetIndex(rootFrame.GetType());
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            if (((Frame)sender).CanGoBack && ((Frame)sender).Content.GetType() != typeof(HomePage))
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            HeaderButton.Visibility = Visibility.Collapsed;
        }

        private async void ShowStatusBar()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusbar.ShowAsync();
                statusbar.BackgroundColor = (gridPane.Background as SolidColorBrush).Color;
                statusbar.BackgroundOpacity = 1;
                statusbar.ForegroundColor = Windows.UI.Colors.White;
            }

        }

<<<<<<< HEAD
        private async void cbSourse_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingIndicator.IsActive = true;
                using (var context = new SourseManager())
                {
                    var sourse = new WebSourse() { Name = "Sublightnovel", Address = @"http://www.sublightnovel.com/p/home.html" };
                    if (context.Sourse.Where(s => s.Name == sourse.Name).Count() == 0)
                    {
                        context.Sourse.Add(sourse);
                        await context.SaveChangesAsync();
                    }
                    cbSourse.ItemsSource = context.Sourse.ToList();
                    cbSourse.SelectedIndex = 0;
                }
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                cbSourse.Width = canvas.ActualWidth - 60;
            }
            else
            {
                Canvas.SetLeft(cbSourse, 0);
                cbSourse.Width = canvas.ActualWidth - 5;
            }
        }
=======
>>>>>>> master
    }
}