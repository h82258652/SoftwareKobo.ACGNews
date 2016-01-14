using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Controls
{
    public partial class Acg178Item
    {
        public Acg178Item()
        {
            InitializeComponent();
            TitleTextBlock.RegisterPropertyChangedCallback(TextBlock.LineHeightProperty, (sender, dp) =>
            {
                UnreadIndicator.Height = TitleTextBlock.LineHeight + 8;
                HasReadIndicator.Height = TitleTextBlock.LineHeight + 8;
            });
            UnreadIndicator.Height = TitleTextBlock.LineHeight + 8;
            HasReadIndicator.Height = TitleTextBlock.LineHeight + 8;
        }

        private void Thumbnail_Failed(object sender, ExceptionRoutedEventArgs e)
        {
            var img = (Image)sender;
            var binding = img.GetBindingExpression(Image.SourceProperty);
            if (binding != null)
            {
                img.SetBinding(Image.SourceProperty, binding.ParentBinding);
            }
            else
            {
                var temp = img.Source;
                img.Source = null;
                img.Source = temp;
            }
        }
    }
}