using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UmengSDK;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Services
{
    public class AcgGamerskyService : ServiceBase<AcgGamerskyFeed>
    {
        private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";

        public override async Task<string> DetailAsync(FeedBase feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            if (feed is AcgGamerskyFeed == false)
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

        public override async Task<IEnumerable<AcgGamerskyFeed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            using (var client = new HttpClient())
            {
                var request = new
                {
                    type = "putspecialbody",
                    isCache = true,
                    cacheTime = 60,
                    specialId = "2319",
                    isSpecialId = "true",
                    page = page + 1
                };

                var queryString = "?jsondata=" + WebUtility.UrlEncode(JsonConvert.SerializeObject(request));
                var url = "http://db2.gamersky.com/LabelJsonpAjax.aspx" + queryString;
                var json = await client.GetStringAsync(new Uri(url));
                json = json.Substring(json.IndexOf('(') + 1);
                json = json.Substring(0, json.LastIndexOf(')'));
                var response = JObject.Parse(json);

                if ((string)response["status"] != "ok")
                {
                    return Enumerable.Empty<AcgGamerskyFeed>();
                }

                var parser = new HtmlParser();
                using (var document = await parser.ParseAsync((string)response["body"]))
                {
                    var feeds = new List<AcgGamerskyFeed>();
                    foreach (var li in document.QuerySelectorAll("li"))
                    {
                        var feed = new AcgGamerskyFeed();
                        try
                        {
                            var anchor = (IHtmlAnchorElement)li.QuerySelector(".tit > a");
                            feed.Title = anchor.Text;
                            feed.DetailLink = anchor.Href;
                            var matches = Regex.Matches(anchor.PathName, @"\d+").Cast<Match>();
                            feed.Id = long.Parse(string.Join(string.Empty, matches.Select(temp => temp.Value)));

                            var img = (IHtmlImageElement)li.QuerySelector(".pic1");
                            feed.Thumbnail = img.Source;

                            var summary = li.QuerySelector(".txt");
                            feed.Summary = summary.TextContent;

                            var time = li.QuerySelector(".time");
                            feed.PublishTime = DateTime.Parse(time.TextContent);

                            var view = li.QuerySelector(".Views");
                            feed.ViewCount = int.Parse(view.TextContent);

                            var comment = li.QuerySelector(".Comments");
                            feed.CommentCount = int.Parse(comment.TextContent);

                            feeds.Add(feed);
                        }
                        catch (Exception ex)
                        {
                            var buffer = new StringBuilder();
                            buffer.AppendLine("Html 解析错误");
                            buffer.AppendLine("Url:" + url);
                            buffer.AppendLine("当前Item:" + li.OuterHtml);
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

        private async Task<string> GetHtml(string url)
        {
            if (url.Contains("wap.gamersky.com") == false)
            {
                // 转换为手机版网页。
                var id = Regex.Matches(url, @"\d+").Cast<Match>().Last().Value;
                url = string.Format("http://wap.gamersky.com/news/Content-{0}.html", id);
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                return await client.GetStringAsync(new Uri(url));
            }
        }

        private async Task<string> NetworkDetailAsync(string url)
        {
            var builder = new StringBuilder();
            var originUrl = url;
            while (true)
            {
                var html = await GetHtml(url);
                var parser = new HtmlParser();
                using (var document = await parser.ParseAsync(html))
                {
                    try
                    {
                        var pagerElement = document.QuerySelector(".Page");
                        IHtmlAnchorElement nextPageElement = null;
                        if (pagerElement != null)
                        {
                            nextPageElement = (IHtmlAnchorElement)pagerElement.QuerySelector(".page-next");
                        }
                        pagerElement?.Remove();
                        var detail = document.QuerySelector(".Mid_2").OuterHtml;
                        builder.Append(detail);
                        if (nextPageElement != null)
                        {
                            url = string.Format("http://wap.gamersky.com/news{0}", nextPageElement.PathName);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        var buffer = new StringBuilder();
                        buffer.AppendLine("AcgGamersky 内容解析错误");
                        buffer.AppendLine("Url:" + url);
                        buffer.AppendLine("UserAgent:" + UserAgent);
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
            var totalDetail = builder.ToString();
            await SaveArticleAsync(originUrl, totalDetail);
            return totalDetail;
        }
    }
}