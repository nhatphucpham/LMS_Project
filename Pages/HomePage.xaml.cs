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
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    public sealed partial class HomePage : Page
    {

        Sublightnovel sourse;
        public HomePage()
        {
            this.InitializeComponent();
            sourse = new Sublightnovel();
        }
        private void MainGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(AllChapterPage), e.ClickedItem as Episode);
        }

        private async void MainGridView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingIndicator.IsActive = true;
                if (sourse.GetEpisodesList() == null || sourse.GetEpisodesList().Count == 0)
                {
                    await sourse.LoadHTLM();
                    sourse.LoadData();
                }

                MainGridView.ItemsSource = sourse.GetEpisodesList();
            }
            finally
            {
                LoadingIndicator.IsActive = false;
            }
        }
    }
}
