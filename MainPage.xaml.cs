using LMS_Project.Data;
using LMS_Project.Model;
using LMS_Project.Pages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        public static Frame contentFrame;
        public static ComboBox cbTitle;
        public static WebSource WebSource;
        public static Chapter CurrentChapter;
        public MainPage()
        {
            this.InitializeComponent();
            //&#xE700; Hamburger button
            contentFrame = ContentFrame;
            cbTitle = cbSourse;
            Menu = new Model.NavMenu();
            MenuItem.ItemsSource = Menu.MenuItems;
            MenuItem.SelectedIndex = 0;
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                ShowStatusBar();
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

            ContentFrame.Navigated += OnNavigated;

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            if (ContentFrame.CanGoBack && ContentFrame.CurrentSourcePageType != typeof(HomePage))
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        public async Task<Chapter> GetCurrentChapter()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile jsonFile = await folder.GetFileAsync("currentChapter.txt");
                string json = await Windows.Storage.FileIO.ReadTextAsync(jsonFile);
                return JsonConvert.DeserializeObject<Chapter>(json);
            }
            catch
            {
                return null;
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                HeaderButton.Visibility = Visibility.Visible;
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
        //    Frame current = ContentFrame;
        //    if(((NavItem)MenuItem.SelectedItem).Page.GetType() != current.GetType())
        //        current.Navigate(((NavItem)MenuItem.SelectedItem).Page);
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new DataManager())
            {
                context.Episodes.RemoveRange(context.Episodes);
                context.EpisodeDetails.RemoveRange(context.EpisodeDetails);
                context.Chapters.RemoveRange(context.Chapters);
                context.Novels.RemoveRange(context.Novels);
                context.NovelDetails.RemoveRange(context.NovelDetails);
                context.SaveChanges();

                ContentFrame.Navigate(typeof(NovelPage));
            }
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

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
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

        private void CbSourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainPage.WebSource = MainPage.cbTitle.SelectedItem as WebSource;
            if(ContentFrame.Content.GetType() != typeof(HomePage))
                ContentFrame.Navigate(typeof(NovelPage), MainPage.WebSource);
        }

        private void AttachProgressAndCompletedHandlers(IBackgroundTaskRegistration task)
        {
            task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }

        private void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
        {
            var progress = "Progress: " + args.Progress + "%";

        }


        private async void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            await new Sublightnovel().CheckConnection();
        }

        public static BackgroundTaskRegistration RegisterBackgroundTask(
                                                string taskEntryPoint,
                                                string taskName,
                                                IBackgroundTrigger trigger,
                                                IBackgroundCondition condition)
        {

            //
            // Register the background task.
            //

            var builder = new BackgroundTaskBuilder()
            {
                Name = taskName,
                TaskEntryPoint = taskEntryPoint
            };

            builder.SetTrigger(trigger);

            if (condition != null)
            {

                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            return task;

        }

        private async void MySplitView_Loading(FrameworkElement sender, object args)
        {
            TimeTrigger connectCheckTime = new TimeTrigger(15, false);

            SystemCondition userCondition = new SystemCondition(SystemConditionType.UserPresent);

            await BackgroundExecutionManager.RequestAccessAsync();

            BackgroundTaskRegistration task = RegisterBackgroundTask("BackgroundTasks.TimerBackgroundTask", "Check connection every 5 seconds", connectCheckTime, userCondition);

            if (task != null)
            {
                AttachProgressAndCompletedHandlers(task);
            }

            await new Sublightnovel().CheckConnection();
            try
            {
                LoadingIndicator.IsActive = true;
                using (var context = new DataManager())
                {
                    WebSource[] sourses =
                    {
                       new WebSource() { Name = "Sublightnovel", Address = @"http://www.sublightnovel.com/p/home.html" },
                       new WebSource() { Name = "Valvrareteam", Address = @"http://valvrareteam.com/" }
                    };
                    foreach (var sourse in sourses)
                    {
                        if (context.WebSourses.Where(s => s.Name == sourse.Name).Count() == 0)
                        {
                            context.WebSourses.Add(sourse);
                            context.SaveChanges();
                        }
                    }
                    cbSourse.ItemsSource = (new DataManager()).WebSourses.ToList();
                    cbSourse.SelectedIndex = 0;

                    CurrentChapter = await GetCurrentChapter();
                }
            }
            finally
            {
                LoadingIndicator.IsActive = false;
                MainPage.WebSource = MainPage.cbTitle.SelectedItem as WebSource;
            }
        }
    }
}