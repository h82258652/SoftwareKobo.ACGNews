using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UmengSDK;
using Windows.Web.Http;

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
            using (var client = new HttpClient())
            {
                var byteArray = (await client.GetBufferAsync(new Uri(url))).ToArray();
                var html = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                var parser = new HtmlParser();
                using (var document = await parser.ParseAsync(html))
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
                            feed.Id = long.Parse(Regex.Match(anchor.PathName, @"\d+").Value);

                            var img = (IHtmlImageElement)article.QuerySelector(".post_t noscript > img");
                            feed.Thumbnail = img.Source;

                            var readMoreElement = article.QuerySelector(".more-link");
                            readMoreElement?.Remove();

                            var summary = article.QuerySelectorAll(".post_t > p");
                            feed.Summary = string.Join(" ", summary.Skip(1).Select(temp => temp.TextContent.Trim()));

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
                            buffer.AppendLine("Html 解析错误");
                            buffer.AppendLine("Url:" + url);
                            buffer.AppendLine("当前Item:" + article.OuterHtml);
                            buffer.AppendLine("已解析:" + JsonConvert.SerializeObject(feed));
                            await UmengAnalytics.TrackException(ex, buffer.ToString());
                        }
                    }
                    return feeds;
                }
            }
        }

        public async Task<string> DetailAsync(FeedBase feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            if (feed is AcgdogeFeed == false)
            {
                throw new InvalidOperationException("feed 类型错误");
            }

            var url = feed.DetailLink;
            const string userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                var byteArray = (await client.GetBufferAsync(new Uri(url))).ToArray();
                var html = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                var parser = new HtmlParser();
                using (var document = await parser.ParseAsync(html))
                {
                    try
                    {
                        var imgs = document.GetElementsByTagName("img").Cast<IHtmlImageElement>();
                        foreach (var img in imgs)
                        {
                            string realSrc = img.GetAttribute("data-lazy-src");
                            if (string.IsNullOrEmpty(realSrc) == false)
                            {
                                img.Source = img.GetAttribute("data-lazy-src");
                            }
                        }
                        var detal = document.QuerySelector(".post_t");
                        var quote = detal.QuerySelector(".post_h_quote");
                        quote?.Remove();
                        return detal.OuterHtml;
                    }
                    catch (Exception ex)
                    {
                        var buffer = new StringBuilder();
                        buffer.AppendLine("Acgdoge 内容解析错误");
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