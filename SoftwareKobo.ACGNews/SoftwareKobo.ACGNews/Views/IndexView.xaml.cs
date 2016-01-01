using SoftwareKobo.ACGNews.Controls;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using SoftwareKobo.ACGNews.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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
                detailRenderTransformOrigin = new Point(center.X / this.ActualWidth, center.Y / this.ActualHeight);
            }
            else
            {
                detailRenderTransformOrigin = new Point(0.5, 0.5);
            }

            NotificationView.ShowLoading("hello world");

            var service = Service.GetService(feed);
            try
            {
                var detail = await service.DetailAsync(feed);
                await AppView.Instance.NavigateToDetail(feed, detail, detailRenderTransformOrigin);
                feed.MarkAsReaded();
            }
            catch (Exception ex)
            {
                var rr = Windows.Web.WebError.GetStatus(ex.HResult);

                NotificationView.ShowToastMessage("加载失败");
                Debug.WriteLine(ex);
                //Debugger.Break();
            }
            //var detail = await Service.GetService(feed).DetailAsync(feed);

            NotificationView.HideLoading();
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
            var feeds = ViewModel.Feeds;
            if (feeds != null)
            {
                EventHandler handler = null;
                handler = async delegate
                {
                    feeds.LoadMoreCompleted -= handler;

                    await NotificationView.ShowToastMessage("刷新完成");
                };
                feeds.LoadMoreCompleted += handler;
                feeds.Refresh();
            }
        }
    }
}