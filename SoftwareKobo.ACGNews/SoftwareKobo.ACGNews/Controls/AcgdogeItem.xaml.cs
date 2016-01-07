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