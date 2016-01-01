using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareKobo.ACGNews.DataSources
{
    public class FeedSource<T> where T : FeedBase
    {
        private readonly IService<T> _service;

        private int _currentPage;

        public FeedSource(IService<T> service)
        {
            _service = service;
        }

        public async Task LoadMoreItemsAsync(IList<T> list)
        {
            // 同时呼起两个任务。
            var networkTask = LoadNetworkFeedsAsync();
            var cacheFeeds = await LoadCacheFeedsAsync(list);

            // 先将 Cache 合并。
            Merge(list, cacheFeeds);

            // 再合并 Network。
            var networkFeeds = await networkTask;
            Merge(list, networkFeeds);

            // 缓存网络数据。
            await CacheNetworkFeeds(networkFeeds);
        }

        public void Refresh()
        {
            _currentPage = 0;
        }

        private static void Merge(IList<T> list, T feed)
        {
            if (list.Count <= 0)
            {
                // 直接加入。
                list.Insert(0, feed);
            }
            else
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var temp = list[i];
                    if (temp.Id == feed.Id)
                    {
                        // 已经存在。
                        break;
                    }
                    if (temp.Id < feed.Id)
                    {
                        // 插入。
                        list.Insert(i, feed);
                        break;
                    }
                    if (i == list.Count - 1)
                    {
                        // 最后一个。
                        list.Add(feed);
                    }
                }
            }
        }

        private static void Merge(IList<T> list, IEnumerable<T> feeds)
        {
            foreach (var feed in feeds)
            {
                Merge(list, feed);
            }
        }

        private async Task CacheNetworkFeeds(IList<T> networkFeeds)
        {
            await AppDatabase.InsertOrUpdateFeedsAsync(networkFeeds);
        }

        private async Task<List<T>> LoadCacheFeedsAsync(IList<T> list)
        {
            var lastFeed = list.LastOrDefault();
            var cacheFeeds = await AppDatabase.GetFeedsAsync<T>(30, lastFeed?.Id);
            return cacheFeeds;
        }

        private async Task<List<T>> LoadNetworkFeedsAsync()
        {
            List<T> networkFeeds;
            try
            {
                networkFeeds = (await _service.GetAsync(_currentPage)).ToList();
                _currentPage++;
            }
            catch
            {
                networkFeeds = new List<T>();
            }
            return networkFeeds;
        }
    }
}