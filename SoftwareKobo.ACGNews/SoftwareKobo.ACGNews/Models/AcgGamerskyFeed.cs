using System;

namespace SoftwareKobo.ACGNews.Models
{
    public class AcgGamerskyFeed : FeedBase
    {
        private DateTime _publishTime;

        private int _viewCount;

        private int _commentCount;

        public DateTime PublishTime
        {
            get
            {
                return _publishTime;
            }
            set
            {
                Set(ref _publishTime, value);
            }
        }

        public int ViewCount
        {
            get
            {
                return _viewCount;
            }
            set
            {
                Set(ref _viewCount, value);
            }
        }

        public int CommentCount
        {
            get
            {
                return _commentCount;
            }
            set
            {
                Set(ref _commentCount, value);
            }
        }
    }
}