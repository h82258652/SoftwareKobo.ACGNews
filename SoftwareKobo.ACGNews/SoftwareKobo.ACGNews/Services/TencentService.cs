using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Services
{
    public class TencentService
    {
        public async Task<IEnumerable<TencentFeed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var url = "http://comic.qq.com/c/dmlist_" + (page + 1) + ".htm";
            using (var client = new HttpClient())
            {
                var html = await client.GetStringAsync(new Uri(url));
                var parser = new HtmlParser();
                using (var document = parser.Parse(html))
                {
                    var feeds = new List<TencentFeed>();
                    foreach (var topic in document.QuerySelectorAll(".Q-tpList"))
                    {
                        var feed = new TencentFeed();

                        var anchor = (IHtmlAnchorElement)topic.QuerySelector("h3 > a");
                        feed.Title = anchor.Text;
                        feed.DetailLink = "http://comic.qq.com" + anchor.Href;

                        var img = (IHtmlImageElement)topic.QuerySelector(".pic > img");
                        feed.Thumbnail = img.Source;

                        var summary = topic.QuerySelector("p");
                        feed.Summary = summary.TextContent;

                        var newsInfo = topic.QuerySelector(".newsinfo");
                        var publishTime = newsInfo.QuerySelector(".fl > span");
                        feed.PublishTime = Regex.Replace(publishTime.TextContent.Replace("更新", string.Empty), @"\s*", " ");
                        var tags = newsInfo.QuerySelector(".fl > span:nth-child(2)").QuerySelectorAll("span");
                        if (tags != null)
                        {
                            feed.Tags = tags.Select(temp => temp.TextContent).ToArray();
                        }

                        feeds.Add(feed);
                    }

                    return feeds;
                }
            }
        }

        public async Task<string> DetailAsync(TencentFeed feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            using (var client = new HttpClient())
            {
                var html = await client.GetStringAsync(new Uri(feed.DetailLink));
                var parser = new HtmlParser();
                using (var document = parser.Parse(html))
                {
                    return document.QuerySelector("#Cnt-Main-Article-QQ").OuterHtml;
                }
            }
        }
    }
}