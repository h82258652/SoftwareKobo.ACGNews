namespace SoftwareKobo.ACGNews.Models
{
    public class ACGdogeFeed
    {
        public string Title
        {
            get;
            set;
        }

        public string DetailLink
        {
            get;
            set;
        }

        public string Thumbnail
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }

        public string PublishTime
        {
            get;
            set;
        }

        public int CommentCount
        {
            get;
            set;
        }

        public string[] Categories
        {
            get;
            set;
        }

        public string[] Tags
        {
            get;
            set;
        }
    }
}