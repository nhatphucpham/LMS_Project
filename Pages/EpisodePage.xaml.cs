using LMS_Project.Data;
using System;
using LMS_Project.Model;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.Graphics.Display;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EpisodePage : Page
    {
        public static Novel novel;

        public string ImageUrl { get { return novel.ImageUrl; } }
        public string Title { get { return novel.Title; } }

        public EpisodePage()
        {
            this.InitializeComponent();
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                MainGridView.Margin = new Thickness(0, 200, 0, 0);
                LoadingIndicator.Margin = new Thickness(0, 200, 0, 0);
                NovelGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                NovelGrid.VerticalAlignment = VerticalAlignment.Top;
                NovelGrid.Width = double.NaN;
                NovelGrid.Height = 195;

                NovelGrid.RowDefinitions[0].Height = new GridLength(200, GridUnitType.Star);
                NovelGrid.RowDefinitions[1].Height = new GridLength(70, GridUnitType.Star);

                var ellipse = NovelGrid.Children[0] as Windows.UI.Xaml.Shapes.Ellipse;
                ellipse.Width = 120;
                ellipse.Height = 120;

                var title = NovelGrid.Children[1] as TextBlock;
                title.FontSize = 18;

                NovelGrid.PointerEntered += NovelGrid_PointerPressed;
                NovelGrid.PointerMoved += NovelGrid_PointerMoved;
                NovelGrid.PointerReleased += NovelGrid_PointerReleased;

            }
            else
            {
                MainGridView.Margin = new Thickness(345, 0, 0, 0);
                LoadingIndicator.Margin = new Thickness(345, 0, 0, 0);
                NovelGrid.HorizontalAlignment = HorizontalAlignment.Left;
                NovelGrid.VerticalAlignment = VerticalAlignment.Stretch;
                NovelGrid.Width = 340;
                NovelGrid.Height = double.NaN;

                NovelGrid.RowDefinitions[0].Height = new GridLength(213, GridUnitType.Star);

                var ellipse = NovelGrid.Children[0] as Windows.UI.Xaml.Shapes.Ellipse;
                ellipse.Width = 300;
                ellipse.Height = 300;

                var title = NovelGrid.Children[1] as TextBlock;
                title.FontSize = 25;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                novel = e.Parameter as Novel;
            }
        }

        private void MainGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ChapterPage), e.ClickedItem);
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
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
                MainGridView.ItemsSource = episodes;
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }

        private bool Capture = false;
        Windows.UI.Input.PointerPoint pointerPoint;

        private void NovelGrid_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(NovelGrid);
            if (Capture)
            {
                var value = (pointerPoint.Position.Y - point.Position.Y);
                if (point.Position.Y < pointerPoint.Position.Y)
                {
                    if (NovelGrid.Height > 21)
                    {
                        MainGridView.Margin = new Thickness(0, 200 - value, 0, 0);
                        LoadingIndicator.Margin = new Thickness(0, 200 - value, 0, 0);

                        NovelGrid.Height = 195 - value;
                    }
                    else NovelGrid.Height = 15;


                }
                value = -value;
                if (point.Position.Y > pointerPoint.Position.Y)
                {
                    if (NovelGrid.Height < 194)
                    {
                        MainGridView.Margin = new Thickness(0, 20 + value, 0, 0);
                        LoadingIndicator.Margin = new Thickness(0, 20 + value, 0, 0);

                        NovelGrid.Height = 15 + value;
                    }
                    else NovelGrid.Height = 195;
                }
            }
        }

        private void NovelGrid_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Capture = false;
            if (NovelGrid.Height > 195 / 2)
            {
                MainGridView.Margin = new Thickness(0, 200, 0, 0);
                LoadingIndicator.Margin = new Thickness(0, 200, 0, 0);

                NovelGrid.Height = 195;
            }
            else
            {
                MainGridView.Margin = new Thickness(0, 20, 0, 0);
                LoadingIndicator.Margin = new Thickness(0, 20, 0, 0);

                NovelGrid.Height = 15;
            }
        }

        private void NovelGrid_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact)
            {
                Capture = true;
                pointerPoint = e.GetCurrentPoint(NovelGrid);
            }
        }

        private void MainGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                var wrap = MainGridView.ItemsPanelRoot as ItemsWrapGrid;
                if (DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Portrait)
                    wrap.ItemWidth = e.NewSize.Width  / 2;
                if (DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Landscape ||
                    DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                    wrap.ItemWidth = e.NewSize.Width / 3;
            }
            else
            {
                if (e.NewSize.Width > 860)
                {
                    var wrap = MainGridView.ItemsPanelRoot as ItemsWrapGrid;
                    wrap.ItemWidth = (e.NewSize.Width - 30) / 5;
                }
                else if (e.NewSize.Width > 560)
                {
                    var wrap = MainGridView.ItemsPanelRoot as ItemsWrapGrid;
                    wrap.ItemWidth = (e.NewSize.Width - 30) / 4;
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
