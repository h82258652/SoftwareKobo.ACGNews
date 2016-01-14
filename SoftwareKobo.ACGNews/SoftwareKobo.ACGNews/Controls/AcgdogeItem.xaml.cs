using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Controls
{
    public partial class AcgdogeItem
    {
        public AcgdogeItem()
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

        private async void Thumbnail_Failed(object sender, ExceptionRoutedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
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