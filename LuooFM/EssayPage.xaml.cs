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
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace LuooFM
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EssayPage : Page
    {
        string html = null;
        public static string essay = null;
        public EssayPage()
        {
            this.InitializeComponent();
            try
            {
                string url = "http://www.luoo.net/essay/" + essay;
                HttpClient essayHttpCLient = new HttpClient();
                StringBuilder essayPageData = new StringBuilder(essayHttpCLient.GetStringAsync(url).Result.ToString());
                Regex essayRegex = new Regex("<div class=\"essay-content\">[\\s\\S]*?</div>", RegexOptions.Compiled);
                Match essayM = essayRegex.Match(essayPageData.ToString());
                html = essayM.ToString();
                essay_WebView.NavigateToString(html);
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame.Navigate(typeof(EssayMainPage));
            e.Handled = true;
        }

        private void btn_List_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EssayMainPage));
        }
    }
}
