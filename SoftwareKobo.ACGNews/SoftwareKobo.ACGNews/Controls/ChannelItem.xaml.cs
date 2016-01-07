using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SoftwareKobo.ACGNews.Controls
{
    public partial class ChannelItem
    {
        public ChannelItem()
        {
            InitializeComponent();
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Polygon.Points = new PointCollection()
            {
                new Point(0,0),
                new Point(e.NewSize.Height,0),
                new Point(0,e.NewSize.Height)
            };
        }
    }
}