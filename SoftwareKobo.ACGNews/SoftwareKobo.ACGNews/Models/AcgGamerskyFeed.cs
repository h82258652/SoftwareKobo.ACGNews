using System;

namespace SoftwareKobo.ACGNews.Models
{
    public class AcgGamerskyFeed : FeedBase
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

        public DateTime PublishTime
        {
            get;
            set;
        }

        public int ViewCount
        {
            get;
            set;
        }

        public int CommentCount
        {
            get;
            set;
        }
    }
}