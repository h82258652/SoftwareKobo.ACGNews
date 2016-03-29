using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmengSDK;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Services
{
    public class AnitamaService : ServiceBase<AnitamaFeed>
    {
        public override Task<string> DetailAsync(FeedBase feed)
        {
            throw new NotImplementedException();
        }

        public override async Task<IEnumerable<AnitamaFeed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var url = $"https://app.anitama.net/timeline?pageNo={(page + 1)}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

                var json = await client.GetStringAsync(new Uri(url));
                var response = JObject.Parse(json);

                if ((bool)response["success"] == false)
                {
                    return Enumerable.Empty<AnitamaFeed>();
                }

                var list = (JArray)(response["data"]["page"]["list"]);
                var feeds = new List<AnitamaFeed>();
                foreach (var element in list)
                {
                    var feed = new AnitamaFeed();

                    try
                    {
                        var releaseDate = (DateTime)element["releaseDate"];
                        feed.Id = releaseDate.Ticks;

                        feed.Title = (string)element["title"];
                        feed.Summary = (string)element["subtitle"];

                        var entryType = (string)element["entryType"];
                        switch (entryType)
                        {
                            case "article":

                                feed.DetailLink = string.Format("http://m.anitama.cn/#page=article;aid={0}",
                                    (string)element["aid"]);
                                feed.Thumbnail = (string)element["cover"]["url"];
                                break;

                            case "guide":

                                feed.DetailLink =
                                    $"http://m.anitama.cn/#page=guide;date={releaseDate.ToString("yyyyMMdd")}";
                                feed.Thumbnail = (string)element["cover"];
                                break;

                            default:

                                var buffer = new StringBuilder();
                                buffer.AppendLine("Anitama 未知类型");
                                buffer.AppendLine("原始 Json" + json);
                                await UmengAnalytics.TrackError(buffer.ToString());
                                continue;
                        }

                        feeds.Add(feed);
                    }
                    catch (Exception ex)
                    {
                        var buffer = new StringBuilder();
                        buffer.AppendLine("Json 解析错误");
                        buffer.AppendLine("Url:" + url);
                        buffer.AppendLine("当前Item:" + JsonConvert.SerializeObject(element));
                        buffer.AppendLine("全部 Json:" + json);
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

        private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";
    }
}