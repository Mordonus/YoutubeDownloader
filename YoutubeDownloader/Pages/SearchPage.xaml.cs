﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace YoutubeDownloader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        private ObservableCollection<SearchItem> _searchItems;
        private string _nextPageToken;
        private string _prevPageToken;


        private void StartQuery(object sender, RoutedEventArgs e)
        {
            StartQuery(SearchQuery.Text);
        }

        public void StartRelatedQuery(string relatedId)
        {
            StartQuery("", "", relatedId);
        }

        private async void StartQuery(string query, string token = "", string relatedId = "")
        {
            SpinnerLoadingSearch.Visibility = Visibility.Visible;
            EmptyNotice.Visibility = Visibility.Collapsed;
            _searchItems = new ObservableCollection<SearchItem>();
            Dictionary<string, Dictionary<string, string>> videos = new Dictionary<string, Dictionary<string, string>>();
            string type = QueryType.Text.ToLower(),
                order = QueryOrder.Text.ToLower().Replace(" ", ""), //There's different thread down there...
                date = QueryDate.Text;
            await Task.Run(
                async () =>
                {
                    videos =
                        await
                            YTDownload.GetSearchResults(query, type, relatedId, token,
                                order.Replace(" ", ""), date);
                });     
            ProcessPageTokens(videos["tokens"]["prev"],videos["tokens"]["next"]);
            videos.Remove("tokens");
            foreach (var item in videos)
            {
                _searchItems.Add(new SearchItem(item.Key, item.Value));
            }
            VideoList.ItemsSource = _searchItems;
            SpinnerLoadingSearch.Visibility = Visibility.Collapsed;        
        }

        private void VideoItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (VideoList.SelectedItems.Count > 0)
            {
                SelectionMenu.Visibility = Visibility.Visible;
                Utils.GetMainPageInstance().AppBarOpened();
            }
            else
            {
                SelectionMenu.Visibility = Visibility.Collapsed;
                Utils.GetMainPageInstance().AppBarClosed();
            }
        }

        private void MassDownload(object sender, RoutedEventArgs e)
        {
            foreach (SearchItem item in VideoList.SelectedItems)
            {
                Utils.GetMainPageInstance().GetDownloaderPage().AddVideoItem(VideoItem.FromSerachItem(item));
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            StartQuery("", _nextPageToken);
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            StartQuery("", _prevPageToken);
        }

        private void ResetPageTokens()
        {
            _nextPageToken = "";
            _prevPageToken = "";
            Pages.Visibility = Visibility.Collapsed;
            SearchQuery.SetValue(Grid.ColumnSpanProperty, 2);
        }

        private void ProcessPageTokens(string tp, string tn) // previous and next
        {
            if (tp != null || tn != null)
            {
                Pages.Visibility = Visibility.Visible;
                SearchQuery.SetValue(Grid.ColumnSpanProperty, 1);
            }
            else
            {
                Pages.Visibility = Visibility.Collapsed;
                SearchQuery.SetValue(Grid.ColumnSpanProperty, 2);
            }

            if (tp != null)
            {
                _prevPageToken = tp;
                PrevPage.IsEnabled = true;
                SymbolIcon ico = PrevPage.Content as SymbolIcon;
                ico.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            }
            else
            {
                PrevPage.IsEnabled = false;
                SymbolIcon ico = PrevPage.Content as SymbolIcon;
                ico.Foreground = new SolidColorBrush(Colors.Black);
            }

            if (tn != null)
            {
                _nextPageToken = tn;
                NextPage.IsEnabled = true;
                SymbolIcon ico = NextPage.Content as SymbolIcon;
                ico.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            }
            else
            {
                NextPage.IsEnabled = false;
                SymbolIcon ico = NextPage.Content as SymbolIcon;
                ico.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        //private void Button_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Button btn = sender as Button;
        //    btn.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
        //}

        //private void Button_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Button btn = sender as Button;
        //    btn.Foreground = new SolidColorBrush(Colors.Black);
        //}

        private void MenuFlyoutDateClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as MenuFlyoutItem;
            QueryDate.Text = btn.Text;
        }

        private void MenuFlyoutOrderClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as MenuFlyoutItem;
            QueryOrder.Text = btn.Text;
        }

        private void MenuFlyoutTypeClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as MenuFlyoutItem;
            QueryType.Text = btn.Text;
        }
    }
}
