using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace SoftwareKobo.ACGNews.Controls
{
    [TemplatePart(Name = PartEllipse, Type = typeof(Ellipse))]
    [TemplatePart(Name = PartMessage, Type = typeof(TextBlock))]
    [TemplateVisualState(GroupName = PullToRefreshPanel.IndicatorVisualStateGroupName, Name = PullToRefreshPanel.IndicatorPullToRefreshStateName)]
    [TemplateVisualState(GroupName = PullToRefreshPanel.IndicatorVisualStateGroupName, Name = PullToRefreshPanel.IndicatorReleaseToRefreshStateName)]
    [TemplateVisualState(GroupName = PullToRefreshPanel.IndicatorVisualStateGroupName, Name = PullToRefreshPanel.IndicatorRefreshingStateName)]
    public class PullToRefreshIndicator : Control
    {
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(PullToRefreshIndicator), new PropertyMetadata(default(double), ProcessChanged));

        private const string PartEllipse = "PART_Ellipse";

        private const string PartMessage = "PART_Message";

        private Ellipse _ellipse;

        public PullToRefreshIndicator()
        {
            DefaultStyleKey = typeof(PullToRefreshIndicator);
        }

        public double Progress
        {
            get
            {
                return (double)GetValue(ProgressProperty);
            }
            set
            {
                SetValue(ProgressProperty, value);
            }
        }

        protected override void OnApplyTemplate()
        {
            _ellipse = (Ellipse)GetTemplateChild(PartEllipse);
            _ellipse.SizeChanged += delegate
            {
                SetEllipseStroke();
            };
            SetEllipseStroke();
        }

        private static void ProcessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (PullToRefreshIndicator)d;
            obj.SetEllipseStroke();
        }

        private double CalculateEllipseCircumference()
        {
            var width = _ellipse.ActualWidth;
            var height = _ellipse.ActualHeight;

            var a = Math.Max(width, height) / 2;
            var b = Math.Min(width, height) / 2;

            return Math.PI * b + 2 * (a - b);
        }

        private void SetEllipseStroke()
        {
            if (_ellipse != null)
            {
                var circumference = CalculateEllipseCircumference();
                _ellipse.StrokeDashArray = new DoubleCollection
                {
                    circumference * Progress,
                    double.MaxValue
                };
            }
        }
    }
}