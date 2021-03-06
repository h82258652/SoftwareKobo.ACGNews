﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Controls
{
    public partial class AcgGamerskyItem
    {
        public AcgGamerskyItem()
        {
            InitializeComponent();
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