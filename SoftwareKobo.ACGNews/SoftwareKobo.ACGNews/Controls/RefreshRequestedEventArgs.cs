using System;
using Windows.Foundation;

namespace SoftwareKobo.ACGNews.Controls
{
    public class RefreshRequestedEventArgs : EventArgs
    {
        private readonly DeferralCompletedHandler _handler;

        internal RefreshRequestedEventArgs(DeferralCompletedHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handler = handler;
        }

        internal bool IsDeferred
        {
            get;
            private set;
        }

        public Deferral GetDeferral()
        {
            IsDeferred = true;
            return new Deferral(_handler);
        }
    }
}