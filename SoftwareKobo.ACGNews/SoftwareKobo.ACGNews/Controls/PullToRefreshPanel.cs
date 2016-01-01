using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Controls
{
    [TemplatePart(Name = PartContentContainer, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PartIndicatorContainer, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PartScrollViewer, Type = typeof(ScrollViewer))]
    public class PullToRefreshPanel : ContentControl
    {
        public const string IndicatorPullToRefreshStateName = "PullToRefresh";

        public const string IndicatorRefreshingStateName = "Refreshing";

        public const string IndicatorReleaseToRefreshStateName = "ReleaseToRefresh";

        public const string IndicatorVisualStateGroupName = "IndicatorStates";

        public static readonly DependencyProperty IndicatorProperty = DependencyProperty.Register(nameof(Indicator), typeof(object), typeof(PullToRefreshPanel), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty PullProgressProperty = DependencyProperty.Register(nameof(PullProgress), typeof(double), typeof(PullToRefreshPanel), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty RefreshThresholdProperty = DependencyProperty.Register(nameof(RefreshThreshold), typeof(double), typeof(PullToRefreshPanel), new PropertyMetadata(default(double)));

        private const string PartContentContainer = "PART_ContentContainer";

        private const string PartIndicatorContainer = "PART_IndicatorContainer";

        private const string PartScrollViewer = "PART_ScrollViewer";

        private FrameworkElement _contentContainer;

        private FrameworkElement _indicatorContainer;

        private ScrollViewer _scrollViewer;

        public PullToRefreshPanel()
        {
            DefaultStyleKey = typeof(PullToRefreshPanel);
            Loaded += delegate
            {
                UpdateContainerLayout();
            };
            SizeChanged += delegate
            {
                UpdateContainerLayout();
            };
        }

        public event EventHandler<PullProgressChangedEventArgs> PullProgressChanged;

        public event EventHandler<RefreshRequestedEventArgs> RefreshRequested;

        public object Indicator
        {
            get
            {
                return GetValue(IndicatorProperty);
            }
            set
            {
                SetValue(IndicatorProperty, value);
            }
        }

        public double PullProgress
        {
            get
            {
                return (double)GetValue(PullProgressProperty);
            }
            private set
            {
                SetValue(PullProgressProperty, value);
            }
        }

        public double RefreshThreshold
        {
            get
            {
                return (double)GetValue(RefreshThresholdProperty);
            }
            set
            {
                SetValue(RefreshThresholdProperty, value);
            }
        }

        protected override void OnApplyTemplate()
        {
            _scrollViewer = (ScrollViewer)GetTemplateChild(PartScrollViewer);
            _contentContainer = (FrameworkElement)GetTemplateChild(PartContentContainer);
            _indicatorContainer = (FrameworkElement)GetTemplateChild(PartIndicatorContainer);

            _scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var indicatorContainerHeight = _indicatorContainer.ActualHeight;
            var scrollViewerVerticalOffset = _scrollViewer.VerticalOffset;

            var progress = Math.Max(0, Math.Min(1, (indicatorContainerHeight - scrollViewerVerticalOffset) / RefreshThreshold));
            PullProgress = progress;
            PullProgressChanged?.Invoke(this, new PullProgressChangedEventArgs(progress));

            if (e.IsIntermediate)
            {
                SetIndicatorVisualState(scrollViewerVerticalOffset <= indicatorContainerHeight - RefreshThreshold ? IndicatorReleaseToRefreshStateName : IndicatorPullToRefreshStateName);
            }
            else
            {
                if (scrollViewerVerticalOffset - 1 <= indicatorContainerHeight - RefreshThreshold)
                {
                    // 到达顶部然后松手，VerticalOffset 会是一个非常接近 0 的浮点数。

                    SetIndicatorVisualState(IndicatorRefreshingStateName);

                    if (RefreshRequested != null)
                    {
                        var args = new RefreshRequestedEventArgs(() =>
                        {
                            _scrollViewer.ChangeView(null, _indicatorContainer.ActualHeight, null);
                        });
                        RefreshRequested(this, args);
                        if (args.IsDeferred == false)
                        {
                            _scrollViewer.ChangeView(null, indicatorContainerHeight, null);
                        }
                    }
                    else
                    {
                        _scrollViewer.ChangeView(null, indicatorContainerHeight, null);
                    }
                }
                else
                {
                    SetIndicatorVisualState(IndicatorPullToRefreshStateName);

                    _scrollViewer.ChangeView(null, indicatorContainerHeight, null);
                }
            }
        }

        private void SetIndicatorVisualState(string stateName)
        {
            var indicator = Indicator as Control;
            if (indicator != null)
            {
                VisualStateManager.GoToState(indicator, stateName, true);
            }
        }

        private void UpdateContainerLayout()
        {
            if (_scrollViewer != null && _indicatorContainer != null)
            {
                _scrollViewer.ChangeView(null, _indicatorContainer.ActualHeight, null);
            }
            if (_contentContainer != null)
            {
                _contentContainer.Height = ActualHeight;
            }
        }
    }
}