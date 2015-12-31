using SoftwareKobo.ACGNews.Datas;
using SQLite.Net.Attributes;

namespace SoftwareKobo.ACGNews.Models
{
    public abstract class FeedBase : BindableBase
    {
        private string _title;

        private string _detailLink;

        private string _thumbnail;

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

        public bool HasRead
        {
            get
            {
                return _hasRead;
            }
            set
            {
                // 请改为调用 MarkAsReaded，set 方法仅被数据库和 MarkAsReaded 方法使用。
                Set(ref _hasRead, value);
            }
        }

        public virtual async void MarkAsReaded()
        {
            if (HasRead == false)
            {
                HasRead = true;
                await AppDatabase.InsertOrUpdateFeedAsync(this);
            }
            HasRead = true;
        }
    }
}