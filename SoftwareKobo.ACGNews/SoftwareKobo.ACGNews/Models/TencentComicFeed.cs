namespace SoftwareKobo.ACGNews.Models
{
    public class TencentComicFeed : FeedBase
    {
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

        public string[] Tags
        {
            get;
            set;
        }
    }
}