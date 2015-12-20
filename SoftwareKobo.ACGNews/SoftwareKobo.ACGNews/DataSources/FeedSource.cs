using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using System;
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

        public async Task LoadMoreItemsAsync(IList<T> list)
        {
            var networkTask = LoadNetworkFeedsAsync(list);
            var cacheTask = LoadCacheFeedsAsync(list);
            await Task.WhenAll(networkTask, cacheTask);

            //List<T> networkFeeds;
            //try
            //{
            //    networkFeeds = (await _service.GetAsync(_currentPage)).ToList();
            //    foreach (var networkFeed in networkFeeds)
            //    {
            //        Merge(list, networkFeed);
            //    }
            //    _currentPage++;
            //}
            //catch
            //{
            //    networkFeeds = new List<T>();
            //}

            //List<T> cacheFeeds;
            //var lastFeed = list.LastOrDefault();
            //if (lastFeed == null)
            //{
            //    // 网络加载失败。
            //    cacheFeeds = AppDatabase.GetFeeds<T>(30).ToList();
            //}
            //else
            //{
            //    cacheFeeds = AppDatabase.GetFeeds<T>(30, lastFeed.Id).ToList();
            //}

            //foreach (var cacheFeed in cacheFeeds)
            //{
            //    Merge(list, cacheFeed);
            //}

            //if (networkFeeds.Count > 0)
            //{
            //    AppDatabase.InsertOrUpdateFeeds(networkFeeds);
            //}
        }

        private async Task LoadNetworkFeedsAsync(IList<T> list)
        {
            List<T> networkFeeds;
            try
            {
                networkFeeds = (await _service.GetAsync(_currentPage)).ToList();
                foreach (var networkFeed in networkFeeds)
                {
                    Merge(list, networkFeed);
                }
            }
            catch
            {
                networkFeeds = new List<T>();
            }

            #region
            // save to db.
            #endregion
        }

        private async Task LoadCacheFeedsAsync(IList<T> list)
        {
            var lastFeed = list.LastOrDefault();
            var cacheFeeds = await AppDatabase.GetFeedsAsync<T>(30, lastFeed?.Id);
            foreach (var cacheFeed in cacheFeeds)
            {
                Merge(list, cacheFeed);
            }
        }

        private async void CacheNetworkFeeds(IList<T> networkFeeds)
        {
        }
    }
}