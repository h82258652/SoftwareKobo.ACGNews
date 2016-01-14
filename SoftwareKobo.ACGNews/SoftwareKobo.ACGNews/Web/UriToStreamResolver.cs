using SoftwareKobo.ACGNews.Extensions;
using SoftwareKobo.ACGNews.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web;
using Windows.Web.Http;
using WinRTXamlToolkit.IO.Extensions;

namespace SoftwareKobo.ACGNews.Web
{
    public class UriToStreamResolver : IUriToStreamResolver
    {
        /// <summary>
        /// WebView 缓存文件夹名称。
        /// </summary>
        private const string CacheFolderName = @"WebViewCache";

        private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";

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

            var cacheFileName = Hash.GetMd5(url) + url.Length + Hash.GetSha1(url) + Path.GetExtension(url);

            if (_webViewCacheFolder == null)
            {
                _webViewCacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
            }

            var cacheFile = await _webViewCacheFolder.TryGetItemAsync(cacheFileName) as StorageFile;
            if (cacheFile != null)
            {
                return await cacheFile.OpenAsync(FileAccessMode.Read);
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                var buffer = await client.GetBufferAsync(new Uri(url));
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

        public static async Task<ulong> GetCachedImagesSizeAsync()
        {
            if (_webViewCacheFolder == null)
            {
                return 0;
            }
            return await _webViewCacheFolder.GetSizeAsync();
        }

        public static async Task CleanUpCacheAsync()
        {
            if (_webViewCacheFolder != null)
            {
                await _webViewCacheFolder.DeleteFilesAsync(true);
            }
        }
    }
}