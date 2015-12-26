using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Controls
{
    public sealed partial class Acg17173Item
    {
        public Acg17173Item()
        {
            InitializeComponent();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
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