using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UmengSDK;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Services
{
    public class Acg17173Service : ServiceBase<Acg17173Feed>
    {
        public override async Task<string> DetailAsync(FeedBase feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            if (feed is Acg17173Feed == false)
            {
                throw new InvalidOperationException("feed 类型错误");
            }

            var url = feed.DetailLink;
            // 加载缓存。
            var detail = await LoadArticleAsync(url);
            if (string.IsNullOrEmpty(detail))
            {
                // 缓存不存在，加载网络数据。
                detail = await NetworkDetailAsync(url);
            }
            // 转换 img 标签的 src 为本地临时路径。
            detail = await ConvertImgSrcToLocalSrcAsync(detail);
            return detail;
        }

        public override async Task<IEnumerable<Acg17173Feed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var url = page == 0 ? "http://acg.17173.com/anime/" : $"http://acg.17173.com/anime/index_{page}.shtml";

            var config = Configuration.Default.WithDefaultLoader();
            using (var document = await BrowsingContext.New(config).OpenAsync(url))
            {
                var feeds = new List<Acg17173Feed>();
                foreach (var item in document.QuerySelectorAll(".comm-plist3 > .item"))
                {
                    var feed = new Acg17173Feed();
                    try
                    {
                        var title = item.QuerySelector(".tit");
                        feed.Title = title.TextContent;

                        var anchor = (IHtmlAnchorElement)item.QuerySelector("a.co");
                        feed.DetailLink = anchor.Href;
                        var matches = Regex.Matches(anchor.PathName, @"\d+");
                        var monthDayYear = matches[0].Value;
                        var monthDay = monthDayYear.Substring(0, 4);
                        var year = monthDayYear.Substring(5);
                        feed.Id = long.Parse(year + monthDay + matches[1].Value);

                        var img = (IHtmlImageElement)item.QuerySelector(".pic");
                        feed.Thumbnail = img.Source;

                        var summary = item.QuerySelector(".txt");
                        feed.Summary = summary.TextContent;

                        feeds.Add(feed);
                    }
                    catch (Exception ex)
                    {
                        var buffer = new StringBuilder();
                        buffer.AppendLine("Html->Acg17173Feed 解析错误");
                        buffer.AppendLine("Url:" + url);
                        buffer.AppendLine("当前Item:" + item.OuterHtml);
                        buffer.AppendLine("已解析:" + JsonConvert.SerializeObject(feed));
                        await UmengAnalytics.TrackException(ex, buffer.ToString());

                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                    }
                }
                return feeds;
            }
        }

        private async Task<string> NetworkDetailAsync(string url)
        {
            const string userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";
            // 最大尝试 3 次，17173 第一次时可能会失败。
            for (var errorTimes = 0; errorTimes < 3; errorTimes++)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                    var html = await client.GetStringAsync(new Uri(url));
                    var parser = new HtmlParser();
                    using (var document = await parser.ParseAsync(html))
                    {
                        try
                        {
                            var detail = document.QuerySelector(".art-bd").OuterHtml;
                            // 缓存内容。
                            await SaveArticleAsync(url, detail);
                            return detail;
                        }
                        catch (Exception ex)
                        {
                            if (errorTimes == 2)
                            {
                                // 最后一次解析也失败了。
                                var buffer = new StringBuilder();
                                buffer.AppendLine("Acg17173 内容解析错误");
                                buffer.AppendLine("Url:" + url);
                                buffer.AppendLine("UserAgent:" + userAgent);
                                buffer.AppendLine("Document:" + document.ToHtml());
                                await UmengAnalytics.TrackException(ex, buffer.ToString());

                                if (Debugger.IsAttached)
                                {
                                    Debugger.Break();
                                }
                            }
                        }
                    }
                }
            }

            return "抱歉，解析错误";
        }
    }
}