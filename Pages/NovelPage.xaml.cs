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
                    else if(!(NovelPage.model is Sublightnovel))
                        NovelPage.model = new Sublightnovel();
                }
                if (NovelPage.model.GetNovels(MainPage.WebSource.WebId) == null)
                {
                    LoadingIndicator.IsActive = true;
                    if (SourceAnalysis.CurrentPages == 0 || SourceAnalysis.CurrentPages == 1)
                        NovelPage.model.Sourse = MainPage.WebSource;

                    bool result = await NovelPage.model.LoadHTLM(NovelPage.model.Sourse.Address);
                    if (!result)
                    {
                        await NovelPage.model.CheckConnection();
                    }
                    if (NovelPage.model.GetNovelFromWebSourse(NovelPage.model.Sourse.Address) == null || NovelPage.model.GetNovelFromWebSourse(NovelPage.model.Sourse.Address).Count == 0)
                    {
                        NovelPage.model.Sourse = MainPage.WebSource;

                        NovelPage.model.LoadNovel();
                    }
                    NovelPage.model.LoadNav();

                    foreach (var item in SourceAnalysis.NavLinks)
                    {
                        await NovelPage.model.LoadHTLM(item.Value);
                        NovelPage.model.LoadNovel();
                    }

                }
                MainGridView.ItemsSource = NovelPage.model.GetNovels(MainPage.WebSource.WebId);
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }

        private void MainGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 0)
            {
                throw new System.Exception("We should be in phase 0, but we are not.");
            }
            
            // It's phase 1, so show this item's subtitle.
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;
            var childTemplateRoot = templateRoot.Children[1] as StackPanel;
            var textBlock = childTemplateRoot.Children[0] as TextBlock;
            textBlock.Text = (args.Item as Novel).Title;
            textBlock.Opacity = 1;
            
            args.RegisterUpdateCallback(this.ShowImage);

            args.Handled = true;

        }
        private void ShowImage(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1)
            {
                throw new System.Exception("We should be in phase 1, but we are not.");
            }
            
            // It's phase 0, so this item's title will already be bound and displayed.
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;
            var image = templateRoot.Children[0] as Image;
            image.Source = new BitmapImage(new Uri((args.Item as Novel).ImageUrl));
            image.Opacity = 1;
            
        }

        private void MainGridView_SizeChanged(object sender, SizeChangedEventArgs e)
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
