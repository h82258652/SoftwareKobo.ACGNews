using SoftwareKobo.ACGNews.DataModels;
using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using System.Collections;

namespace SoftwareKobo.ACGNews.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        private IFeedCollection _feeds;

        public IndexViewModel()
        {
            Feeds = AppSetting.Instance.CurrentChannel.CreateFeedCollection();
        }

        public IFeedCollection Feeds
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