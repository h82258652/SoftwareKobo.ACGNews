using AngleSharp;
using AngleSharp.Dom.Html;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftwareKobo.ACGNews.Services
{
    public class ACG17173Service
    {
        public async Task<IEnumerable<ACG17173Feed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            string url = page == 0 ? "http://acg.17173.com/anime/" : "http://acg.17173.com/anime/index_" + page + ".shtml";

            var config = Configuration.Default.WithDefaultLoader();
            using (var document = await BrowsingContext.New(config).OpenAsync(url))
            {
                var feeds = new List<ACG17173Feed>();
                foreach (var item in document.QuerySelectorAll(".comm-plist3 > .item"))
                {
                    var feed = new ACG17173Feed();

                    var title = item.QuerySelector(".tit");
                    feed.Title = title.TextContent;

                    var anchor = (IHtmlAnchorElement)item.QuerySelector("a.co");
                    feed.DetailLink = anchor.Href;

                    var img = (IHtmlImageElement)item.QuerySelector(".pic");
                    feed.Thumbnail = img.Source;

                    var summary = item.QuerySelector(".txt");
                    feed.Summary = summary.TextContent;

                    feeds.Add(feed);
                }
                return feeds;
            }
        }

        public async Task<string> DetailAsync(ACG17173Feed feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            var config = Configuration.Default.WithDefaultLoader();
            using (var document = await BrowsingContext.New(config).OpenAsync(feed.DetailLink))
            {
                return document.QuerySelector("#mod_article").OuterHtml;
            }
        }
    }
}