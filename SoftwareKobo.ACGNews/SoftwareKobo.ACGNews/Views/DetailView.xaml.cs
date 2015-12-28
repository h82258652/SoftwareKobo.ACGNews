using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Threading.Tasks;
using UmengSocialSDK;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Views
{
    public sealed partial class DetailView
    {
        private FeedBase _feed;

        private bool _isFullScreen;

        private string _template;

        public DetailView()
        {
            InitializeComponent();
        }

        public void Hide()
        {
            // 确保屏幕回到纵向。
            ExitFullScreen();
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
            // TODO
            UmengLink link = new UmengLink();
            link.Url = _feed.DetailLink;
            link.Title = _feed.Title;

            MultiClient client = new MultiClient(null);
            var result = await client.ShareLinkAsync(new UmengLink(""));
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var h = await WebView.InvokeScriptAsync("eval", new[] { "document.getElementsByTagName('html')[0].outerHTML" });
            await new MessageDialog(h).ShowAsync();
        }

        private void EnterFullScreen()
        {
            if (_isFullScreen == false)
            {
                _isFullScreen = true;
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape |
                                                             DisplayOrientations.LandscapeFlipped;
                AppBar.Visibility = Visibility.Collapsed;
            }
        }

        private void ExitFullScreen()
        {
            if (_isFullScreen)
            {
                _isFullScreen = false;
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
                AppBar.Visibility = Visibility.Visible;
            }
        }

        private void HideStoryboard_Completed(object sender, object e)
        {
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
            WebView.NavigateToString(html);
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
                    EnterFullScreen();
                    break;

                case "exitFullScreen":
                    ExitFullScreen();
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