using SoftwareKobo.ACGNews.DataModels;
using SoftwareKobo.ACGNews.DataSources;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using SoftwareKobo.ACGNews.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SoftwareKobo.ACGNews.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class IndexView : Page
    {
        public IndexView()
        {
            this.InitializeComponent();

            #region
            //var testC = new List<Acg17173Feed>();
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //testC.Add(new Acg17173Feed()
            //{
            //    Title = "三次元10周年原创动画1月9日开播　PV及声优等最新情报公开",
            //    Thumbnail = "http://i1.17173.itc.cn/2015/acg/2015/12/10/tn1210vu01aaaa.jpg",
            //    Summary = "曾参与制作过《黑岩射手》、《苍蓝钢铁的琶音》、《兔宝的悲惨日常》等动画的动画制作公司“三次元”，..."
            //});
            //this.DataContext = testC;

            //this.DataContext = new DataModels.FeedCollection<Acg178Feed>(new Acg178Service());

            this.DataContext = new IndexViewModel()
            {
            };
            #endregion
        }

        private void NewsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var container = NewsList.ContainerFromItem(e.ClickedItem);
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

            AppView.Instance.NavigateToDetail("hello world", detailRenderTransformOrigin);
        }

        private void NewsList_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = NewsList.GetFirstDescendantOfType<ScrollViewer>();
            var scrollBar = scrollViewer.GetDescendantsOfType<ScrollBar>().Single(temp => temp.Orientation == Orientation.Vertical);
            scrollBar.ValueChanged += ScrollBar_ValueChanged;
        }

        private void ScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
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
    }
}