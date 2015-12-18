using SoftwareKobo.ACGNews.Models;
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