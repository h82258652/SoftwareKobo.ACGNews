using SoftwareKobo.ACGNews.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Controls
{
    public sealed partial class Acg17173Item : UserControl
    {
        public Acg17173Item()
        {
            this.InitializeComponent();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var feed = (Acg17173Feed)DataContext;
            feed.RaisePropertyChanged(nameof(feed.Thumbnail));
        }
    }
}