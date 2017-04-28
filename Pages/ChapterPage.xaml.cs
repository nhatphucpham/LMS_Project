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
    public sealed partial class ChapterPage : Page
    {
        Chapter chapter;
        Sublightnovel model;
        public ChapterPage()
        {
            this.InitializeComponent();
            model = new Sublightnovel();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            chapter = e.Parameter as Chapter;
        }

        private async void Grid_Loading(FrameworkElement sender, object args)
        {
            try
            {
                LoadingIndicator.IsActive = true;
                chapter = model.GetChapterFromChapterId(chapter.ChapterId);
                await model.SetContent(chapter.ChapterId);
                wpContent.NavigateToString(model.StringHtml);
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }
    }
}
