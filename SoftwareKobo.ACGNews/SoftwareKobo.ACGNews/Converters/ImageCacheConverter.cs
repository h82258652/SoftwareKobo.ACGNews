using SoftwareKobo.ACGNews.Utils;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;
using WinRTXamlToolkit.IO.Extensions;

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

        private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 520)";

        /// <summary>
        /// 获取图片缓存文件夹的大小。单位是字节。
        /// </summary>
        /// <returns></returns>
        public static async Task<ulong> GetCachedImagesSizeAsync()
        {
            if (_imageCacheFolder == null)
            {
                _imageCacheFolder = await LocalFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
            }
            return await _imageCacheFolder.GetSize();
        }

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

                var cacheFileName = Hash.GetMd5(url) + url.Length + Hash.GetSha1(url) + Path.GetExtension(url);
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
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                    var buffer = await client.GetBufferAsync(uri);
                    if (buffer.Length > 0)
                    {
                        if (_imageCacheFolder == null)
                        {
                            _imageCacheFolder = await LocalFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
                        }
                        var tempFile = await _imageCacheFolder.CreateFileAsync(Guid.NewGuid().ToString(), CreationCollisionOption.GenerateUniqueName);
                        await FileIO.WriteBufferAsync(tempFile, buffer);
                        await tempFile.RenameAsync(cacheFileName, NameCollisionOption.ReplaceExisting);
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