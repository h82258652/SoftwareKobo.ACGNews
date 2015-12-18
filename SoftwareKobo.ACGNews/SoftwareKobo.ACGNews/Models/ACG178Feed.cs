using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;
using System.ComponentModel;

namespace SoftwareKobo.ACGNews.Models
{
    public class Acg178Feed : FeedBase
    {
        private string _thumbnail;

        private string _summary;

        public string Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                Set(ref _thumbnail, value);
            }
        }

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

        public DateTime PublishTime
        {
            get;
            set;
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
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string OriginSource
        {
            get;
            set;
        }
    }
}