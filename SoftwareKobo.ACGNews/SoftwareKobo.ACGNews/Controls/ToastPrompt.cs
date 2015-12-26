using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using WinRTXamlToolkit.AwaitableUI;

namespace SoftwareKobo.ACGNews.Controls
{
    public class ToastPrompt : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ToastPrompt), new PropertyMetadata(default(CornerRadius)));

        public static readonly DependencyProperty SlideInDirectionProperty = DependencyProperty.Register(nameof(SlideInDirection), typeof(SlideInDirection), typeof(ToastPrompt), new PropertyMetadata(SlideInDirection.Right, SlideInDirectionChanged));

        private static readonly DependencyProperty ContainerMarginProperty = DependencyProperty.Register(nameof(ContainerMargin), typeof(Thickness), typeof(ToastPrompt), new PropertyMetadata(default(Thickness)));
        private static readonly DependencyProperty ContainerOpacityProperty = DependencyProperty.Register(nameof(ContainerOpacity), typeof(double), typeof(ToastPrompt), new PropertyMetadata(0));

        private static readonly DependencyProperty ContainerPaddingProperty = DependencyProperty.Register(nameof(ContainerPadding), typeof(Thickness), typeof(ToastPrompt), new PropertyMetadata(default(Thickness)));

        private static readonly DependencyProperty RootGridClipProperty = DependencyProperty.Register(nameof(RootGridClip), typeof(RectangleGeometry), typeof(ToastPrompt), new PropertyMetadata(null));

        private readonly TranslateTransform _translate = new TranslateTransform();

        public ToastPrompt()
        {
            DefaultStyleKey = typeof(ToastPrompt);
            SlideInDirection = SlideInDirection.Right;
            SetRootGridClip();
            SizeChanged += delegate
            {
                SetRootGridClip();
            };
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        public SlideInDirection SlideInDirection
        {
            get
            {
                return (SlideInDirection)GetValue(SlideInDirectionProperty);
            }
            set
            {
                SetValue(SlideInDirectionProperty, value);
            }
        }

        private Thickness ContainerMargin
        {
            get
            {
                return (Thickness)GetValue(ContainerMarginProperty);
            }
            set
            {
                SetValue(ContainerMarginProperty, value);
            }
        }

        private double ContainerOpacity
        {
            get
            {
                return (double)GetValue(ContainerOpacityProperty);
            }
            set
            {
                SetValue(ContainerOpacityProperty, value);
            }
        }

        private Thickness ContainerPadding
        {
            get
            {
                return (Thickness)GetValue(ContainerPaddingProperty);
            }
            set
            {
                SetValue(ContainerPaddingProperty, value);
            }
        }

        private RectangleGeometry RootGridClip
        {
            get
            {
                return (RectangleGeometry)GetValue(RootGridClipProperty);
            }
            set
            {
                SetValue(RootGridClipProperty, value);
            }
        }

        public async Task ShowAsync(double seconds = 1, double slideInDistance = 300)
        {
            Storyboard storyboard = new Storyboard();

            {
                var animation = new DoubleAnimationUsingKeyFrames()
                {
                    EnableDependentAnimation = true
                };
                Storyboard.SetTarget(animation, this);
                Storyboard.SetTargetProperty(animation, nameof(ContainerOpacity));
                animation.KeyFrames.Add(new LinearDoubleKeyFrame()
                {
                    KeyTime = TimeSpan.FromSeconds(0),
                    Value = 0
                });
                animation.KeyFrames.Add(new LinearDoubleKeyFrame()
                {
                    KeyTime = TimeSpan.FromSeconds(0.3),
                    Value = 1
                });
                animation.KeyFrames.Add(new LinearDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromSeconds(seconds + 0.3),
                    Value = 1
                });
                animation.KeyFrames.Add(new LinearDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromSeconds(seconds + 0.6),
                    Value = 0
                });
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(animation, _translate);
                switch (SlideInDirection)
                {
                    case SlideInDirection.Right:
                    case SlideInDirection.Left:
                        Storyboard.SetTargetProperty(animation, "X");
                        break;

                    case SlideInDirection.Top:
                    case SlideInDirection.Bottom:
                        Storyboard.SetTargetProperty(animation, "Y");
                        break;
                }

                switch (SlideInDirection)
                {
                    case SlideInDirection.Right:
                    case SlideInDirection.Bottom:
                        animation.KeyFrames.Add(new EasingDoubleKeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(0),
                            Value = slideInDistance
                        });
                        break;

                    case SlideInDirection.Left:
                    case SlideInDirection.Top:
                        animation.KeyFrames.Add(new EasingDoubleKeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(0),
                            Value = 0 - slideInDistance
                        });
                        break;
                }

                animation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromSeconds(0.3),
                    Value = 0,
                    EasingFunction = new BackEase
                    {
                        EasingMode = EasingMode.EaseOut,
                        Amplitude = 0.5
                    }
                });

                animation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromSeconds(seconds + 0.3),
                    Value = 0
                });

                switch (SlideInDirection)
                {
                    case SlideInDirection.Right:
                    case SlideInDirection.Bottom:
                        animation.KeyFrames.Add(new EasingDoubleKeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(seconds + 0.6),
                            Value = slideInDistance,
                            EasingFunction = new BackEase
                            {
                                EasingMode = EasingMode.EaseIn,
                                Amplitude = 0.5
                            }
                        });
                        break;

                    case SlideInDirection.Left:
                    case SlideInDirection.Top:
                        animation.KeyFrames.Add(new EasingDoubleKeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(seconds + 0.6),
                            Value = 0 - slideInDistance,
                            EasingFunction = new BackEase
                            {
                                EasingMode = EasingMode.EaseIn,
                                Amplitude = 0.5
                            }
                        });
                        break;
                }

                storyboard.Children.Add(animation);
            }

            await storyboard.BeginAsync();
        }

        protected override void OnApplyTemplate()
        {
            var container = (UIElement)GetTemplateChild("PART_Container");
            container.RenderTransform = _translate;
        }

        private static void SlideInDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ToastPrompt)d;
            var value = (SlideInDirection)e.NewValue;
            switch (value)
            {
                case SlideInDirection.Right:
                    obj.ContainerMargin = new Thickness(0, 0, -100, 0);
                    obj.ContainerPadding = new Thickness(0, 0, 100, 0);
                    break;

                case SlideInDirection.Left:
                    obj.ContainerMargin = new Thickness(-100, 0, 0, 0);
                    obj.ContainerPadding = new Thickness(100, 0, 0, 0);
                    break;

                case SlideInDirection.Top:
                    obj.ContainerMargin = new Thickness(0, -100, 0, 0);
                    obj.ContainerPadding = new Thickness(0, 100, 0, 0);
                    break;

                case SlideInDirection.Bottom:
                    obj.ContainerMargin = new Thickness(0, 0, 0, -100);
                    obj.ContainerPadding = new Thickness(0, 0, 0, 100);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(SlideInDirection));
            }
            obj.SetRootGridClip();
        }

        private void SetRootGridClip()
        {
            switch (SlideInDirection)
            {
                case SlideInDirection.Right:
                    RootGridClip = new RectangleGeometry()
                    {
                        Rect = new Rect(-100, 0, ActualWidth + 100, ActualHeight)
                    };
                    break;

                case SlideInDirection.Left:
                    RootGridClip = new RectangleGeometry()
                    {
                        Rect = new Rect(0, 0, ActualWidth + 100, ActualHeight)
                    };
                    break;

                case SlideInDirection.Top:
                    RootGridClip = new RectangleGeometry()
                    {
                        Rect = new Rect(0, -100, ActualWidth, ActualHeight + 100)
                    };
                    break;

                case SlideInDirection.Bottom:
                    RootGridClip = new RectangleGeometry()
                    {
                        Rect = new Rect(0, 0, ActualWidth, ActualHeight + 100)
                    };
                    break;
            }
        }
    }
}