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
        private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";

        public Task<string> DetailAsync(FeedBase feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            if (feed is Acg178Feed == false)
            {
                throw new InvalidOperationException("feed 类型错误");
            }

            return DetailAsync(feed.DetailLink);
        }

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
                            feed.Id = long.Parse(string.Join(string.Empty, matches.Select(temp => temp.Value)));

                            var img = (IHtmlImageElement)news.QuerySelector(".newspic img");
                            feed.Thumbnail = img.Source;

                            var summary = news.QuerySelector(".newstext");
                            feed.Summary = summary.TextContent;

                            var titleData = ((IHtmlDivElement)news.QuerySelector(".title_data")).TextContent;
                            feed.PublishTime =
                                DateTime.Parse(Regex.Match(titleData, @"\d{4}\-\d{2}\-\d{2} \d{2}:\d{2}").Value);

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
                            buffer.AppendLine("Html 解析错误");
                            buffer.AppendLine("Url:" + url);
                            buffer.AppendLine("当前Item:" + news.OuterHtml);
                            buffer.AppendLine("已解析:" + JsonConvert.SerializeObject(feed));
                            await UmengAnalytics.TrackException(ex, buffer.ToString());
                        }
                    }
                    return feeds;
                }
            }
        }

        private async Task<string> DetailAsync(string url)
        {
            var html = await GetHtml(url);
            var parser = new HtmlParser();
            using (var document = await parser.ParseAsync(html))
            {
                try
                {
                    var detail = document.QuerySelector("#txt");

                    var hasMore = document.QuerySelector("#getMore");
                    if (hasMore != null)
                    {
                        var totalPageSpan = hasMore.QuerySelector("span:last-child");
                        var totalPage = int.Parse(totalPageSpan.TextContent);

                        for (var page = 2; page <= totalPage; page++)
                        {
                            string thisPageUrl = url.Replace(".html", $"_{page}.html");
                            string thisPageDetail = await GetHtml(thisPageUrl);
                            detail.InnerHtml += thisPageDetail;
                        }
                    }

                    return detail.OuterHtml;
                }
                catch (Exception ex)
                {
                    var buffer = new StringBuilder();
                    buffer.AppendLine("Acg178 内容解析错误");
                    buffer.AppendLine("Url:" + url);
                    buffer.AppendLine("UserAgent:" + UserAgent);
                    buffer.AppendLine("Document:" + document.ToHtml());
                    await UmengAnalytics.TrackException(ex, buffer.ToString());
                    return "抱歉，解析错误";
                }
            }
        }

        private static async Task<string> GetHtml(string url)
        {
            if (url.Contains("_s.html") == false)
            {
                // 转换为手机版网页。
                url = url.Replace(".html", "_s.html");
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                return await client.GetStringAsync(new Uri(url));
            }
        }
    }
}