using SoftwareKobo.ACGNews.Converters;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Web
{
    public class UriToStreamResolver : IUriToStreamResolver
    {
        private readonly string _html;

        public UriToStreamResolver(string html)
        {
            _html = html;
        }

        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            return GetContent(uri).AsAsyncOperation();
        }

        private static StorageFolder _webViewCacheFolder;

        private const string CacheFolderName = @"WebViewCache";

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
                url = url.Substring(10);
                url = "https:" + url;
            }
            else if (url.StartsWith("localhttp"))
            {
                url = url.Substring(9);
                url = "http:" + url;
            }

            var cacheFileName = WebUtility.UrlEncode(url);

            if (_webViewCacheFolder == null)
            {
                _webViewCacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
            }

            var file = await _webViewCacheFolder.TryGetItemAsync(cacheFileName) as StorageFile;
            if (file != null)
            {
                return await file.OpenAsync(FileAccessMode.Read);
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Referer = new Uri(url);
                var buffer = await client.GetBufferAsync(new Uri(url));
                var bytes = buffer.ToArray();
                if (bytes.Length > 0)
                {
                    var fileC = await _webViewCacheFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBufferAsync(fileC, buffer);
                    return await fileC.OpenAsync(FileAccessMode.Read);
                }
            }

            using (var memoryStream = new InMemoryRandomAccessStream())
            {
                await memoryStream.WriteAsync(Encoding.UTF8.GetBytes("").AsBuffer());
                return memoryStream.GetInputStreamAt(0);
            }
        }
    }
}