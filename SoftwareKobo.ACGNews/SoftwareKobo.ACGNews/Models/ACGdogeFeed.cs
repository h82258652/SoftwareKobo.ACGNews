using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System.ComponentModel;

namespace SoftwareKobo.ACGNews.Models
{
    public class AcgdogeFeed : FeedBase
    {
        private string _publishTime;

        private int _commentCount;

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

        public int CommentCount
        {
            get
            {
                return _commentCount;
            }
            set
            {
                Set(ref _commentCount, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DatabaseCategories
        {
            get;
            set;
        }

        [Ignore]
        public string[] Categories
        {
            get
            {
                return DatabaseCategories == null ? null : JsonConvert.DeserializeObject<string[]>(DatabaseCategories);
            }
            set
            {
                DatabaseCategories = JsonConvert.SerializeObject(value);
                RaisePropertyChanged();
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