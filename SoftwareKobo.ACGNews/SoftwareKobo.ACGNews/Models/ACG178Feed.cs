using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;
using System.ComponentModel;

namespace SoftwareKobo.ACGNews.Models
{
    public class Acg178Feed : FeedBase
    {
        private DateTime _publishTime;

        private string _tag;

        private string _author;

        private string _originSource;

        public DateTime PublishTime
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

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                Set(ref _tag, value);
            }
        }

        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                Set(ref _author, value);
            }
        }

        public string OriginSource
        {
            get
            {
                return _originSource;
            }
            set
            {
                Set(ref _originSource, value);
            }
        }
    }
}