using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace SoftwareKobo.ACGNews.Controls
{
    public sealed partial class BubblePanel
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public BubblePanel()
        {
            InitializeComponent();
        }

        private void AddBubble()
        {
            var bubble = new Bubble
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            RootGrid.Children.Add(bubble);

            var animation = new DoubleAnimation
            {
                From = 100,
                To = -100 - ActualHeight,
                Duration = TimeSpan.FromSeconds(App.GlobalRand.NextDouble() * 15 + 5)
            };
            Storyboard.SetTarget(animation, bubble.Translate);
            Storyboard.SetTargetProperty(animation, "Y");

            var storyboard = new Storyboard();

            storyboard.Children.Add(animation);

            EventHandler<object> handler = null;
            handler = (sender, e) =>
            {
                storyboard.Completed -= handler;
                RootGrid.Children.Remove(bubble);
            };
            storyboard.Completed += handler;
            storyboard.Begin();
        }

        private void BubblePanel_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            _timer.Interval = TimeSpan.FromSeconds(App.GlobalRand.NextDouble() * 3 + 1);
            AddBubble();
        }
    }
}