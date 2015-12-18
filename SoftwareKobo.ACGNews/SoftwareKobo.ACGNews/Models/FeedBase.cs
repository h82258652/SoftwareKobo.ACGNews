using SQLite.Net.Attributes;

namespace SoftwareKobo.ACGNews.Models
{
    public abstract class FeedBase : BindableBase
    {
        private int _databaseId;

        private long _sortId;

        private string _title;

        private string _detailLink;

        [PrimaryKey]
        [AutoIncrement]
        public int DatabaseId
        {
            get
            {
                return _databaseId;
            }
            set
            {
                Set(ref _databaseId, value);
            }
        }

        public long SortId
        {
            get
            {
                return _sortId;
            }
            set
            {
                Set(ref _sortId, value);
            }
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
    }
}