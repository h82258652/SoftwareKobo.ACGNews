using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SoftwareKobo.ACGNews.Controls
{
    public sealed partial class Acg178Item
    {
        public Acg178Item()
        {
            InitializeComponent();
        }

        private void Thumbnail_ImageFailed(object sender, ExceptionRoutedEventArgs e)
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