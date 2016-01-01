using SoftwareKobo.ACGNews.Controls;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using SoftwareKobo.ACGNews.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using UmengSDK;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Web;
using WinRTXamlToolkit.Controls.Extensions;

namespace SoftwareKobo.ACGNews.Views
{
    public sealed partial class IndexView
    {
        public IndexView()
        {
            InitializeComponent();
        }

        public IndexViewModel ViewModel => (IndexViewModel)DataContext;

        private async void NewsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var feed = (FeedBase)e.ClickedItem;
            if (feed == null)
            {
                return;
            }

            var container = NewsList.ContainerFromItem(feed);
            var image = container.GetFirstDescendantOfType<Image>();
            Point detailRenderTransformOrigin;
            if (image != null)
            {
                var position = image.TransformToVisual(null).TransformPoint(new Point());
                var center = new Point(position.X + image.ActualWidth / 2, position.Y + image.ActualHeight / 2);
                detailRenderTransformOrigin = new Point(center.X / ActualWidth, center.Y / ActualHeight);
            }
            else
            {
                detailRenderTransformOrigin = new Point(0.5, 0.5);
            }

            var service = Service.GetService(feed);
            NotificationView.ShowLoading("正在加载" + feed.Title);
            try
            {
                var detail = await service.DetailAsync(feed);
                await AppView.Instance.NavigateToDetail(feed, detail, detailRenderTransformOrigin);
                await feed.MarkAsReadedAsync();
            }
            catch (Exception ex)
            {
                var error = WebError.GetStatus(ex.HResult);
                if (error == WebErrorStatus.Unknown)
                {
                    await UmengAnalytics.TrackException(ex);

                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                }

                NotificationView.ShowToastMessage("加载失败，请检查网络连接");
            }
            finally
            {
                NotificationView.HideLoading();
            }
        }

        private void NewsList_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = NewsList.GetFirstDescendantOfType<ScrollViewer>();
            var scrollBar = scrollViewer.GetDescendantsOfType<ScrollBar>().Single(temp => temp.Orientation == Orientation.Vertical);
            scrollBar.ValueChanged += ScrollBar_ValueChanged;
        }

        private void ScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // TODO
            var value = e.NewValue;
            if (value <= 10)
            {
                Header.Height = 80;
            }
            else if (value >= 210)
            {
                Header.Height = 30;
            }
            else
            {
                Header.Height = value * -0.25d + 82.5d;
            }
        }

        private void PullToRefreshPanel_OnRefreshRequested(object sender, RefreshRequestedEventArgs e)
        {
            if (AppView.Instance.CurrentView == this)
            {
                var feeds = ViewModel.Feeds;
                if (feeds != null)
                {
                    EventHandler handler = null;
                    handler = delegate
                    {
                        // TODO
                        feeds.LoadMoreCompleted -= handler;

                        NotificationView.ShowToastMessage("刷新完成");
                    };
                    feeds.LoadMoreCompleted += handler;
                    feeds.Refresh();
                }
            }
        }
    }
}