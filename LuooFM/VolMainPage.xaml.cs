using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace LuooFM
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VolMainPage : Page
    {
        public static int pageNumber = 1;
        int volsCount = 0;
        static ObservableCollection<RelativePanel> oc = new ObservableCollection<RelativePanel>();
        static bool first = true;
        public VolMainPage()
        {
            this.InitializeComponent();
            gv.ItemsSource = oc;
            //GetVols(pageNumber);
            if (first)
            {
                this.GetVols(1);
                first = false;
            }
        }

        void GetVols(int pageNumb)
        {
            try
            {
                string url = "http://www.luoo.net/tag/?p=" + pageNumb;
                HttpClient volHttpCLient = new HttpClient();
                StringBuilder volPageData = new StringBuilder(volHttpCLient.GetStringAsync(url).Result.ToString());
                //Regex volPicRegex = new Regex("<img src=\"http://img-cdn.luoo.net/pics/vol/.+..+\\?imageView\\d+/\\d+/w/\\d+/h/\\d+\" alt=\".+\" class=\"cover rounded\">", RegexOptions.Compiled);
                Regex volPicRegex = new Regex("<img src=\"http://img-cdn2.luoo.net/pics/vol/.+..+!/fwfh/.+\" alt=\".+\" class=\"cover rounded\">", RegexOptions.Compiled);
                MatchCollection mctPic = volPicRegex.Matches(volPageData.ToString());
                Regex volRegex = new Regex("<a href=\"http://www.luoo.net/vol/index/\\d+\" class=\"name\" title=\".+\">vol\\.\\d+\\s.+</a>", RegexOptions.Compiled);
                MatchCollection mct = volRegex.Matches(volPageData.ToString());
                if (pageNumber == 1 && volsCount == 0)
                {
                    volsCount = Int32.Parse(mct[0].ToString().Substring(mct[0].ToString().IndexOf('>') + 1, mct[0].ToString().LastIndexOf('<') - mct[0].ToString().IndexOf('>') - 1).Split(' ')[0].Split('.')[1]);
                }
                int volPicHead = 0;
                int volPicTail = 0;
                int volHead = 0;
                int volTail = 0;
                for (int i = 0; i < mct.Count; i++)
                {
                    volPicHead = mctPic[i].ToString().IndexOf('\"');
                    volPicTail = mctPic[i].ToString().Substring(volPicHead + 1).IndexOf('\"') + volPicHead + 1;
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(mctPic[(int)i].ToString().Substring(volPicHead + 1, volPicTail - volPicHead - 1)));
                    volHead = mct[i].ToString().IndexOf('>');
                    volTail = mct[i].ToString().LastIndexOf('<');
                    TextBlock tb = new TextBlock();
                    tb.Text = mct[i].ToString().Substring(volHead + 1, volTail - volHead - 1);
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    tb.FontSize = 24;
                    tb.TextWrapping = TextWrapping.Wrap;
                    Rectangle rec = new Rectangle();
                    rec.Fill = new SolidColorBrush(Colors.Black);
                    rec.Opacity = 0.5;
                    RelativePanel rp = new RelativePanel();
                    rp.Children.Add(img);
                    rp.Children.Add(rec);
                    rp.Children.Add(tb);
                    tb.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignTopWithProperty, tb);
                    rec.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignRightWithPanelProperty, true);
                    oc.Add(rp);
                }
                pageNumber = pageNumb;
            }
            catch (Exception ex)
            {
                TextBox tb = new TextBox();
                tb.Text = ex.ToString();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.IsReadOnly = true;
                //tb.SelectedText = tb.Text;
                tb.SelectAll();
                var dialog = new ContentDialog()
                {
                    Title = "很抱歉，落.FM Lite在获取期刊列表时出现了异常：",
                    Content = tb,
                    PrimaryButtonText = "退出",
                    //SecondaryButtonText = "取消",
                    FullSizeDesired = true,
                };
                dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
                dialog.ShowAsync();
            }
        }

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            App.Current.Exit();
            //throw new NotImplementedException();
        }

        private void button_PlayPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PlayPage));
        }

        private void button_Go_Click(object sender, RoutedEventArgs e)
        {
            Regex numbRegex = new Regex("^\\d{1,}$", RegexOptions.Compiled);
            Match match = numbRegex.Match(tbox_Go.Text);
            if (match.Length == 0)
            {
                tbox_Go.Text = "";
            }
            else
            {
                try
                {
                    if (Int32.Parse(tbox_Go.Text) <= volsCount && tbox_Go.Text != "0")
                    {
                        VolPage.vol = tbox_Go.Text;
                        Frame.Navigate(typeof(VolPage));
                    }
                    else
                    {
                        tbox_Go.Text = "";
                    }
                }
                catch (Exception)
                {
                    tbox_Go.Text = "";
                    //throw;
                }
            }
        }

        private void button_Load_Click(object sender, RoutedEventArgs e)
        {
            GetVols(pageNumber + 1);
        }

        private void gv_ItemClick(object sender, ItemClickEventArgs e)
        {
            RelativePanel rp = (RelativePanel)e.ClickedItem;
            TextBlock tb = (TextBlock)rp.Children[2];
            VolPage.vol = tb.Text.Split(' ')[0].Split('.')[1];
            Frame.Navigate(typeof(VolPage));
        }
    }
}
