﻿using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Storage;
using YoutubeExtractor;
using Windows.UI.Popups;
using System.Linq;
using System.IO;
using System;
using Windows.Media.Transcoding;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace YoutubeDownloader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Settings.Init();
           // TagProcessing.Convert("test1.mp4");
           // TagProcessing.SetAlbumTag("yup.mp3", "lolme");
        }


        public ObservableCollection<VideoItem> vidListItems;
        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            VideoItem vidItem;
            SpinnerLoadingPlaylist.Visibility = Visibility.Visible;
            vidListItems = new ObservableCollection<VideoItem>();
            switch (YTDownload.IsIdValid(BoxID.Text))
            {
                case IdType.TYPE_VIDEO:
                    vidItem = new VideoItem(BoxID.Text);
                    vidListItems.Add(vidItem);
                    break;
                case IdType.TYPE_PLAYLIST:
                    List<string> videos = await YTDownload.GetVideosInPlaylist(BoxID.Text);
                    foreach (var video in videos)
                    {
                        vidItem = new VideoItem(video);
                        vidListItems.Add(vidItem);
                    }
                    break;
                case IdType.INVALID:
                    MessageDialog dialog = new MessageDialog("YouTube video got injured in an horrible accident, sorry it's ID is INVALID","God F&#$%*@ D%&#");
                    await dialog.ShowAsync();
                    break;
                default:
                    throw new Exception("Invalid enumm - id valid");
            }
            SpinnerLoadingPlaylist.Visibility = Visibility.Collapsed;
            VideoList.ItemsSource = vidListItems;
        }
        
        #region Setting Setters
        public void SetSetAlbumAsPlaylistNameSetting(string val)
        {
            SettingSetAlbumAsPlaylistName.IsOn = val == "True" ? true : false;
        }
        

        public void SetAutoDownloadSetting(string val)
        {
            SettingAutoDownload.IsOn = val == "True" ? true : false;
        }

        public void SetOutputFormat(int iFormat)
        {
            ComboOutputFormat.SelectedIndex = iFormat;
        }
        #endregion
       
        #region Settings Controls
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
        }

        private void ChangeSetting(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = (ToggleSwitch)sender;
            Settings.ChangeSetting(toggle.Name, Convert.ToString(toggle.IsOn));
        }


        #endregion

        private void ChangePrefferedFormat(object sender, object e)
        {
            ComboBox cmb = (ComboBox)sender;

            Settings.ChangeFormat((Settings.PossibleOutputFormats)cmb.SelectedIndex);
        }

    }
}
