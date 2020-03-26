using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
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
    public sealed partial class PlayPage : Page
    {
        public static string pageData = null;
        public static List<string> ml = new List<string>();
        public static int playMode = 0;
        int musicPicHead = 0;
        int musicPicTail = 0;
        static MediaPlayer mp = new MediaPlayer();
        public static MediaPlaybackList mpl = new MediaPlaybackList();
        MatchCollection mctImg = null;
        static DispatcherTimer timer = new DispatcherTimer();
        static int fromVol = new int();
        static List<string> musicList = new List<string>();
        static List<string> titleList = new List<string>();
        static int selectedMusic = 0;
        public static void CreateMediaPlaybackList(List<string> musicList, int fromVol, int selectedMusic, List<string> titleList)
        {
            mpl.Items.Clear();
            PlayPage.fromVol = fromVol;
            PlayPage.musicList.Clear();
            for (int i = 0; i < musicList.Count; i++)
            {
                PlayPage.musicList.Add(musicList[i]);
                PlayPage.titleList.Add(titleList[i]);
            }
            PlayPage.selectedMusic = selectedMusic;
            for (int i = 0; i < musicList.Count; i++)
            {
                mpl.Items.Add(new MediaPlaybackItem(MediaSource.CreateFromUri(new Uri("http://mp3-cdn.luoo.net/low/luoo/radio" + fromVol + "/" + musicList[i] + ".mp3"))));
            }
            for (int i = 0; i < mpl.Items.Count; i++)
            {
                MediaItemDisplayProperties props = mpl.Items[i].GetDisplayProperties();
                props.Type = Windows.Media.MediaPlaybackType.Music;
                props.MusicProperties.Title = titleList[i];
                mpl.Items[i].ApplyDisplayProperties(props);
            }
            mpl.StartingItem = mpl.Items[selectedMusic];
            mpl.AutoRepeatEnabled = true;
            mp.AudioCategory = MediaPlayerAudioCategory.Media;
            mp.Source = mpl;
            mpl.ItemFailed += Mpl_ItemFailed;
            timer.Start();
        }

        private static void Mpl_ItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
        {
            mp.Pause();
            mpl.Items.Clear();
            for (int i = 0; i < musicList.Count; i++)
            {
                mpl.Items.Add(new MediaPlaybackItem(MediaSource.CreateFromUri(new Uri("http://mp3-cdn.luoo.net/low/luoo/radio" + fromVol + "/" + Int32.Parse(musicList[i]).ToString("0") + ".mp3"))));
            }
            for (int i = 0; i < mpl.Items.Count; i++)
            {
                MediaItemDisplayProperties props = mpl.Items[i].GetDisplayProperties();
                props.Type = Windows.Media.MediaPlaybackType.Music;
                props.MusicProperties.Title = titleList[i];
                mpl.Items[i].ApplyDisplayProperties(props);
            }
            mp.Play();
            //throw new NotImplementedException();
        }

        public static void Play()
        {
            //mpl.StartingItem = mpl.Items[selectedMusic];
            mp.Play();
        }

        public PlayPage()
        {
            this.InitializeComponent();
            //Regex imRegex = new Regex("<img src=\"http://img-cdn.luoo.net/pics/albums/\\d+/.+\\..+?imageView\\d+/\\d+/w/\\d+/h/\\d+\" alt=\".+\" class=\"cover rounded\">", RegexOptions.Compiled);
            Regex imRegex = new Regex("<img src=\"http://img-cdn2.luoo.net/pics/albums/.+..+!/fwfh/.+\" alt=\".+\" class=\"cover rounded\">", RegexOptions.Compiled);
            if (pageData != null)
            {
                mctImg = imRegex.Matches(pageData);
            }
            mpe.SetMediaPlayer(mp);
            mpe.AreTransportControlsEnabled = true;
            mpe.TransportControls.IsFullWindowButtonVisible = false;
            mpe.TransportControls.IsZoomButtonVisible = false;
            mpe.TransportControls.IsPreviousTrackButtonVisible = true;
            mpe.TransportControls.IsNextTrackButtonVisible = true;
            if (mpl.ShuffleEnabled)
            {
                button_ModeTwo.FontWeight = FontWeights.ExtraBlack;
                button_ModeTwo.Foreground = new SolidColorBrush(Colors.LawnGreen);
                button_ModeTwo.BorderBrush = new SolidColorBrush(Colors.LawnGreen);
            }
            if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(VolMainPage));
            }
            e.Handled = true;
            //throw new NotImplementedException();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (mctImg != null && (int)mpl.CurrentItemIndex != -1)
            {
                musicPicHead = mctImg[(int)mpl.CurrentItemIndex].ToString().IndexOf('\"');
                musicPicTail = mctImg[(int)mpl.CurrentItemIndex].ToString().Substring(musicPicHead + 1).IndexOf('\"') + musicPicHead + 1;
                im_Im.Source = new BitmapImage(new Uri(mctImg[(int)mpl.CurrentItemIndex].ToString().Substring(musicPicHead + 1, musicPicTail - musicPicHead - 1)));
            }
            if (mpl.CurrentItem != null)
            {
                tb_Title.Text = mpl.CurrentItem.GetDisplayProperties().MusicProperties.Title;
            }
        }

        private void button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(VolMainPage));
            }
        }

        private void button_ModeTwo_Click(object sender, RoutedEventArgs e)
        {
            if (mpl.ShuffleEnabled)
            {
                mpl.ShuffleEnabled = false;
                button_ModeTwo.FontWeight = FontWeights.Normal;
                button_ModeTwo.Foreground = new SolidColorBrush(Colors.Black);
                button_ModeTwo.BorderBrush = null;
            }
            else if (mpl.ShuffleEnabled == false)
            {
                mpl.ShuffleEnabled = true;
                button_ModeTwo.FontWeight = FontWeights.ExtraBlack;
                button_ModeTwo.Foreground = new SolidColorBrush(Colors.LawnGreen);
                button_ModeTwo.BorderBrush = new SolidColorBrush(Colors.LawnGreen);
            }
        }
    }
}