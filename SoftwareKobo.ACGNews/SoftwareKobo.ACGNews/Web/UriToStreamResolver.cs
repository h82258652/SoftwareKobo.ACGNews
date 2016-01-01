using SoftwareKobo.ACGNews.Utils;
using System;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Web
{
    public class UriToStreamResolver : IUriToStreamResolver
    {
        /// <summary>
        /// WebView 缓存文件夹名称。
        /// </summary>
        private const string CacheFolderName = @"WebViewCache";

        private static StorageFolder _webViewCacheFolder;

        private readonly string _html;

        public UriToStreamResolver(string html)
        {
            _html = html;
        }

        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            return GetContent(uri).AsAsyncOperation();
        }

        private async Task<IInputStream> GetContent(Uri uri)
        {
            var url = uri.PathAndQuery;
            if (url.StartsWith("/"))
            {
                url = url.Substring(1);
            }
            if (string.IsNullOrEmpty(url))
            {
                using (var memoryStream = new InMemoryRandomAccessStream())
                {
                    await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(_html).AsBuffer());
                    return memoryStream.GetInputStreamAt(0);
                }
            }

            if (url.StartsWith("localhttps"))
            {
                // 转换 localhttps 回 https:
                url = url.Substring(10);
                url = "https:" + url;
            }
            else if (url.StartsWith("localhttp"))
            {
                // 转换 localhttp 回 http:
                url = url.Substring(9);
                url = "http:" + url;
            }

            var cacheFileName = Hash.GetMd5(url);

            if (_webViewCacheFolder == null)
            {
                _webViewCacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
            }

            var cacheFile = await _webViewCacheFolder.TryGetItemAsync(cacheFileName) as StorageFile;
            if (cacheFile != null)
            {
                return await cacheFile.OpenAsync(FileAccessMode.Read);
            }

            var originUri = new Uri(url);
            using (var client = new HttpClient())
            {
                var buffer = await client.GetBufferAsync(originUri);
                if (buffer.Length > 0)
                {
                    var file = await _webViewCacheFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBufferAsync(file, buffer);
                    return await file.OpenAsync(FileAccessMode.Read);
                }
            }

            // 返回空。
            using (var memoryStream = new InMemoryRandomAccessStream())
            {
                return memoryStream.GetInputStreamAt(0);
            }
        }
    }
}