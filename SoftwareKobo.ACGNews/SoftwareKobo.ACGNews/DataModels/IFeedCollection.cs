using System;
using System.Collections;

namespace SoftwareKobo.ACGNews.DataModels
{
    public interface IFeedCollection : IList
    {
        event EventHandler LoadMoreCompleted;

        event EventHandler LoadMoreStarted;

        bool IsLoading
        {
            get;
            set;
        }

        void Refresh();
    }
}