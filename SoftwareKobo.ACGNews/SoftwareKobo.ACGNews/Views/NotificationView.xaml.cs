using System;
using Windows.UI.Xaml;

namespace SoftwareKobo.ACGNews.Views
{
    public sealed partial class NotificationView
    {
        public static NotificationView Instance
        {
            get;
            private set;
        }

        public NotificationView()
        {
            InitializeComponent();
            Instance = this;
        }

        public static void ShowLoading(string message)
        {
            Instance.LoadingProgressBar.Visibility = Visibility.Visible;
            Instance.LoadingMessage.Text = message;
        }

        public static void HideLoading()
        {
            Instance.LoadingProgressBar.Visibility = Visibility.Collapsed;
            Instance.LoadingMessage.Text = string.Empty;
        }

        private void LocalTime_OnLoaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += delegate
            {
                LocalTime.Text = DateTime.Now.ToString("t");
            };
            timer.Start();
        }
    }
}