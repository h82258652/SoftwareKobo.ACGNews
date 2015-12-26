using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using System.Collections;

namespace SoftwareKobo.ACGNews.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        private IList _feeds;

        public IndexViewModel()
        {
            Feeds = AppSetting.Instance.CurrentChannel.CreateFeedCollection();
        }

        public IList Feeds
        {
            get
            {
                return _feeds;
            }
            set
            {
                Set(ref _feeds, value);
            }
        }
    }
}