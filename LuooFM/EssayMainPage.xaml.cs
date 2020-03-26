using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class EssayMainPage : Page
    {
        public static int pageNumber = 1;
        //int essaysCount = 0;
        static ObservableCollection<RelativePanel> oc = new ObservableCollection<RelativePanel>();
        static bool first = true;
        public EssayMainPage()
        {
            this.InitializeComponent();
            gv.ItemsSource = oc;
            //GetEssays(pageNumber);
            if (first)
            {
                this.GetEssays(1);
                first = false;
            }
        }

        void GetEssays(int pageNumb)
        {
            //try
            //{
                string url = "http://www.luoo.net/essay/index/p/" + pageNumb;
                HttpClient essayHttpCLient = new HttpClient();
                StringBuilder essayPageData = new StringBuilder(essayHttpCLient.GetStringAsync(url).Result.ToString());
                Regex essayIDRegex = new Regex("<a href=\"http://www.luoo.net/essay/\\d+\" class=\"cover-wrapper\">", RegexOptions.Compiled);
                MatchCollection MctID = essayIDRegex.Matches(essayPageData.ToString());
                Regex essayPicRegex = new Regex("<img class=\"cover rounded\" src=\"http://img-cdn2.luoo.net/(pics/essays|library)/\\d+/.+..+\\?imageView\\d+/\\d+/w/\\d+/h/\\d+\" alt=\".+\">", RegexOptions.Compiled);
                MatchCollection mctPic = essayPicRegex.Matches(essayPageData.ToString());
                Regex essayRegex = new Regex("<div class=\"subscribe\">[\\s\\S]*?</div>", RegexOptions.Compiled);
                MatchCollection mct = essayRegex.Matches(essayPageData.ToString());
                int essayPicHead = 0;
                int essayPicTail = 0;
                int essayHead = 0;
                int essayTail = 0;
                int essaySubHead = 0;
                int essaySubTail = 0;
                int essayIDHead = 0;
                int essayIDTail = 0;
                if (pageNumb==1)
                {
                    Regex bannerEssayIDRegex = new Regex("<a href=\"http://www.luoo.net/essay/\\d+\" class=\"title\">", RegexOptions.Compiled);
                    Match bannerMID = bannerEssayIDRegex.Match(essayPageData.ToString());
                    Regex bannerEssayPicRegex = new Regex("<img src=\"http://img-cdn2.luoo.net/(pics/essays|library)/\\d+/.+..+\\?imageView\\d+/\\d+/w/\\d+/h/\\d+\" alt=\".+\" class=\"cover\">", RegexOptions.Compiled);
                    Match bannerMPic = bannerEssayPicRegex.Match(essayPageData.ToString());
                    Regex bannerEssayRegex = new Regex("<p class=\"content\">[\\s\\S]*?</p>", RegexOptions.Compiled);
                    Match bannerM = bannerEssayRegex.Match(essayPageData.ToString());
                    int bannerEssayPicHead = 0;
                    int bannerEssayPicTail = 0;
                    int bannerEssayHead = 0;
                    int bannerEssayTail = 0;
                    int bannerEssaySubHead = 0;
                    int bannerEssaySubTail = 0;
                    int bannerEssayIDHead = 0;
                    int bannerEssayIDTail = 0;
                    bannerEssayPicHead = bannerMPic.ToString().IndexOf("src=\"");
                    bannerEssayPicTail = bannerMPic.ToString().IndexOf("\" alt");
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(bannerMPic.ToString().Substring(bannerEssayPicHead + 5, bannerEssayPicTail - bannerEssayPicHead - 5)));
                    bannerEssayHead = bannerMPic.ToString().IndexOf("alt=\"");
                    bannerEssayTail = bannerMPic.ToString().IndexOf("\" cla");
                    TextBlock tb = new TextBlock();
                    tb.Text = bannerMPic.ToString().Substring(bannerEssayHead + 5, bannerEssayTail - bannerEssayHead - 5);
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    tb.FontSize = 24;
                    tb.TextWrapping = TextWrapping.Wrap;
                    bannerEssaySubHead = bannerM.ToString().IndexOf('>');
                    bannerEssaySubTail = bannerM.ToString().LastIndexOf('<');
                    TextBlock tb2 = new TextBlock();
                    tb2.Text = bannerM.ToString().Substring(bannerEssaySubHead + 1, bannerEssaySubTail - bannerEssaySubHead - 1).Trim();
                    tb2.Foreground = new SolidColorBrush(Colors.White);
                    tb2.TextWrapping = TextWrapping.Wrap;
                    bannerEssayIDHead = bannerMID.ToString().IndexOf("essay/");
                    bannerEssayIDTail = bannerMID.ToString().IndexOf("\" clas");
                    TextBlock tb3 = new TextBlock();
                    tb3.Text = bannerMID.ToString().Substring(bannerEssayIDHead + 6, bannerEssayIDTail - bannerEssayIDHead - 6);
                    tb3.Foreground = new SolidColorBrush(Colors.White);
                    tb3.FontSize = 12;
                    tb3.TextWrapping = TextWrapping.Wrap;
                    Rectangle rec = new Rectangle();
                    rec.Fill = new SolidColorBrush(Colors.Black);
                    rec.Opacity = 0.5;
                    RelativePanel rp = new RelativePanel();
                    rp.Children.Add(img);
                    rp.Children.Add(rec);
                    rp.Children.Add(tb);
                    rp.Children.Add(tb2);
                    rp.Children.Add(tb3);
                    tb.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignRightWithPanelProperty, true);
                    tb2.SetValue(RelativePanel.BelowProperty, tb);
                    tb3.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
                    tb3.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                    oc.Add(rp);
                }
                for (int i = 0; i < mct.Count; i++)
                {
                    essayPicHead = mctPic[i].ToString().IndexOf("src=\"");
                    essayPicTail = mctPic[i].ToString().IndexOf("\" alt");
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(mctPic[(int)i].ToString().Substring(essayPicHead +5, essayPicTail - essayPicHead - 5)));
                    essayHead = mctPic[i].ToString().LastIndexOf("=\"");
                    essayTail = mctPic[i].ToString().IndexOf("\">");
                    TextBlock tb = new TextBlock();
                    tb.Text = mctPic[i].ToString().Substring(essayHead + 2, essayTail - essayHead - 2);
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    tb.FontSize = 24;
                    tb.TextWrapping = TextWrapping.Wrap;
                    essaySubHead = mct[i].ToString().IndexOf('>');
                    essaySubTail = mct[i].ToString().LastIndexOf('<');
                    TextBlock tb2 = new TextBlock();
                    tb2.Text = mct[i].ToString().Substring(essaySubHead + 1, essaySubTail - essaySubHead - 1).Trim();
                    tb2.Foreground = new SolidColorBrush(Colors.White);
                    tb2.TextWrapping = TextWrapping.Wrap;
                    TextBlock tb3 = new TextBlock();
                    essayIDHead = MctID[i].ToString().IndexOf("essay/");
                    essayIDTail = MctID[i].ToString().IndexOf("\" clas");
                    tb3.Text = MctID[i].ToString().Substring(essayIDHead + 6, essayIDTail - essayIDHead - 6);
                    tb3.Foreground = new SolidColorBrush(Colors.White);
                    tb3.FontSize = 12;
                    tb3.TextWrapping = TextWrapping.Wrap;
                    Rectangle rec = new Rectangle();
                    rec.Fill = new SolidColorBrush(Colors.Black);
                    rec.Opacity = 0.5;
                    RelativePanel rp = new RelativePanel();
                    rp.Children.Add(img);
                    rp.Children.Add(rec);
                    rp.Children.Add(tb);
                    rp.Children.Add(tb2);
                    rp.Children.Add(tb3);
                    tb.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignTopWithPanelProperty,true);
                    rec.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                    rec.SetValue(RelativePanel.AlignRightWithPanelProperty, true);
                    tb2.SetValue(RelativePanel.BelowProperty,tb);
                    tb3.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                    tb3.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                    oc.Add(rp);
                }
                pageNumber = pageNumb;
            //}
            //catch (Exception ex)
            //{
            //    //throw;
            //    TextBox tb = new TextBox();
            //    tb.Text = ex.ToString();
            //    tb.TextWrapping = TextWrapping.Wrap;
            //    tb.IsReadOnly = true;
            //    //tb.SelectedText = tb.Text;
            //    tb.SelectAll();
            //    var dialog = new ContentDialog()
            //    {
            //        Title = "很抱歉，落.FM Lite在获取专栏列表时出现了异常：",
            //        Content = tb,
            //        PrimaryButtonText = "退出",
            //        //SecondaryButtonText = "取消",
            //        FullSizeDesired = true,
            //    };
            //    dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
            //    dialog.ShowAsync();
            //}
        }

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //throw new NotImplementedException();
            App.Current.Exit();
        }

        private void button_Load_Click(object sender, RoutedEventArgs e)
        {
            GetEssays(pageNumber + 1);
        }

        private void gv_ItemClick(object sender, ItemClickEventArgs e)
        {
            RelativePanel rp = (RelativePanel)e.ClickedItem;
            TextBlock tb = (TextBlock)rp.Children[4];
            EssayPage.essay = tb.Text;
            //System.Diagnostics.Debug.WriteLine(essay_Str);
            Frame.Navigate(typeof(EssayPage));
        }
    }
}
