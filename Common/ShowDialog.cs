using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using LMS_Project.Pages;
using Windows.UI.Xaml.Controls;

namespace LMS_Project.Common
{
        class ShowDialog
        {

            private static ShowDialog instance;
            private static ContentDialog contentDialog;
            private static Waiting waiting;
            private static IAsyncOperation<ContentDialogResult> result;


            private ShowDialog()
            {

            }

            public static ShowDialog getInstance()
            {
                if (instance == null)
                {
                    instance = new ShowDialog();
                }
                if (contentDialog == null)
                {
                    contentDialog = new ContentDialog();
                }
                if (waiting == null)
                {
                    waiting = new Waiting();
                }

                return instance;
            }
            public async Task<ButtonDialog> ShowContentDialog(string title, string content = "", string primariButton = "OK")
            {

                try
                {
                    HideWaiting();
                    HideContentDialog();

                    contentDialog.Title = title;
                    contentDialog.Content = content;
                    contentDialog.VerticalContentAlignment = VerticalAlignment.Center;
                    contentDialog.HorizontalContentAlignment = HorizontalAlignment.Center;
                    contentDialog.PrimaryButtonText = primariButton;
                    contentDialog.RequestedTheme = ElementTheme.Dark;

                    await contentDialog.ShowAsync();
                    return ButtonDialog.Primari;
                }
                catch (Exception ex)
                {

                    return ButtonDialog.Error;
                }

            }
            public async Task<ButtonDialog> ShowContentDialog(string title, string content, string primariButton, Boolean isHasSecondButton = false, string secondButton = "", ElementTheme themeRequest = ElementTheme.Dark)
            {
                try
                {
                    HideWaiting();
                    HideContentDialog();

                    contentDialog.Title = title;
                    contentDialog.Content = content;
                    contentDialog.VerticalContentAlignment = VerticalAlignment.Center;
                    contentDialog.HorizontalContentAlignment = HorizontalAlignment.Center;
                    contentDialog.PrimaryButtonText = primariButton;
                    contentDialog.RequestedTheme = themeRequest;
                    contentDialog.IsSecondaryButtonEnabled = isHasSecondButton;
                    contentDialog.SecondaryButtonText = secondButton;

                    var dialog = await contentDialog.ShowAsync();
                    if (dialog == ContentDialogResult.Primary)
                    {
                        return ButtonDialog.Primari;
                    }
                    else if (dialog == ContentDialogResult.Secondary)
                    {
                        return ButtonDialog.Second;
                    }
                    else
                    {
                        return ButtonDialog.Error;
                    }

                }
                catch (Exception ex)
                {

                    return ButtonDialog.Error;
                }
            }


            public async Task ShowWaiting(string title = "waiting", int max = 100, int value = 0, bool showProgress = false)
            {
                try
                {
                    if (waiting.ActualWidth > 0)
                    {
                        waiting.Title = title;
                        waiting.value = value;
                        waiting.max = max;
                        waiting.showPregress = showProgress;
                    }
                    else
                    {
                        HideWaiting();
                        contentDialog.Hide();

                        waiting.Title = title;
                        waiting.value = value;
                        waiting.max = max;
                        waiting.showPregress = showProgress;
                        var t = await waiting.ShowAsync();
                    }
                }
                catch (Exception ex)
                {

                }
            }

            public void HideWaiting()
            {
                try
                {
                    waiting.Hide();
                    waiting = new Waiting();
                }
                catch (Exception ex)
                {

                }
            }
            public void HideContentDialog()
            {
                try
                {
                    contentDialog.Hide();
                    contentDialog = new ContentDialog();
                }
                catch (Exception ex)
                {

                }
            }

        }
        public enum ButtonDialog
        {
            Error = 0,
            Primari = 1,
            Second = 2,

        }
    }
