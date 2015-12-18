using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.DataSources;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace SoftwareKobo.ACGNews.DataModels
{
    public class FeedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading where T : FeedBase
    {
        private readonly FeedSource<T> _feedSource;

        private bool _isLoading;

        public FeedCollection(IService<T> service)
        {
            foreach (var feed in AppDatabase.GetFeeds<T>())
            {
                Add(feed);
            }

            _feedSource = new FeedSource<T>(service);
        }

        public bool HasMoreItems => true;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoading)));
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (IsLoading)
            {
                return Task.FromResult(new LoadMoreItemsResult
                {
                    Count = 0
                }).AsAsyncOperation();
            }

            IsLoading = true;
            return AsyncInfo.Run(async c =>
            {
                try
                {
                    var beforLoadCount = Count;
                    await _feedSource.LoadMoreItemsAsync(this);
                    var afterLoadCount = Count;
                    uint loadCount = afterLoadCount >= beforLoadCount ? (uint)(afterLoadCount - beforLoadCount) : 0;
                    return new LoadMoreItemsResult
                    {
                        Count = loadCount
                    };
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }
    }
}