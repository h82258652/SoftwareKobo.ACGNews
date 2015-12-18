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
    public class TencentComicService : IService<TencentComicFeed>
    {
        public async Task<IEnumerable<TencentComicFeed>> GetAsync(int page = 0)
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
                            feed.DetailLink = "http://comic.qq.com" + anchor.Href;
                            var matches = Regex.Matches(anchor.PathName, @"\d+").Cast<Match>();
                            feed.SortId = long.Parse(string.Join(string.Empty, matches.Select(temp => temp.Value)));

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
                            buffer.AppendLine("Html 解释错误");
                            buffer.AppendLine("Url:" + url);
                            buffer.AppendLine("当前Item:" + topic.OuterHtml);
                            buffer.AppendLine("已解释:" + JsonConvert.SerializeObject(feed));
                            await UmengAnalytics.TrackException(ex, buffer.ToString());
                        }
                    }
                    return feeds;
                }
            }
        }

        public Task<string> DetailAsync(TencentComicFeed feed)
        {
            throw new NotImplementedException();
        }
    }
}