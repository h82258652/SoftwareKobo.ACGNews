using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.Social.Sina.Weibo;
using System;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Views
{
    public sealed partial class DetailView
    {
        private FeedBase _feed;

        private string _template;

        public bool IsFullScreen
        {
            get;
            private set;
        }

        public DetailView()
        {
            InitializeComponent();
        }

        public void Hide()
        {
            // 确保屏幕回到纵向。
            OnExitFullScreen();
            VisualStateManager.GoToState(this, "HideState", true);
        }

        public async Task ShowAsync(FeedBase feed, string detail, Point? showAnimateCenter = null)
        {
            // 将当前页面对应的 Feed 存储起来。
            _feed = feed;

            // 设置动画中心。
            if (showAnimateCenter.HasValue == false)
            {
                showAnimateCenter = new Point(0.5, 0.5);
            }
            ContentGrid.RenderTransformOrigin = showAnimateCenter.Value;

            TypedEventHandler<WebView, WebViewDOMContentLoadedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                WebView.DOMContentLoaded -= handler;
                VisualStateManager.GoToState(this, "ShowState", true);
            };
            WebView.DOMContentLoaded += handler;
            await SetContentAsync(detail);
        }

        private async void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            NotificationView.ShowLoading();

            #region 下载图片

            byte[] bytes;
            try
            {
                using (var client = new HttpClient())
                {
                    var buffer = await client.GetBufferAsync(new Uri(_feed.Thumbnail));
                    bytes = buffer.ToArray();
                }
            }
            catch
            {
                bytes = null;
            }

            #endregion 下载图片

            try
            {
                var client = await WeiboClient.CreateAsync();
                if (bytes != null)
                {
                    // Thumbnail download success, share image,text and link.
                    await client.ShareImageAsync(bytes, _feed.Title + Environment.NewLine + _feed.DetailLink);
                }
                else
                {
                    // Thumbnail download failed, only share text and link.
                    await client.ShareTextAsync(_feed.Title + Environment.NewLine + _feed.DetailLink);
                }

                NotificationView.ShowToastMessage("分享成功！");
            }
            catch
            {
                NotificationView.ShowToastMessage("抱歉，分享失败");
            }
            finally
            {
                NotificationView.HideLoading();
            }
        }

        public async Task ExitFullScreen()
        {
            await WebView.InvokeScriptAsync("eval", new[] { "document.exitFullscreen()" });
        }

        private void OnEnterFullScreen()
        {
            if (IsFullScreen == false)
            {
                IsFullScreen = true;
                // 转到横屏。
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape |
                                                             DisplayOrientations.LandscapeFlipped;
                AppBar.Visibility = Visibility.Collapsed;
                NotificationView.Instance.Visibility = Visibility.Collapsed;
            }
        }

        private void OnExitFullScreen()
        {
            if (IsFullScreen)
            {
                IsFullScreen = false;
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
                AppBar.Visibility = Visibility.Visible;
                NotificationView.Instance.Visibility = Visibility.Visible;
            }
        }

        private void HideStoryboard_Completed(object sender, object e)
        {
            // 后退时清空内容。这是为了防止隐藏时仍然播放视频产生声音。
            WebView.Navigate(new Uri("about:blank"));
        }

        private async Task SetContentAsync(string content)
        {
            // 加载模板。
            if (_template == null)
            {
                var templateFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Web/Views/app.html"));
                _template = await FileIO.ReadTextAsync(templateFile);
            }
            // 填充内容。
            var html = string.Format(_template, content);
            //WebView.NavigateToString(html);

            var uri = WebView.BuildLocalStreamUri("Cache", "/");
            WebView.NavigateToLocalStreamUri(uri, new Web.UriToStreamResolver(html));
        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var value = e.Value;
            switch (value)
            {
                case "goback":
                    if (AppSetting.Instance.NavigateBackBySlideToRight)
                    {
                        Hide();
                    }
                    break;

                case "enterFullScreen":
                    OnEnterFullScreen();
                    break;

                case "exitFullScreen":
                    OnExitFullScreen();
                    break;
            }
        }

        private async void BtnOpenInBrowser_Click(object sender, RoutedEventArgs e)
        {
            if (_feed != null)
            {
                await Launcher.LaunchUriAsync(new Uri(_feed.DetailLink));
            }
        }
    }
}