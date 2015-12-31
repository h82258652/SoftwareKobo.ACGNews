using SQLite.Net.Attributes;

namespace SoftwareKobo.ACGNews.DatabaseModels
{
    public class ArticleEntity
    {
        [PrimaryKey]
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