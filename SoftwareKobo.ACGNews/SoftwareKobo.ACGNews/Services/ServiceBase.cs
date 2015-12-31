using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Search;

namespace SoftwareKobo.ACGNews.Services
{
    public abstract class ServiceBase<T> : IService<T> where T : FeedBase
    {
        public abstract Task<string> DetailAsync(FeedBase feed);

        public abstract Task<IEnumerable<T>> GetAsync(int page = 0);

        public async Task<string> LoadArticleAsync(string url)
        {
            return await AppDatabase.LoadArticleAsync(url);
        }

        public async void SaveArticleAsync(string url, string html)
        {
            await AppDatabase.SaveArticleAsync(url, html);
        }

        public async Task<string> SetImgSrcToLocalSrcAsync(string html)
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
                        src = src.Substring(6);
                        src = "localhttps" + src;
                        img.Source = src;
                    }
                    else if (src.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
                    {
                        src = src.Substring(5);
                        src = "localhttp" + src;
                        img.Source = src;
                    }
                }
                return document.ToHtml();
            }
        }
    }
}