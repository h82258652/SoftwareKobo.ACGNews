using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Threading.Tasks;
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
            ExitFullScreen();
            VisualStateManager.GoToState(this, "HideState", true);
        }

        public async Task ShowAsync(FeedBase feed, string detail, Point? showAnimateCenter = null)
        {
            _feed = feed;
            if (showAnimateCenter.HasValue == false)
            {
                showAnimateCenter = new Point(0.5, 0.5);
            }
            ContentGrid.RenderTransformOrigin = showAnimateCenter.Value;

            await SetContentAsync(detail);

            VisualStateManager.GoToState(this, "ShowState", true);
        }

        private void BtnShare_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var h = await WebView.InvokeScriptAsync("eval", new string[] { "document.getElementsByTagName('html')[0].outerHTML" });
            await new MessageDialog(h).ShowAsync();
        }

        private async void ButtonBase1_OnClick(object sender, RoutedEventArgs e)
        {
            if (_feed != null)
            {
                await Launcher.LaunchUriAsync(new Uri(_feed.DetailLink));
            }
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
            if (_template == null)
            {
                var templateFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Web/Views/app.html"));
                _template = await FileIO.ReadTextAsync(templateFile);
            }
            var html = string.Format(_template, content);
            WebView.NavigateToString(html);
        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var value = e.Value;
            switch (value)
            {
                case "goback":
                    if (AppSetting.NavigateBackBySlideToRight)
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
    }
}