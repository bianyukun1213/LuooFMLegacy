using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace LuooFM
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VolPage : Page
    {
        public static string vol = null;
        //public static int fromPage = 1;
        public static string pageData = null;
        public static List<string> musicList = new List<string>();
        public VolPage()
        {
            this.InitializeComponent();
            musicList.Clear();
            //try
            //{
                string url = "http://www.luoo.net/music/" + vol;
                HttpClient volHttpCLient = new HttpClient();
                StringBuilder volPageData = new StringBuilder(volHttpCLient.GetStringAsync(url).Result.ToString());
                pageData = volPageData.ToString();
                Regex volTitleRegex = new Regex("<span class=\"vol-title\">.+</span>", RegexOptions.Compiled);
                Match mt = volTitleRegex.Match(volPageData.ToString());
                int volTitleHead = mt.ToString().IndexOf('>');
                int volTitleTail = mt.ToString().LastIndexOf('<');
                textBlock_Title.Text = mt.ToString().Substring(volTitleHead + 1, volTitleTail - volTitleHead - 1);
            //Regex volPicRegex = new Regex("<img src=\"http://img-cdn.luoo.net/pics/vol/\\w+\\..+\\?imageView\\d+/\\d+/w/\\d+/h/\\d+\" alt=\".+\" class=\"vol-cover\">", RegexOptions.Compiled);
            Regex volPicRegex = new Regex("<img src=\"http://img-cdn2.luoo.net/pics/vol/.+..+!/fwfh/.+\" alt=\".+\" class=\"vol-cover\">", RegexOptions.Compiled);
            Match mcPic = volPicRegex.Match(volPageData.ToString());
                int volPicHead = mcPic.ToString().IndexOf('\"');
                int volPicTail = mcPic.ToString().Substring(volPicHead + 1).IndexOf('\"') + volPicHead + 1;
                image_Vol.Source = new BitmapImage(new Uri(mcPic.ToString().Substring(volPicHead + 1, volPicTail - volPicHead - 1)));
                Regex volContentRegex = new Regex("<div class=\"vol-desc\">[\\w\\W]*?</div>", RegexOptions.Compiled);
                Match mt2 = volContentRegex.Match(volPageData.ToString());
                int volContentHead = mt2.ToString().IndexOf('>');
                int volContentTail = mt2.ToString().LastIndexOf('<');
                textBlock_Content.Text = mt2.ToString().Substring(volContentHead + 1, volContentTail - volContentHead - 1).Trim().Replace("<br>", "");
                Regex volMusicRegex = new Regex("<a href=\"javascript:;\" rel=\"nofollow\" class=\"trackname btn-play\">\\d+\\. .+</a>", RegexOptions.Compiled);
                MatchCollection mctMusic = volMusicRegex.Matches(volPageData.ToString());
                for (int i = 0; i < mctMusic.Count; i++)
                {
                    this.grid_Music.RowDefinitions.Add(new RowDefinition());
                    Button b = new Button();
                    b.Name = "button_" + i;
                    int volMusicHead = mctMusic[i].ToString().IndexOf('>');
                    int volMusicTail = mctMusic[i].ToString().LastIndexOf('<');
                    musicList.Add(mctMusic[(int)i].ToString().Substring(volMusicHead + 1, volMusicTail - volMusicHead - 1));
                    b.Content = mctMusic[(int)i].ToString().Substring(volMusicHead + 1, volMusicTail - volMusicHead - 1);
                    b.HorizontalAlignment = HorizontalAlignment.Stretch;
                    b.VerticalAlignment = VerticalAlignment.Center;
                    b.Click += new RoutedEventHandler(b_Click);
                    grid_Music.Children.Add(b);
                    b.SetValue(Grid.RowProperty, i);
                }
                if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame.Navigate(typeof(VolMainPage));
            e.Handled = true;
        }

        private void b_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)(sender);
            List<string> createList = new List<string>();
            for (int i = 0; i < musicList.Count; i++)
            {
                createList.Add(musicList[i].Split(' ')[0].Remove(musicList[i].Split(' ')[0].Length-1,1));
            }
            PlayPage.pageData = pageData;
            PlayPage.CreateMediaPlaybackList(createList,Int32.Parse(vol),Int32.Parse(btn.Content.ToString().Split(' ')[0].Replace(".",""))-1,musicList);
            PlayPage.Play();
            Frame.Navigate(typeof(PlayPage));
        }

        private void button_List_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VolMainPage));
        }
    }
}
