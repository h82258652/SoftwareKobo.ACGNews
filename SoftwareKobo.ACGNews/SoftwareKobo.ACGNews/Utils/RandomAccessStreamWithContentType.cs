using System;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace SoftwareKobo.ACGNews.Utils
{
    public class RandomAccessStreamWithContentType : IRandomAccessStreamWithContentType
    {
        private readonly IRandomAccessStream _stream;

        public RandomAccessStreamWithContentType(IRandomAccessStream stream, string contentType)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (contentType == null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            _stream = stream;
            ContentType = contentType;
        }

        public bool CanRead => _stream.CanRead;

        public bool CanWrite => _stream.CanWrite;

        public string ContentType
        {
            get;
        }

        public ulong Position => _stream.Position;

        public ulong Size
        {
            get
            {
                return _stream.Size;
            }

            set
            {
                _stream.Size = value;
            }
        }

        public IRandomAccessStream CloneStream()
        {
            return _stream.CloneStream();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public IAsyncOperation<bool> FlushAsync()
        {
            return _stream.FlushAsync();
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            return _stream.GetInputStreamAt(position);
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            return _stream.GetOutputStreamAt(position);
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return _stream.ReadAsync(buffer, count, options);
        }

        public void Seek(ulong position)
        {
            _stream.Seek(position);
        }

        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return _stream.WriteAsync(buffer);
        }
    }
}