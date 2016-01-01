using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareKobo.ACGNews.Services
{
    public abstract class ServiceBase<T> : IService<T> where T : FeedBase
    {
        public async Task<string> ConvertImgSrcToLocalSrcAsync(string html)
        {
            var parser = new HtmlParser();
            using (var document = await parser.ParseAsync(html))
            {
                var imgs = document.All.OfType<IHtmlImageElement>();
                foreach (var img in imgs)
                {
                    var src = img.Source;
                    if (src.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
                    {
                        // 转换 https: 为 localhttps
                        src = src.Substring(6);
                        src = "localhttps" + src;
                        img.Source = src;
                    }
                    else if (src.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
                    {
                        // 转换 http: 为 localhttp
                        src = src.Substring(5);
                        src = "localhttp" + src;
                        img.Source = src;
                    }
                }
                return document.ToHtml();
            }
        }

        public abstract Task<string> DetailAsync(FeedBase feed);

        public abstract Task<IEnumerable<T>> GetAsync(int page = 0);

        public async Task<string> LoadArticleAsync(string url)
        {
            return await AppDatabase.LoadArticleAsync(url);
        }

        public async Task SaveArticleAsync(string url, string html)
        {
            await AppDatabase.SaveArticleAsync(url, html);
        }

        public static IService GetService(FeedBase feed)
        {
            if (feed is Acg17173Feed)
            {
                return new Acg17173Service();
            }
            if (feed is Acg178Feed)
            {
                return new Acg178Service();
            }
            if (feed is AcgdogeFeed)
            {
                return new AcgdogeService();
            }
            if (feed is AcgGamerskyFeed)
            {
                return new AcgGamerskyService();
            }
            if (feed is TencentComicFeed)
            {
                return new TencentComicService();
            }

            throw new ArgumentException("无法根据该类型 feed 创建服务", nameof(feed));
        }
    }
}