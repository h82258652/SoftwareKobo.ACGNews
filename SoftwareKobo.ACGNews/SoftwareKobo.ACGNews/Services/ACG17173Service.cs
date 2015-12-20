using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UmengSDK;
using Windows.Devices.AllJoyn;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SoftwareKobo.ACGNews.Services
{
    public class Acg17173Service : IService<Acg17173Feed>
    {
        public async Task<IEnumerable<Acg17173Feed>> GetAsync(int page = 0)
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
                    }
                }
                return feeds;
            }
        }

        public async Task<string> DetailAsync(FeedBase feed)
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
            const string userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                var html = await client.GetStringAsync(new Uri(url));
                var parser = new HtmlParser();
                using (var document = await parser.ParseAsync(html))
                {
                    try
                    {
                        return document.QuerySelector(".art-bd").OuterHtml;
                    }
                    catch (Exception ex)
                    {
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

                        return "抱歉，解析错误";
                    }
                }
            }
        }
    }
}