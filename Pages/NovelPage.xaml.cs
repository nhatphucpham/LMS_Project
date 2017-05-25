using ColorThiefDotNet;
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
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
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
        
        private List<Novel> novels;
             
        public NovelPage()
        {
            this.InitializeComponent();
        }
        int i = 0, j = 0;
        private GridView AddNewGridView( int count)
        {
            var gridView = new GridView()
            {
                ItemTemplate = Resources["NovelsDataTemplate"] as DataTemplate
            };
            List<Novel> IDs = new List<Novel>();
            while(i < j + count)
            {
                if (novels.Count > i)
                {
                    IDs.Add(novels[i]);
                    i++;
                }
                else
                {
                    break;
                }
            }
            j = i;
            gridView.ItemsSource = IDs;
            gridView.IsItemClickEnabled = true;
            gridView.ItemClick += GridView_ItemClick;
            gridView.SelectionChanged += GridView_SelectionChanged;
            string template = "<ItemsPanelTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
                + "<ItemsWrapGrid Orientation = \"Vertical\" MaximumRowsOrColumns=\"1\"/></ItemsPanelTemplate> ";
            gridView.ItemsPanel = (ItemsPanelTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(template);
            return gridView;
        }

        private async void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var novel = (sender as GridView).SelectedItem as Novel;
                LoadingIndicator.IsActive = true;
                if (NovelPage.model.Sourse == null)
                    NovelPage.model.Sourse = MainPage.WebSource;
                var novels = NovelPage.model.GetNovelFromWebSourse(MainPage.WebSource.Address);

                List<Episode> episodes;

                if (novel != null)
                    episodes = NovelPage.model.GetEpisodesFromNovelId(novel.NovelId);
                else
                    episodes = new List<Episode>();

                if (episodes == null || episodes.Count == 0)
                {
                    bool result = await NovelPage.model.LoadHTLM(NovelPage.model.Sourse.Address);
                    if (!result)
                    {
                        await NovelPage.model.CheckConnection();
                    }
                    NovelPage.model.LoadEpisode(novel.NovelId);
                    episodes = NovelPage.model.GetEpisodesFromNovelId(novel.NovelId);
                }
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem == (sender as GridView).SelectedItem)
                Frame.Navigate(typeof(EpisodePage), e.ClickedItem);
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            var image = grid.Children[0] as Image;
            RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri((image.Source as BitmapImage).UriSource);
            using (IRandomAccessStream stream = await random.OpenReadAsync())
            {
                //Create a decoder for the image
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var colorThief = new ColorThief();
                var color = await colorThief.GetColor(decoder);
                grid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(color.Color.A, color.Color.R, color.Color.G, color.Color.B));
            }
        }

        private void flipView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scrollViewer.Height = scrollViewer.Height;
        }

        private void border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.NewSize.Height < 342.39999389648438)
            {
                border.Margin = new Thickness(25, 25, 25, 0);
                border.Height = 342.39999389648438;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var offset = scrollViewer.HorizontalOffset;
            var View = scrollViewer.ViewportWidth;
            scrollViewer.ChangeView(offset - View, 0, 1);
            rButton.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var offset = scrollViewer.HorizontalOffset;
            var View = scrollViewer.ViewportWidth;
            scrollViewer.ChangeView(offset + View, 0, 1);
            lButton.Visibility = Visibility.Visible;
        }

        private void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (scrollViewer.HorizontalOffset == 0)
                lButton.Visibility = Visibility.Collapsed;
            if (scrollViewer.HorizontalOffset == scrollViewer.ScrollableWidth)
                rButton.Visibility = Visibility.Collapsed;
        }

        private async void scrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainPage.WebSource != null)
                {
                    if (MainPage.WebSource.Name == "Valvrareteam" && !(NovelPage.model is Valvrareteam))
                        NovelPage.model = new Valvrareteam();
                    else if (MainPage.WebSource.Name == "Sublightnovel" && !(NovelPage.model is Sublightnovel))
                        NovelPage.model = new Sublightnovel();
                }

                novels = NovelPage.model.GetNovels(MainPage.WebSource.WebId);

                if (novels == null || novels.Count == 0)
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
                novels = NovelPage.model.GetNovels(MainPage.WebSource.WebId);
                var gv = AddNewGridView(20);
                if ((gv.ItemsSource as List<Novel>).Count > 0)
                    scrollViewer.Content = gv;
                
                //MainGridView.ItemsSource = novels;
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }

        //private void MainGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var wrap = MainGridView.ItemsPanelRoot as ItemsWrapGrid;
        //    if (MainGridView.ItemsSource != null && !((MainGridView.ItemsSource as List<Novel>).Count < 5))
        //    {
        //        if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
        //        {
        //            if (DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Portrait)
        //                wrap.ItemWidth = (e.NewSize.Width - 20) / 2;
        //            if (DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Landscape ||
        //                DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.LandscapeFlipped)
        //                wrap.ItemWidth = (e.NewSize.Width - 30) / 3;
        //        }
        //        else
        //        {
        //            if (e.NewSize.Width > 860)
        //            {
        //                wrap.ItemWidth = (e.NewSize.Width - 50) / 5;
        //            }
        //            else if (e.NewSize.Width > 560)
        //            {
        //                wrap.ItemWidth = (e.NewSize.Width - 40) / 4;
        //            }
        //            else
        //            {
        //                wrap.ItemWidth = (e.NewSize.Width - 30) / 3;
        //            }
        //        }
        //    }
        //}
    }
}
