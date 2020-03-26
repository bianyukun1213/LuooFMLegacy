using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LuooFM
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        bool _isInBackgroundMode = false;
        int volInt = new int();
        bool canJump = false;
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            _isInBackgroundMode = false;
            //throw new NotImplementedException();
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            _isInBackgroundMode = true;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>

        private async Task InsertVoiceCommands()
        {
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(
                await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///CortanaVoiceCommands.xml")));
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    this.DebugSettings.EnableFrameRateCounter = true;
            //}
#endif
            if (DateTime.Now.ToString().Split(' ')[0] == "2017/5/1")
            {
                MessageDialog md = new MessageDialog("今天是五一劳动节！", "劳动节：");
                md.Commands.Add(new UICommand("我放假了！"));
                md.Commands.Add(new UICommand("我没放假……"));
                md.Commands.Add(new UICommand("关你屁事！"));
                md.ShowAsync();
            }
            else if (DateTime.Now.ToString().Split(' ')[0] == "2017/5/30")
            {
                MessageDialog md = new MessageDialog("……我也不知道说什么了，太水了……", "端午：");
                md.Commands.Add(new UICommand("我吃甜粽子！"));
                md.Commands.Add(new UICommand("我吃咸粽子！"));
                md.Commands.Add(new UICommand("不用你提醒我！"));
                md.ShowAsync();
            }
            InsertVoiceCommands();
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (args.Kind != ActivationKind.VoiceCommand) return;
            var vcargs = (VoiceCommandActivatedEventArgs)args;
            var res = vcargs.Result;
            var cmdName = res.RulePath[0];
            string propertie = null;
            switch (cmdName)
            {
                case "PlayVol":
                    MessageDialog md = new MessageDialog("本版暂时停止对Cortana语音命令的支持，请见谅！","暂停命令支持：");
                    md.ShowAsync();
                    //propertie = res.SemanticInterpretation.Properties["Vol"][0];
                    //int volsCount = GetVolsCount();
                    //if (Int32.TryParse(propertie, out volInt) && volInt != 0 && volInt<=volsCount)
                    //{
                    //    VolPage.vol = volInt.ToString();
                    //    //VolPage.fromPage = 1;
                    //    canJump = true;
                    //}
                    //else
                    //{
                    //    VolPage.vol = "0";
                    //    //VolPage.fromPage = 0;
                    //    canJump = false;
                    //}
                    break;
            }
            var root = Window.Current.Content as Frame;
            if (root == null)
            {
                root = new Frame();
                Window.Current.Content = root;
            }
            //if (canJump)
            //{
            //    root.Navigate(typeof(VolPage));
            //    List<string> createList = new List<string>();
            //    for (int i = 0; i < VolPage.musicList.Count; i++)
            //    {
            //        createList.Add(VolPage.musicList[i].Split(' ')[0].Remove(VolPage.musicList[i].Split(' ')[0].Length - 1, 1));
            //    }
            //    PlayPage.pageData = VolPage.pageData;
            //    PlayPage.CreateMediaPlaybackList(createList, Int32.Parse(VolPage.vol), 0, VolPage.musicList);
            //    PlayPage.Play();
            //    root.Navigate(typeof(PlayPage));
            //}
            //else
            //{
            //    root.Navigate(typeof(MainPage));
            //}
            root.Navigate(typeof(MainPage));//恢复功能后记得删除这句。
            Window.Current.Activate();
        }

        int GetVolsCount()
        {
            try
            {
                string url = "http://www.luoo.net/";
                HttpClient volsCountHttpCLient = new HttpClient();
                string pageData = volsCountHttpCLient.GetStringAsync(url).Result.ToString();
                Regex volsCountRegex = new Regex("<a href=\"http://www.luoo.net/vol/index/\\d+\"\\sclass=\"cover-wrapper cover-wrapper-lg\">", RegexOptions.Compiled);
                Match mt = volsCountRegex.Match(pageData);
                string tempStr1 = mt.ToString().Split(' ')[1];
                string tempStr2 = tempStr1.Substring(tempStr1.LastIndexOf('/') + 1);
                return Int32.Parse(tempStr2.Remove(tempStr2.Count() - 1));
            }
            catch (Exception)
            {
                return 0;
                //throw;
            }
        }
    }
}
