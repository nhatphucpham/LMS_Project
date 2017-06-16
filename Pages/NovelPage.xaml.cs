using ColorThiefDotNet;
using LMS_Project.Data;
using LMS_Project.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using Windows.UI;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    public sealed partial class NovelPage : Page
    {
        public static SourceAnalysis model;

        private List<Novel> novels;
        private Novel novel;
        private bool flag;
        private static readonly Random rand = new Random();

        public NovelPage()
        {
            flag = false;
            this.InitializeComponent();
        }
        int i = 0, j = 0;
        private GridView AddNewGridView(int count)
        {
            var gridView = new GridView()
            {
                ItemTemplate = Resources["NovelsDataTemplate"] as DataTemplate
            };
            List<Novel> IDs = new List<Novel>();
            while (i < j + count)
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

        private async Task<Brush> GetColorFromImage(string url)
        {
            RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(new Uri(url));
            using (IRandomAccessStream stream = await random.OpenReadAsync())
            {
                //Create a decoder for the image
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var colorThief = new ColorThief();
                var color = await colorThief.GetColor(decoder);
                return new SolidColorBrush(Windows.UI.Color.FromArgb(color.Color.A, color.Color.R, color.Color.G, color.Color.B));
            }
        }

        private async void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SummanyTextBlock.Text = "Loading...";
                prSummany.IsActive = true;

                novel = (sender as GridView).SelectedItem as Novel;
                ellipse.Fill = new ImageBrush();
                var imageBrush = (ellipse.Fill as ImageBrush);
                imageBrush.ImageSource = new BitmapImage(new Uri(novel.ImageUrl));
                imageBrush.Stretch = Stretch.UniformToFill;

                SummanyBorder.Background = await GetColorFromImage(novel.ImageUrl);

                if (SummanyBorder.Visibility == Visibility.Collapsed)
                    SummanyBorder.Visibility = Visibility.Visible;

                if (NovelPage.model.Sourse == null)
                    NovelPage.model.Sourse = MainPage.WebSource;

                List<Episode> episodes;

                if (novel != null)
                    episodes = NovelPage.model.GetEpisodesFromNovelId(novel.NovelId).ToList();
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
                    episodes = NovelPage.model.GetEpisodesFromNovelId(novel.NovelId).ToList();
                }
                Debug.WriteLine(novel);
            }
            catch
            {
                throw;
            }
            finally
            {
                novel = NovelPage.model.GetNovel(novel.NovelId);
                SummanyTextBlock.Text = "Hiện Chưa có tóm tắt cho truyện này";
                if (novel.Summany != null)
                {
                    SummanyTextBlock.Text = novel.Summany;
                }
                prSummany.IsActive = false;
            }
            Debug.WriteLine(novel.Summany != null ? novel.Summany : "nothing");
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == (sender as GridView).SelectedItem)
                Frame.Navigate(typeof(EpisodePage), e.ClickedItem);
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Common.ShowDialog.getInstance().ShowWaiting("Waiting...");
            var grid = sender as Grid;
            var image = grid.Children[0] as Image;
<<<<<<< HEAD
            grid.Background = await GetColorFromImage((image.Source as BitmapImage).UriSource.OriginalString);

=======
                grid.Background = await GetColorFromImage((image.Source as BitmapImage).UriSource.OriginalString);
>>>>>>> origin/master
            SearchBox.ItemsSource = novels;
            Common.ShowDialog.getInstance().HideWaiting();
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Common.ShowDialog.getInstance().ShowWaiting("Waiting...");
            try
            {
                button.Content = button.Content.ToString().Equals("Tất Cả") ? "Mới Nhất" : "Tất Cả";
                label.Text = label.Text.Equals("Tất Cả") ? "Mới Nhất" : "Tất Cả";
                var gv = new GridView();
                if (flag == false)
                {
                    flag = true;
                    gv = new GridView()
                    {
                        ItemTemplate = Resources["NovelsDataTemplate"] as DataTemplate
                    };

                    gv.ItemsSource = novels;
                    gv.IsItemClickEnabled = true;
                    gv.ItemClick += GridView_ItemClick;
                    gv.SelectionChanged += GridView_SelectionChanged;
                    string template = "<ItemsPanelTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
                        + "<ItemsWrapGrid Orientation = \"Vertical\" MaximumRowsOrColumns=\"1\"/></ItemsPanelTemplate> ";
                    gv.ItemsPanel = (ItemsPanelTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(template);
                }
                else
                {
                    flag = false;
                    novels = NovelPage.model.GetNovels(MainPage.WebSource.WebId).ToList();
                    gv = AddNewGridView(20);
                }
                var lt = (gv.ItemsSource as List<Novel>);
                if (lt.Count > 0)
                    scrollViewer.Content = gv;
                if (lt.Count < 5)
                    rButton.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingIndicator.IsActive = false;
                Common.ShowDialog.getInstance().HideWaiting();
            }

        }

        private void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (scrollViewer.HorizontalOffset == 0)
                lButton.Visibility = Visibility.Collapsed;
            if (scrollViewer.HorizontalOffset == scrollViewer.ScrollableWidth)
                rButton.Visibility = Visibility.Collapsed;
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ellipse.Width = ellipse.ActualHeight;
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var stack = sender as StackPanel;
            var grid = stack.Children[1] as Grid;
            grid.Width = e.NewSize.Width - e.NewSize.Height - 50;
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                var listsearch = new List<Novel>();
                foreach (var item in novels)
                {
                    if (item.Title.ToLower().Contains(sender.Text.ToLower()))
                        listsearch.Add(item);
                }
                SearchBox.ItemsSource = listsearch;
            }
            catch (Exception)
            {

            }
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                novel = args.ChosenSuggestion as Novel;
                Frame.Navigate(typeof(EpisodePage), novel);
            }
            catch (Exception)
            {

            }

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

                novels = NovelPage.model.GetNovels(MainPage.WebSource.WebId).ToList();

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
                novels = NovelPage.model.GetNovels(MainPage.WebSource.WebId).ToList();
                var gv = AddNewGridView(20);
                var lt = (gv.ItemsSource as List<Novel>);
                if (lt.Count > 0)
                    scrollViewer.Content = gv;
                if (lt.Count < 5)
                    rButton.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }
    }
}
