﻿using LMS_Project.Data;
using LMS_Project.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChapterPage : Page
    {

        public string Title { get { return episode.Name; } }
        public ChapterPage()
        {
            this.InitializeComponent();
        }

        Episode episode;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                episode = (e.Parameter as Episode);
            }
        }

        private void MainListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ViewNovelPage), e.ClickedItem, new ContinuumNavigationTransitionInfo());
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {              
                LoadingIndicator.IsActive = true;
                if (MainPage.Menu.SelectedIndex != 1)
                    MainPage.Menu.SelectedIndex = 1;
                if (episode != null)
                {
                    var list = NovelPage.model.GetChaptersFromEpisodeId(episode.EpisodeId).ToList().OrderBy(o => o.NumberInEpisode);
                    MainListView.ItemsSource = list;

                    if (list== null || list.Count() == 0)
                        tbMessage.Text = "Không có chương nào hiện tại ở tập này";
                }
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void MainListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var wrap = MainListView.ItemsPanelRoot as ItemsWrapGrid;
            wrap.ItemWidth = (e.NewSize.Width - 10) / 2;
            if (e.NewSize.Width > 830)
            {
                wrap.ItemWidth = (e.NewSize.Width - 20) / 3;
            }
            if (e.NewSize.Width > 1050)
            {
                wrap.ItemWidth = (e.NewSize.Width - 20) / 4;
            }
        }
    }
}
