using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareKobo.ACGNews.Services
{
    public class GamerskyService
    {
        public async Task<IEnumerable<GamerskyFeed>> GetAsync(int page = 0)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            using (var client = new HttpClient())
            {
                var request = new GamerskyRequest()
                {
                    Page = page + 1
                };

                var queryString = "?jsondata=" + WebUtility.UrlEncode(JsonConvert.SerializeObject(request));
                var json = await client.GetStringAsync(new Uri("http://db2.gamersky.com/LabelJsonpAjax.aspx" + queryString));
                json = json.Substring(json.IndexOf('(') + 1);
                json = json.Substring(0, json.LastIndexOf(')'));
                var response = JsonConvert.DeserializeObject<GamerskyResponse>(json);

                if (response.Status == "ok")
                {
                    var parser = new HtmlParser();
                    using (var document = parser.Parse(response.Body))
                    {
                        var feeds = new List<GamerskyFeed>();
                        foreach (var li in document.QuerySelectorAll("li"))
                        {
                            var feed = new GamerskyFeed();

                            var anchor = (IHtmlAnchorElement)li.QuerySelector(".tit > a");
                            feed.Title = anchor.Text;
                            feed.DetailLink = anchor.Href;

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
                        return feeds;
                    }
                }

                return Enumerable.Empty<GamerskyFeed>();
            }
        }

        public Task<string> DetailAsync(GamerskyFeed feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException(nameof(feed));
            }

            return DetailAsync(feed.DetailLink);
        }

        private async Task<string> DetailAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using (var document = await BrowsingContext.New(config).OpenAsync(url))
            {
                var pagerElement = document.QuerySelector(".page_css");
                var nextPageElement = pagerElement.Children.OfType<IHtmlAnchorElement>().LastOrDefault(temp => temp.Text == "下一页");
                var nextPageDetail = string.Empty;
                if (nextPageElement != null)
                {
                    nextPageDetail = await DetailAsync(nextPageElement.Href);
                }
                pagerElement.Remove();
                var thisPageDetail = document.QuerySelector(".MidL_con").OuterHtml;
                return thisPageDetail + nextPageDetail;
            }
        }

        [JsonObject]
        private class GamerskyRequest
        {
            public GamerskyRequest()
            {
                Type = "putspecialbody";
                IsCache = true;
                CacheTime = 60;
                SpecialId = "2319";
                IsSpecialId = "true";
            }

            [JsonProperty("type")]
            public string Type
            {
                get;
                set;
            }

            [JsonProperty("isCahce")]
            public bool IsCache
            {
                get;
                set;
            }

            [JsonProperty("cacheTime")]
            public int CacheTime
            {
                get;
                set;
            }

            [JsonProperty("specialId")]
            public string SpecialId
            {
                get;
                set;
            }

            [JsonProperty("isSpecialId")]
            public string IsSpecialId
            {
                get;
                set;
            }

            [JsonProperty("page")]
            public int Page
            {
                get;
                set;
            }
        }

        [JsonObject]
        private class GamerskyResponse
        {
            [JsonProperty("status")]
            public string Status
            {
                get;
                set;
            }

            [JsonProperty("totalPages")]
            public int TotalPages
            {
                get;
                set;
            }

            [JsonProperty("body")]
            public string Body
            {
                get;
                set;
            }
        }
    }
}