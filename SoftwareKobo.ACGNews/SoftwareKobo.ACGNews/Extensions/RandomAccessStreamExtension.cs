using SoftwareKobo.ACGNews.Utils;
using Windows.Storage.Streams;

namespace SoftwareKobo.ACGNews.Extensions
{
    public static class RandomAccessStreamExtension
    {
        public static RandomAccessStreamWithContentType WithContentType(this IRandomAccessStream stream, string contentType)
        {
            return new RandomAccessStreamWithContentType(stream, contentType);
        }
    }
}