using LMS_Project.Data;
using LMS_Project.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    public sealed partial class NovelPage : Page
    {
        public static SourceAnalysis model;
        public NovelPage()
        {
            this.InitializeComponent();
        }
        private void MainGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(EpisodePage), e.ClickedItem);
        }

        private async void MainGridView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainPage.WebSource != null)
                {
                    if (MainPage.WebSource.Name == "Valvrareteam" && !(NovelPage.model is Valvrareteam))
                        NovelPage.model = new Valvrareteam();
                    else if(MainPage.WebSource.Name == "Sublightnovel" && !(NovelPage.model is Sublightnovel))
                        NovelPage.model = new Sublightnovel();
                }

                var Novels = NovelPage.model.GetNovels(MainPage.WebSource.WebId);

                if (Novels == null || Novels.Count == 0)
                {
                    LoadingIndicator.IsActive = true;
                    if (SourceAnalysis.CurrentPages == 0 || SourceAnalysis.CurrentPages == 1)
                        NovelPage.model.Sourse = MainPage.WebSource;

                    bool result = await NovelPage.model.LoadHTLM(NovelPage.model.Sourse.Address);
                    if (!result)
                    {
                        await NovelPage.model.CheckConnection();
                    }
                    NovelPage.model.Sourse = MainPage.WebSource;

                    NovelPage.model.LoadNovel();

                    NovelPage.model.LoadNav();

                    foreach (var item in SourceAnalysis.NavLinks)
                    {
                        await NovelPage.model.LoadHTLM(item.Value);
                        NovelPage.model.LoadNovel();
                    }

                }
                Novels = NovelPage.model.GetNovels(MainPage.WebSource.WebId);
                MainGridView.ItemsSource = Novels;
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }

        private void MainGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                var wrap = MainGridView.ItemsPanelRoot as ItemsWrapGrid;
                if (DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Portrait)
                    wrap.ItemWidth = (e.NewSize.Width - 20) / 2;
                if (DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Landscape ||
                    DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                    wrap.ItemWidth = (e.NewSize.Width - 30) / 3;
            }
            else
            {
                if (e.NewSize.Width > 860)
                {
                    var wrap = MainGridView.ItemsPanelRoot as ItemsWrapGrid;
                    wrap.ItemWidth = (e.NewSize.Width - 50) / 5;
                }
                else if (e.NewSize.Width > 560)
                {
                    var wrap = MainGridView.ItemsPanelRoot as ItemsWrapGrid;
                    wrap.ItemWidth = (e.NewSize.Width - 40) / 4;
                }
                else
                {
                    var wrap = MainGridView.ItemsPanelRoot as ItemsWrapGrid;
                    wrap.ItemWidth = (e.NewSize.Width - 30) / 3;
                }
            }
        }
    }
}
