using SoftwareKobo.ACGNews.DataModels;
using SoftwareKobo.ACGNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareKobo.ACGNews.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        public FeedCollection<FeedBase> Feeds
        {
            get;
            set;
        }
    }
}