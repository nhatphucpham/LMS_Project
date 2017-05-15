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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EpisodePage : Page
    {
        private Novel novel;
        public EpisodePage()
        {
            this.InitializeComponent();
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
    }
}
