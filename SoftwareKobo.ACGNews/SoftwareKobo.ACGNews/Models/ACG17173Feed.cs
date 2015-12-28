namespace SoftwareKobo.ACGNews.Models
{
    public class Acg17173Feed : FeedBase
    {
        private string _summary;

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