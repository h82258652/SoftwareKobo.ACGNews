using SoftwareKobo.ACGNews.Datas;
using SQLite.Net.Attributes;

namespace SoftwareKobo.ACGNews.Models
{
    public abstract class FeedBase : BindableBase
    {
        private string _title;

        private string _detailLink;

        private bool _hasRead;

        [PrimaryKey]
        public long Id
        {
            get;
            set;
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                Set(ref _title, value);
            }
        }

        public string DetailLink
        {
            get
            {
                return _detailLink;
            }
            set
            {
                Set(ref _detailLink, value);
            }
        }

        public bool HasRead
        {
            get
            {
                return _hasRead;
            }
            set
            {
                //if (_hasRead == value)
                //{
                //    return;
                //}

                Set(ref _hasRead, value);
                //MarkAsReaded();
            }
        }

        private async void MarkAsReaded()
        {
            await AppDatabase.InsertOrUpdateFeedAsync(this);
        }
    }
}