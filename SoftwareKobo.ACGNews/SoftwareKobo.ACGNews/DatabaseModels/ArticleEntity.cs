using SQLite.Net.Attributes;

namespace SoftwareKobo.ACGNews.DatabaseModels
{
    public class ArticleEntity
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id
        {
            get;
            set;
        }

        [Unique]
        public string Url
        {
            get;
            set;
        }

        public string Html
        {
            get;
            set;
        }
    }
}