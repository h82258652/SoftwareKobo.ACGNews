using System;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;

namespace SoftwareKobo.ACGNews.Converters
{
    public class ImageCacheConverter : IValueConverter
    {
        private static readonly StorageFolder ImageCacheFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync("ImageCache", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string url = value as string;
            if (url == null)
            {
                return value;
            }

            var cacheFileName = WebUtility.UrlEncode(url);
            var cacheImage = ImageCacheFolder.TryGetItemAsync(cacheFileName).GetAwaiter().GetResult();
            if (cacheImage != null)
            {
                return cacheImage.Path;
            }

            DownloadImageAndCache(url, cacheFileName);
            return url;
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
                    var buffer = (await client.GetBufferAsync(uri)).ToArray();
                    if (buffer.Length > 0)
                    {
                        var cacheImage = await ImageCacheFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteBytesAsync(cacheImage, buffer);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}