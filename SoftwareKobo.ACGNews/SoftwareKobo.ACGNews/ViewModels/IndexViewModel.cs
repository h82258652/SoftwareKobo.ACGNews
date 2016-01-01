using SoftwareKobo.ACGNews.DataModels;
using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using System.Collections;

namespace SoftwareKobo.ACGNews.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        public IndexViewModel()
        {
            AppSetting.Instance.CurrentChannelChanged += delegate
            {
                RaisePropertyChanged(nameof(Feeds));
            };
        }

        public IFeedCollection Feeds
        {
            get
            {
                return AppSetting.Instance.CurrentChannel.CreateFeedCollection();
            }
        }
    }
}