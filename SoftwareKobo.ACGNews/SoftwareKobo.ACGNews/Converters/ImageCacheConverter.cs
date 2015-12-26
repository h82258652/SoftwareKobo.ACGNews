using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Converters
{
    public class ImageCacheConverter : IValueConverter
    {
        /// <summary>
        /// 缓存图片文件夹名称。
        /// </summary>
        private const string CacheFolderName = @"ImageCache";

        private static readonly IsolatedStorageFile IsoLocalFolder = IsolatedStorageFile.GetUserStoreForApplication();

        private static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

        private static readonly string LocalFolderPath = LocalFolder.Path;

        private static StorageFolder _imageCacheFolder;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var url = value as string;
            if (url == null)
            {
                return value;
            }

            if (url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
            {
                // 网络 url。

                var cacheFileName = WebUtility.UrlEncode(url);
                var cacheFilePath = Path.Combine(CacheFolderName, cacheFileName);

                if (IsoLocalFolder.FileExists(cacheFilePath))
                {
                    // 缓存文件存在，拼接路径。
                    return Path.Combine(LocalFolderPath, cacheFilePath);
                }
                if (IsNetworkAvailable())
                {
                    // 当前网络可用，缓存图片。
                    DownloadImageAndCache(url, cacheFileName);
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static async void DownloadImageAndCache(string url, string cacheFileName)
        {
            try
            {
                var uri = new Uri(url);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Referer = uri;
                    var buffer = await client.GetBufferAsync(uri);
                    var bytes = buffer.ToArray();
                    if (bytes.Length > 0)
                    {
                        if (_imageCacheFolder == null)
                        {
                            _imageCacheFolder = await LocalFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
                        }
                        var cacheImage = await _imageCacheFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteBytesAsync(cacheImage, bytes);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private static bool IsNetworkAvailable()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            return profile != null && profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }
    }
}