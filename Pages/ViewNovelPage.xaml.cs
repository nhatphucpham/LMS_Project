using LMS_Project.Data;
using LMS_Project.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        List<UIElement> ListElement = new List<UIElement>();

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
                {
                    MainFlip.Items.Add(new TextBlock()
                    {
                        Text = "Không có chương nào đã chọn",
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    });
                }
            }
        }
        List<string> TextContent = new List<string>();
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
                    if (NovelPage.model.GetChapter(chapter.ChapterId).Content == null)
                        TextContent = await NovelPage.model.LoadContent(chapter.ChapterId);

                    MainPage.CurrentChapter = NovelPage.model.GetChapter(chapter.ChapterId);
                    if(TextContent.Count == 0)
                        TextContent = HTMLAnalyze.ToCodeList(MainPage.CurrentChapter.Content);

                    if (SourceAnalysis.GetWebSourceOfChapter(chapter.ChapterId).Name == "Sublightnovel")
                        MainPage.cbTitle.SelectedIndex = 0;
                    else
                        MainPage.cbTitle.SelectedIndex = 1;
                    
                    MainPage.CurrentChapter = NovelPage.model.GetChapter(chapter.ChapterId);
                }
            }
            finally
            {
                ListElement = ConfigContent(TextContent, 18, "Default");
                StackPanel panel = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Vertical
                };
                foreach (var element in ListElement)
                {
                    if(panel.DesiredSize.Height >= MainFlip.ActualHeight)
                    {
                        MainFlip.Items.Add(panel);
                        Debug.WriteLine("Added:" + panel.ToString());
                        panel = new StackPanel
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Top,
                            Orientation = Orientation.Vertical
                        };
                    }
                    panel.Children.Add(element);
                    Debug.WriteLine("Added: " + element.ToString());
                    panel.Measure(new Size(MainFlip.ActualWidth, MainFlip.ActualWidth));
                }
                if(panel.Children.Count > 0)
                {
                    MainFlip.Items.Add(panel);
                }
                LoadingIndicator.IsActive = false;
            }
        }

        private List<UIElement> ConfigContent(List<string> content, int FontSize, string YourFontFamily)
        {
            var TextContent = HTMLAnalyze.ToTextList(content);
            var ItemList = new List<UIElement>();

            var blockContent = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Justify,
                TextWrapping = TextWrapping.Wrap,
                FontSize = FontSize,
                FontFamily = YourFontFamily.Equals("Default") ? new FontFamily("Calibri") : new FontFamily(YourFontFamily)
            };

            bool IsContent = false;
            bool IsTable = false;
            bool IsTableBody = false;
            bool IsRow = false;
            bool IsCell = false;
            
            var rowPanel = new Grid();

            var cellPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            foreach (string item in TextContent)
            {
                string NewValue = item.Trim();
                switch(NewValue)
                {
                    case "TABLE BEGIN":
                        if(!IsTable)
                        {
                            IsTable = true;
                        }
                        break;
                    case "TABLE END":
                        if (IsTable)
                        {
                            IsTable = false;
                        }
                        break;
                    case "TABLE BODY BEGIN":
                        if (IsTable)
                        {
                            IsTableBody = true;
                        }
                        break;
                    case "TABLE BODY END":
                        if (IsTableBody)
                        {
                            IsTableBody = false;
                        }
                        break;
                    case "TABLE ROW BEGIN":
                        if (IsTableBody)
                        {
                            if (!IsRow)
                            {
                                IsRow = true;
                                rowPanel = new Grid();
                            }
                         }
                        break;
                    case "TABLE ROW END":
                        if (IsRow)
                        {
                            IsRow = false;
                            ItemList.Add(rowPanel);
                        }
                        break;
                    case "TABLE CELL BEGIN":
                        if(IsRow)
                        {
                            if (!IsCell)
                            {
                                IsCell = true;
                                cellPanel = new StackPanel
                                {
                                    Orientation = Orientation.Vertical,
                                    VerticalAlignment = VerticalAlignment.Stretch
                                };
                            }
                        }
                        break;
                    case "TABLE CELL END":
                        if (IsCell)
                        {
                            IsCell = false;
                            var border = new Border
                            {
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                Background = Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                Child = cellPanel
                            };
                            rowPanel.Children.Add(border);
                            if (rowPanel.Children.Count > 0)
                            {
                                if (rowPanel.Children.Count > 1)
                                {
                                    for (int i = 0; i < rowPanel.Children.Count; i++)
                                    {
                                        ColumnDefinition col = new ColumnDefinition()
                                        {
                                            Width = new GridLength(230)
                                        };
                                        rowPanel.ColumnDefinitions.Add(col);
                                    }
                                }
                                Grid.SetColumn(border, rowPanel.Children.Count - 1);
                                Grid.SetColumnSpan(border, 1);
                                if(Grid.GetColumn(border) == 0)
                                {
                                    foreach(TextBlock Child in cellPanel.Children)
                                    {
                                        Child.Foreground = new SolidColorBrush(Colors.White);
                                    }
                                }
                            }
                        }
                        break;
                    case "CONTENT BEGIN":
                        IsContent = true;
                        blockContent = new TextBlock
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            TextAlignment = TextAlignment.Justify,
                            TextWrapping = TextWrapping.Wrap,
                            FontSize = FontSize,
                            FontFamily = YourFontFamily.Equals("Default") ? new FontFamily("Calibri") : new FontFamily(YourFontFamily),
                            Margin = new Thickness(7, 0, 7, 0)
                        };
                        
                        break;
                    case "CONTENT END":
                        if (IsContent)
                        {
                            IsContent = false;
                            if (!IsCell)
                            {
                                ItemList.Add(blockContent);
                            }
                            else
                            {
                                blockContent.TextAlignment = TextAlignment.Center;
                                cellPanel.Children.Add(blockContent);
                            }
                        }
                        break;
                    default:
                        if (IsContent)
                        {
                            if (blockContent.Text != null && blockContent.Text.Length > 1)
                            {
                                if (blockContent.Text.Last() == ' ' || NewValue == "" || NewValue.First() == ' ')
                                    blockContent.Text += NewValue;
                                else blockContent.Text += " " + NewValue;
                            }
                            else
                            {
                                if(NewValue == "" || NewValue.First() != ' ')
                                    blockContent.Text = NewValue;
                                else
                                    blockContent.Text = NewValue.Remove(0, 1);
                            }
                        }
                        else
                        {
                            if (NewValue != "" && NewValue != " ")
                            {
                                blockContent = new TextBlock
                                {
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    TextAlignment = TextAlignment.Justify,
                                    TextWrapping = TextWrapping.Wrap,
                                    FontSize = FontSize,
                                    FontFamily = YourFontFamily.Equals("Default") ? new FontFamily("Calibri") : new FontFamily(YourFontFamily),
                                    Margin = new Thickness(7, 0, 7, 0),
                                    Text = NewValue
                                };
                                ItemList.Add(blockContent);
                            }
                        }
                        break;
                }
               
            }
            return ItemList;
        }

        private void MainFlip_SizeChanged(object sender, SizeChangedEventArgs e)
        { 
            //if((MainFlip.Items[0] as StackPanel).ActualHeight >= e.NewSize.Height)
            //{
            //    for (int i = 0; i < MainFlip.Items.Count; i++)
            //    {
            //        var item = MainFlip.Items[i] as StackPanel;
            //        var LastElem = item.Children.Last();
            //        item.Children.RemoveAt(item.Children.Count - 1);
            //    }
            //}
        }
    }
}
