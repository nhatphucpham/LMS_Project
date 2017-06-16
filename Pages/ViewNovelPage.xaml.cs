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

        public string Title { get { return chapter != null ? chapter.Name : "Không tìm thấy"; } }
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
                if (MainPage.CurrentChapter != null)
                {
                    chapter = MainPage.CurrentChapter;
                }
                else
                    wView.NavigateToString("<b><p style=\"font-size: 24; display: flex; text-align: center;\">Không có chương nào đã chọn</p></b>");
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

                    if(NovelPage.model.GetChapter(chapter.ChapterId).Content == null)
                        await NovelPage.model.LoadContent(chapter.ChapterId);

                    if (SourceAnalysis.GetWebSourceOfChapter(chapter.ChapterId).Name == "Sublightnovel")
                        MainPage.cbTitle.SelectedIndex = 0;
                    else
                        MainPage.cbTitle.SelectedIndex = 1;
                    
                    MainPage.CurrentChapter = NovelPage.model.GetChapter(chapter.ChapterId);
                    wView.NavigateToString(NovelPage.model.StringHtml);
                }
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }
    }
}
