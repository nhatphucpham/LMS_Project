using LMS_Project.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Setting : Page
    {
        public Setting()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
            string theme = App.Current.RequestedTheme.ToString();
            DarkModeToggle.Toggled -= DarkModeToggle_Toggled;
            if (localSettings.Values["isDarkMode"].Equals("true"))
            {
                DarkModeToggle.IsOn = true;
            }
            else
            {
                DarkModeToggle.IsOn = false;
            }
            DarkModeToggle.Toggled += DarkModeToggle_Toggled;
        }

        private async void  DarkModeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (DarkModeToggle.IsOn == true)
                localSettings.Values["isDarkMode"] = "true";
            else
                localSettings.Values["isDarkMode"] = "false";
            var showDialog = new MessageDialog("Application restart is needed to apply this setting");
            showDialog.Commands.Add(new UICommand("OK") { Id = 0 });
            showDialog.DefaultCommandIndex = 0;
            showDialog.CancelCommandIndex = 1;
            await showDialog.ShowAsync();
        }
    }
}
