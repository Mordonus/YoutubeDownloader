﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace YoutubeDownloader
{
    public static class Utils
    {
        ///<summary>
        ///Removes all illegal chacters from filename so it can be saved.
        ///</summary>
        public static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        /// <summary>
        /// Tries to delete file n times , each try is postponed by 5 seconds.
        /// </summary>
        /// <param name="nRetries"></param>
        /// <param name="file"></param>
        public static void TryToRemoveFile(int nRetries,StorageFile file)
        {
            Task.Run( async () =>
            {
                try
                {
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch (Exception)
                {
                    if (nRetries > 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        TryToRemoveFile(nRetries - 1,file);
                    }            
                }
            });
        }

        //Snippet from https://github.com/flagbug/YoutubeExtractor
        public static async Task<string> TryNormalizeYoutubeUrl(string url)
        {
            url = url.Trim();

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube", "youtube");
            url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

            if (url.Contains("/v/"))
            {
                url = "http://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
            }

            url = url.Replace("/watch#", "/watch?");


            if (url.Contains("?"))
            {
                url = url.Substring(url.IndexOf('?') + 1);
            }

            var dictionary = Regex.Split(url, "&").Select(vp => Regex.Split(vp, "=")).ToDictionary(strings => strings[0], strings => strings.Length == 2 ? WebUtility.UrlDecode(strings[1]) : string.Empty);

            if (dictionary.ContainsKey("list") && dictionary.ContainsKey("v"))
            {
                MessageDialog md = new MessageDialog("There's both playlist and video id in provided string.\nWhich one would you like to be processed?");
                bool? result = null;
                md.Commands.Add(
                   new UICommand("Playlist", cmd => result = true));
                md.Commands.Add(
                   new UICommand("Video", cmd => result = false));

                await md.ShowAsync();
                if (result == true)
                    return dictionary["list"];
                return dictionary["v"];
            }

            if (dictionary.ContainsKey("list"))
                return dictionary["list"];
            if (dictionary.ContainsKey("v"))
                return dictionary["v"];

            return "";
        }
        /// <summary>
        /// Visible = true , Collapsed = false
        /// </summary>
        /// <param name="vis"></param>
        /// <returns></returns>
        public static bool VisibilityConverter(Visibility vis)
        {
            return vis == Visibility.Visible;
        }

        /// <summary>
        /// Use only from main thread
        /// </summary>
        /// <returns></returns>
        public static MainPage GetMainPageInstance()
        {
            var frame = (Frame)Window.Current.Content;
            return (MainPage)frame.Content;
        }

        public static async Task<StorageFile> SelectCoverFile()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            
            var result = await picker.PickSingleFileAsync();
            StorageApplicationPermissions.MostRecentlyUsedList.Add(result);
            return result;
        }

        internal static void DetailsPopulate(VideoItem videoItem)
        {
            var page = GetMainPageInstance();
            var dlPage = page.GetDownloaderPage();           
            dlPage.DetailsPopulate(videoItem);
        }

    }
}
