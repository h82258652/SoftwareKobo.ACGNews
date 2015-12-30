using System;

namespace SoftwareKobo.ACGNews.Controls
{
    public class PullProgressChangedEventArgs : EventArgs
    {
        internal PullProgressChangedEventArgs(double progress)
        {
            Progress = progress;
        }

        public double Progress
        {
            get;
            private set;
        }
    }
}