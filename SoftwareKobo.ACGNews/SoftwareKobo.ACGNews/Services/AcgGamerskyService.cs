using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UmengSDK;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Services
{
    public class AcgGamerskyService : IService<AcgGamerskyFeed>
    {
        public async Task<IEnumerable<AcgGamerskyFeed>> GetAsync(int page = 0)
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
                            feed.SortId = long.Parse(string.Join(string.Empty, matches.Select(temp => temp.Value)));

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
                            buffer.AppendLine("Html 解释错误");
                            buffer.AppendLine("Url:" + url);
                            buffer.AppendLine("当前Item:" + li.OuterHtml);
                            buffer.AppendLine("已解释:" + JsonConvert.SerializeObject(feed));
                            await UmengAnalytics.TrackException(ex, buffer.ToString());
                        }
                    }
                    return feeds;
                }
            }
        }

        public Task<string> DetailAsync(AcgGamerskyFeed feed)
        {
            throw new NotImplementedException();
        }
    }
}