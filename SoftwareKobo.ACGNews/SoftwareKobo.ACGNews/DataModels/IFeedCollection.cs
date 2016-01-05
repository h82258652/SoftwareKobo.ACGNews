using System;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace SoftwareKobo.ACGNews.DataModels
{
    public interface IFeedCollection : IList, ISupportIncrementalLoading
    {
        event EventHandler LoadMoreCompleted;

        event EventHandler LoadMoreStarted;

        bool IsLoading
        {
            get;
        }

        void Refresh();
    }
}