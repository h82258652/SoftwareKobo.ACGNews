namespace SoftwareKobo.ACGNews.Models
{
    public class Acg17173Feed : FeedBase
    {
        private string _thumbnail;

        private string _summary;

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

        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                Set(ref _summary, value);
            }
        }
    }
}