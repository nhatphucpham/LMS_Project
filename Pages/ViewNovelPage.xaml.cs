using LMS_Project.Data;
using LMS_Project.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewNovelPage : Page
    {
        Chapter chapter;
        List<WebView> webViews;

        public string Title { get { return chapter.Name; } }
        public ViewNovelPage()
        {
            this.InitializeComponent();
            webViews = new List<WebView>();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            chapter = e.Parameter as Chapter;
            if (chapter == null)
            {
                if(MainPage.CurrentChapter != null)
                {
                    chapter = MainPage.CurrentChapter;
                }
                else
                    flView.Items.Add(new TextBlock() { Text = "Cannot see any chapter's history in here!" });
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingIndicator.IsActive = true;
                if (chapter != null)
                {
                    if (NovelPage.model == null)
                    {
                        if (MainPage.cbTitle.ItemsSource == null)
                        {
                            MainPage.cbTitle.ItemsSource = (new DataManager()).WebSourses.ToList().OrderBy(o => o.WebId);
                        }

                        if (SourceAnalysis.GetWebSourceOfChapter(chapter.ChapterId).Name == "Sublightnovel")
                        {
                            MainPage.cbTitle.SelectedIndex = 0;
                            NovelPage.model = new Sublightnovel();
                        }
                        else
                        {
                            MainPage.cbTitle.SelectedIndex = 1;
                            NovelPage.model = new Valvrareteam();
                        }
                        MainPage.WebSource = MainPage.cbTitle.SelectedItem as WebSource;
                    }

                    await NovelPage.model.LoadContent(chapter.ChapterId);

                    if (SourceAnalysis.GetWebSourceOfChapter(chapter.ChapterId).Name == "Sublightnovel")
                        MainPage.cbTitle.SelectedIndex = 0;
                    else
                        MainPage.cbTitle.SelectedIndex = 1;

                    var webView = new WebView();
                    MainPage.CurrentChapter = NovelPage.model.GetChapter(chapter.ChapterId);
                    webView.NavigateToString(NovelPage.model.StringHtml);
                    webView.IsHitTestVisible = false;
                    flView.Items.Add(webView);
                }
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }
    }
}
