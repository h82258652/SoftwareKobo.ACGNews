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
    public class ACG178Service
    {
        public async Task<IEnumerable<ACG178Feed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            string url = page == 0 ? "http://acg.178.com" : "http://acg.178.com/index_" + (page + 1) + ".html";

            using (var client = new HttpClient())
            {
                var html = await client.GetStringAsync(new Uri(url));
                var parser = new HtmlParser();
                using (var document = parser.Parse(html))
                {
                    var feeds = new List<ACG178Feed>();
                    foreach (var news in document.QuerySelectorAll(".news_box"))
                    {
                        var feed = new ACG178Feed();

                        var anchor = (IHtmlAnchorElement)news.QuerySelector(".title > a");
                        feed.Title = anchor.Title;
                        feed.DetailLink = "http://acg.178.com" + anchor.PathName;

                        var img = (IHtmlImageElement)news.QuerySelector(".newspic img");
                        feed.Thumbnail = img.Source;

                        var summary = news.QuerySelector(".newstext");
                        feed.Summary = summary.TextContent;

                        var titleData = ((IHtmlDivElement)news.QuerySelector(".title_data")).TextContent;
                        feed.PublishTime = DateTime.Parse(Regex.Match(titleData, @"\d{4}\-\d{2}\-\d{2} \d{2}:\d{2}").Value);

                        var categories = news.QuerySelectorAll(".float_right > a").Cast<IHtmlAnchorElement>();
                        feed.Categories = categories.Select(temp => temp.Text).ToArray();

                        var tag = news.QuerySelector(".color_display");
                        feed.Tag = tag.TextContent;

                        var author = news.QuerySelector(".title_data > span:nth-child(3)");
                        if (author != null)
                        {
                            feed.Author = author.TextContent;
                        }

                        var originSource = news.QuerySelector(".title_data > span:nth-child(2)");
                        feed.OriginSource = originSource.TextContent;

                        feeds.Add(feed);
                    }

                    return feeds;
                }
            }
        }

        public Task<string> DetailAsync(ACG178Feed feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            return DetailAsync(feed.DetailLink);
        }

        private async Task<string> DetailAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var html = await client.GetStringAsync(new Uri(url));
                var parser = new HtmlParser();
                using (var document = parser.Parse(html))
                {
                    var nextPageElement = (IHtmlAnchorElement)document.QuerySelector("#cms_page_next");
                    var nextPageDetail = string.Empty;
                    if (nextPageElement!=null)
                    {
                        nextPageDetail = await DetailAsync(nextPageElement.Href);
                    }
                    var thisPageDetail = document.QuerySelector(".newstext").OuterHtml;
                    return thisPageDetail + nextPageDetail;
                }
            }
        }
    }
}