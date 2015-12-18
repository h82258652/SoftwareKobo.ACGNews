using AngleSharp;
using AngleSharp.Dom.Html;
using Newtonsoft.Json;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UmengSDK;

namespace SoftwareKobo.ACGNews.Services
{
    public class AcgdogeService : IService<AcgdogeFeed>
    {
        public async Task<IEnumerable<AcgdogeFeed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var url = string.Format("http://www.acgdoge.net/page/{0}", page + 1);
            var config = Configuration.Default.WithDefaultLoader();
            using (var document = await BrowsingContext.New(config).OpenAsync(url))
            {
                var feeds = new List<AcgdogeFeed>();
                foreach (var article in document.QuerySelectorAll("article"))
                {
                    var feed = new AcgdogeFeed();
                    try
                    {
                        var anchor = (IHtmlAnchorElement)article.QuerySelector(".post_h > a");
                        feed.Title = anchor.Text;
                        feed.DetailLink = anchor.Href;
                        feed.SortId = long.Parse(Regex.Match(anchor.PathName, @"\d+").Value);

                        var img = (IHtmlImageElement)article.QuerySelector(".post_t noscript > img");
                        feed.Thumbnail = img.Source;

                        var summary = article.QuerySelector(".post_t > p:nth-child(2)");
                        feed.Summary = summary.TextContent;

                        var monthDay = article.QuerySelector(".post_t_d");
                        var hourMinute = article.QuerySelector(".post_t_u");
                        feed.PublishTime = monthDay.TextContent + " " + hourMinute.TextContent;

                        var commentAnchor = article.QuerySelector(".post_c > a");
                        int commentCount;
                        if (int.TryParse(Regex.Replace(commentAnchor.TextContent, @"[^\d]*", string.Empty), out commentCount))
                        {
                            feed.CommentCount = commentCount;
                        }

                        var categories = article.QuerySelectorAll(".post_ct > a").Cast<IHtmlAnchorElement>();
                        feed.Categories = categories.Select(temp => temp.Text).ToArray();

                        var tags = article.QuerySelectorAll(".post_tag > a").Cast<IHtmlAnchorElement>();
                        feed.Tags = tags.Select(temp => temp.Text).ToArray();

                        feeds.Add(feed);
                    }
                    catch (Exception ex)
                    {
                        var buffer = new StringBuilder();
                        buffer.AppendLine("Html 解释错误");
                        buffer.AppendLine("Url:" + url);
                        buffer.AppendLine("当前Item:" + article.OuterHtml);
                        buffer.AppendLine("已解释:" + JsonConvert.SerializeObject(feed));
                        await UmengAnalytics.TrackException(ex, buffer.ToString());
                    }
                }
                return feeds;
            }
        }

        public async Task<string> DetailAsync(AcgdogeFeed feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            var config = Configuration.Default.WithDefaultLoader();
            using (var document = await BrowsingContext.New(config).OpenAsync(feed.DetailLink))
            {
                return document.QuerySelector(".post_t").OuterHtml;
            }
        }
    }
}