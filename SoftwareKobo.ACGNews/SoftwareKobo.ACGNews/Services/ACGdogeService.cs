using AngleSharp;
using AngleSharp.Dom.Html;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoftwareKobo.ACGNews.Services
{
    public class ACGdogeService
    {
        public async Task<IEnumerable<ACGdogeFeed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var config = Configuration.Default.WithDefaultLoader();
            using (var document = await BrowsingContext.New(config).OpenAsync("http://www.acgdoge.net/page/" + (page + 1)))
            {
                var feeds = new List<ACGdogeFeed>();
                foreach (var article in document.QuerySelectorAll("article"))
                {
                    var feed = new ACGdogeFeed();

                    var anchor = (IHtmlAnchorElement)article.QuerySelector(".post_h > a");
                    feed.Title = anchor.Text;
                    feed.DetailLink = anchor.Href;

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

                return feeds;
            }
        }

        public async Task<string> DetailAsync(ACGdogeFeed feed)
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