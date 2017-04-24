using LMS_Project.Data;
using LMS_Project.Model;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllChapterPage : Page
    {
        public Sublightnovel model;
        public AllChapterPage()
        {
            this.InitializeComponent();
            model = new Sublightnovel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                Episode episode = e.Parameter as Episode;
                MainGridView.ItemsSource = model.GetChaptersFromEpisodeId(episode.EpisodeId);
            }
            else
            {
                MainGridView.ItemsSource = model.GetChaptersList();
            }
        }

        private void MainGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ChapterPage), e.ClickedItem as Chapter);
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (((TextBlock)sender).Text.Length > 28)
                ((TextBlock)sender).Text = string.Format("{0}...",((TextBlock)sender).Text.Remove(28));
        }

        private void TextBlock_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (((TextBlock)sender).Text.Length > 40)
                ((TextBlock)sender).Text = string.Format("{0}...", ((TextBlock)sender).Text.Remove(40));
        }
        
        private void TextBlock_Loaded_2(object sender, RoutedEventArgs e)
        {
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            Windows.UI.Color c = uiSettings.GetColorValue(UIColorType.Accent);
            ((TextBlock)sender).Foreground = new SolidColorBrush(c);
        }
    }
}
