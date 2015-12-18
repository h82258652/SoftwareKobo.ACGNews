using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UmengSDK;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Services
{
    public class Acg178Service : IService<Acg178Feed>
    {
        public async Task<IEnumerable<Acg178Feed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var url = page == 0 ? "http://acg.178.com" : $"http://acg.178.com/index_{(page + 1)}.html";

            using (var client = new HttpClient())
            {
                var html = await client.GetStringAsync(new Uri(url));
                var parser = new HtmlParser();
                using (var document = await parser.ParseAsync(html))
                {
                    var feeds = new List<Acg178Feed>();
                    foreach (var news in document.QuerySelectorAll(".news_box"))
                    {
                        var feed = new Acg178Feed();
                        try
                        {
                            var anchor = (IHtmlAnchorElement)news.QuerySelector(".title > a");
                            feed.Title = anchor.Title;
                            feed.DetailLink = "http://acg.178.com" + anchor.PathName;
                            var matches = Regex.Matches(anchor.PathName, @"\d+").Cast<Match>();
                            feed.SortId = long.Parse(string.Join(string.Empty, matches.Select(temp => temp.Value)));

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
                        catch (Exception ex)
                        {
                            var buffer = new StringBuilder();
                            buffer.AppendLine("Html 解释错误");
                            buffer.AppendLine("Url:" + url);
                            buffer.AppendLine("当前Item:" + news.OuterHtml);
                            buffer.AppendLine("已解释:" + JsonConvert.SerializeObject(feed));
                            await UmengAnalytics.TrackException(ex, buffer.ToString());
                        }
                    }
                    return feeds;
                }
            }
        }

        public Task<string> DetailAsync(Acg178Feed feed)
        {
            throw new NotImplementedException();
        }
    }
}