using System;

namespace SoftwareKobo.ACGNews.Models
{
    public class ACG178Feed
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

        public DateTime PublishTime
        {
            get;
            set;
        }

        public string[] Categories
        {
            get;
            set;
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