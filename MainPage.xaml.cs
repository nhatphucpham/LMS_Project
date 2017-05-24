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
using Windows.UI.ViewManagement;
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
        NavMenu navMenu;

        public static Frame contentFrameStatic;
        public static ComboBox cbTitle;
        public static WebSource WebSource;
        public static Chapter CurrentChapter;
        public static ListView Menu;

        public MainPage()
        {
            this.InitializeComponent();
            //&#xE700; Hamburger button
            contentFrameStatic = ContentFrame;
            cbTitle = cbSourse;
            navMenu = new Model.NavMenu();
            MenuItem.ItemsSource = navMenu.MenuItems;
            MenuItem.SelectedIndex = 0;
            Menu = MenuItem;

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                MySplitView.LostFocus += MySplitView_LostFocus;
                HideStatusBar();
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

            contentFrameStatic.Navigated += OnNavigated;

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            if (contentFrameStatic.CanGoBack && contentFrameStatic.CurrentSourcePageType != typeof(HomePage))
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private async void HideStatusBar()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

                var statusbar = StatusBar.GetForCurrentView();

                await statusbar.HideAsync();
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
            Frame current = contentFrameStatic;
            
            if (!current.GetType().Equals(((NavItem)MenuItem.SelectedItem).Page))
            {
                current.Navigate(((NavItem)MenuItem.SelectedItem).Page);
            }

            if(current.Content is HomePage || current.Content is ChapterPage || current.Content is ViewNovelPage)
            {
                UpdateButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                UpdateButton.Visibility = Visibility.Visible;
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
                if (contentFrameStatic.Content is NovelPage)
                {
                    context.Episodes.RemoveRange(context.Episodes);
                    context.EpisodeDetails.RemoveRange(context.EpisodeDetails);
                    context.Chapters.RemoveRange(context.Chapters);
                    context.Novels.RemoveRange(context.Novels);
                    context.NovelDetails.RemoveRange(context.NovelDetails);
                    context.WebDetails.RemoveRange(context.WebDetails);
                    context.SaveChanges();
                    contentFrameStatic.Navigate(typeof(NovelPage));
                }
                if(contentFrameStatic.Content is EpisodePage)
                {
                    context.NovelDetails.RemoveRange(context.NovelDetails);
                    context.Episodes.RemoveRange(context.Episodes);
                    context.EpisodeDetails.RemoveRange(context.EpisodeDetails);
                    context.Chapters.RemoveRange(context.Chapters);
                    context.SaveChanges();
                    contentFrameStatic.Navigate(typeof(EpisodePage), EpisodePage.novel);
                }
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = contentFrameStatic;

            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
                try
                {
                    MenuItem.SelectedIndex = navMenu.GetIndex(rootFrame.GetType()).Value;
                }
                catch
                {
                    MenuItem.SelectedIndex = MenuItem.SelectedIndex;
                }
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
            try
            {
                MenuItem.SelectedIndex = navMenu.GetIndex(((Frame)sender).Content.GetType()).Value;
            }
            catch 
            {
                MenuItem.SelectedIndex = MenuItem.SelectedIndex;
            }
        }

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            HeaderButton.Visibility = Visibility.Collapsed;
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
            if(contentFrameStatic.Content.GetType() != typeof(HomePage))
                contentFrameStatic.Navigate(typeof(NovelPage), MainPage.WebSource);
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
                MainPage.WebSource = MainPage.cbTitle.SelectedItem as WebSource;
            }
        }

        private void MySplitView_LostFocus(object sender, RoutedEventArgs e)
        {
            if(!MySplitView.IsPaneOpen && HeaderButton.Visibility == Visibility.Collapsed)
            {
                HeaderButton.Visibility = Visibility.Visible;
            }
        }
    }
}