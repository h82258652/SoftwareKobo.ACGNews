using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System.ComponentModel;

namespace SoftwareKobo.ACGNews.Models
{
    public class TencentComicFeed : FeedBase
    {
        private string _summary;

        private string _publishTime;

        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                Set(ref _summary, value);
            }
        }

        public string PublishTime
        {
            get
            {
                return _publishTime;
            }
            set
            {
                Set(ref _publishTime, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DatabaseTags
        {
            get;
            set;
        }

        [Ignore]
        public string[] Tags
        {
            get
            {
                return DatabaseTags == null ? null : JsonConvert.DeserializeObject<string[]>(DatabaseTags);
            }
            set
            {
                DatabaseTags = JsonConvert.SerializeObject(value);
                RaisePropertyChanged();
            }
        }
    }
}