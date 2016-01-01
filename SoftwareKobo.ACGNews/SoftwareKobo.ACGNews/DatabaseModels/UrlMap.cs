using SQLite.Net.Attributes;
using System;

namespace SoftwareKobo.ACGNews.DatabaseModels
{
    public class UrlMap
    {
        [PrimaryKey]
        public string Url
        {
            get;
            set;
        }

        public Guid MapId
        {
            get;
            set;
        }
    }
}