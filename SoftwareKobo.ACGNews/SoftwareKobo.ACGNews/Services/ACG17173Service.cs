using AngleSharp;
using AngleSharp.Dom.Html;
using Newtonsoft.Json;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UmengSDK;

namespace SoftwareKobo.ACGNews.Services
{
    public class Acg17173Service : IService<Acg17173Feed>
    {
        public async Task<IEnumerable<Acg17173Feed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var url = page == 0 ? "http://acg.17173.com/anime/" : $"http://acg.17173.com/anime/index_{page}.shtml";

            var config = Configuration.Default.WithDefaultLoader();
            using (var document = await BrowsingContext.New(config).OpenAsync(url))
            {
                var feeds = new List<Acg17173Feed>();
                foreach (var item in document.QuerySelectorAll(".comm-plist3 > .item"))
                {
                    var feed = new Acg17173Feed();
                    try
                    {
                        var title = item.QuerySelector(".tit");
                        feed.Title = title.TextContent;

                        var anchor = (IHtmlAnchorElement)item.QuerySelector("a.co");
                        feed.DetailLink = anchor.Href;
                        var matches = Regex.Matches(anchor.PathName, @"\d+");
                        var monthDayYear = matches[0].Value;
                        var monthDay = monthDayYear.Substring(0, 4);
                        var year = monthDayYear.Substring(5);
                        feed.SortId = long.Parse(year + monthDay + matches[1].Value);

                        var img = (IHtmlImageElement)item.QuerySelector(".pic");
                        feed.Thumbnail = img.Source;

                        var summary = item.QuerySelector(".txt");
                        feed.Summary = summary.TextContent;

                        feeds.Add(feed);
                    }
                    catch (Exception ex)
                    {
                        var buffer = new StringBuilder();
                        buffer.AppendLine("Html 解释错误");
                        buffer.AppendLine("Url:" + url);
                        buffer.AppendLine("当前Item:" + item.OuterHtml);
                        buffer.AppendLine("已解释:" + JsonConvert.SerializeObject(feed));
                        await UmengAnalytics.TrackException(ex, buffer.ToString());
                    }
                }
                return feeds;
            }
        }

        public Task<string> DetailAsync(Acg17173Feed feed)
        {
            throw new NotImplementedException();
        }
    }
}