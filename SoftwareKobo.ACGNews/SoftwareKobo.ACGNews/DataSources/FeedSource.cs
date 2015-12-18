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

        private bool _firstLoad;

        public async Task LoadMoreItemsAsync(IList<T> list)
        {
            if (_firstLoad == false)
            {
                _firstLoad = true;
            }

            List<T> feeds;
            try
            {
                feeds = (await _service.GetAsync(_currentPage)).ToList();
                _currentPage++;
            }
            catch
            {
                return;
            }

            CacheFeeds(feeds);

            foreach (var feed in feeds)
            {
                if (list.Count == 0)
                {
                    list.Insert(0, feed);
                }
                else
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        var temp = list[i];
                        if (temp.SortId == feed.SortId)
                        {
                            break;
                        }
                        if (temp.SortId < feed.SortId)
                        {
                            list.Insert(i, feed);
                            break;
                        }
                        if (i == list.Count - 1)
                        {
                            list.Add(feed);
                        }
                    }
                }
            }
        }

        private static async void CacheFeeds(IEnumerable<T> feeds)
        {
            await Task.Run(() =>
            {
                AppDatabase.InsertOrUpdateFeeds(feeds);
            });
        }
    }
}