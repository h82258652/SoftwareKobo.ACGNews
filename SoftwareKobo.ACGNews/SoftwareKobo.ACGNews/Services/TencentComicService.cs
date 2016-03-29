using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Html;
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
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace SoftwareKobo.ACGNews.Services
{
    public class TencentComicService : ServiceBase<TencentComicFeed>
    {
        private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";

        public override async Task<string> DetailAsync(FeedBase feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            if (feed is TencentComicFeed == false)
            {
                throw new InvalidOperationException("feed 类型错误");
            }

            var url = feed.DetailLink;
            var detail = await LoadArticleAsync(url);
            if (string.IsNullOrEmpty(detail))
            {
                detail = await NetworkDetailAsync(url);
            }
            detail = await ConvertImgSrcToLocalSrcAsync(detail);
            return detail;
        }

        public override async Task<IEnumerable<TencentComicFeed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var url = string.Format("http://comic.qq.com/c/dmlist_{0}.htm", page + 1);
            using (var client = new HttpClient())
            {
                var html = await client.GetStringAsync(new Uri(url));
                var parser = new HtmlParser();
                using (var document = parser.Parse(html))
                {
                    var feeds = new List<TencentComicFeed>();
                    foreach (var topic in document.QuerySelectorAll(".Q-tpList"))
                    {
                        var feed = new TencentComicFeed();
                        try
                        {
                            var anchor = (IHtmlAnchorElement)topic.QuerySelector("h3 > a");
                            feed.Title = anchor.Text;
                            feed.DetailLink = "http://comic.qq.com" + anchor.PathName;
                            var matches = Regex.Matches(anchor.PathName, @"\d+").Cast<Match>();
                            feed.Id = long.Parse(string.Join(string.Empty, matches.Select(temp => temp.Value)));

                            var img = (IHtmlImageElement)topic.QuerySelector(".pic > img");
                            feed.Thumbnail = img.Source;

                            var summary = topic.QuerySelector("p");
                            feed.Summary = summary.TextContent;

                            var newsInfo = topic.QuerySelector(".newsinfo");
                            var publishTime = newsInfo.QuerySelector(".fl > span");
                            feed.PublishTime = Regex.Replace(publishTime.TextContent.Replace("更新", string.Empty), @"\s*",
                                " ");
                            var tags = newsInfo.QuerySelector(".fl > span:nth-child(2)").QuerySelectorAll("span");
                            if (tags != null)
                            {
                                feed.Tags = tags.Select(temp => temp.TextContent).ToArray();
                            }

                            feeds.Add(feed);
                        }
                        catch (Exception ex)
                        {
                            var buffer = new StringBuilder();
                            buffer.AppendLine("Html 解析错误");
                            buffer.AppendLine("Url:" + url);
                            buffer.AppendLine("当前Item:" + topic.OuterHtml);
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
        }

        private static async Task<string> GetHtml(string url)
        {
            if (url.Contains("xw.qq.com") == false)
            {
                url = "http://xw.qq.com/comic/" + string.Join(string.Empty, Regex.Matches(url, @"\d+").Cast<Match>().Select(temp => temp.Value));
            }

            var filter = new HttpBaseProtocolFilter
            {
                AllowAutoRedirect = false
            };
            using (var client = new HttpClient(filter))
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                return await client.GetStringAsync(new Uri(url));
            }
        }

        private async Task<string> NetworkDetailAsync(string url)
        {
            var html = await GetHtml(url);
            var parser = new HtmlParser();
            using (var document = await parser.ParseAsync(html))
            {
                try
                {
                    var detail = document.QuerySelector(".content").OuterHtml;
                    await SaveArticleAsync(url, detail);
                    return detail;
                }
                catch (Exception ex)
                {
                    var buffer = new StringBuilder();
                    buffer.AppendLine("TencentComic 内容解析错误");
                    buffer.AppendLine("Url:" + url);
                    buffer.AppendLine("UserAgent:" + UserAgent);
                    buffer.AppendLine("Document:" + document.ToHtml(HtmlMarkupFormatter.Instance));
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