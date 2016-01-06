using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SoftwareKobo.ACGNews.Controls
{
    public sealed partial class ChannelItem : UserControl
    {
        public ChannelItem()
        {
            this.InitializeComponent();
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