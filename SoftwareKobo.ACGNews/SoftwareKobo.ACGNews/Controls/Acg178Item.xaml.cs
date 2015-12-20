﻿using System;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SoftwareKobo.ACGNews.Controls
{
    public sealed partial class Acg178Item : UserControl
    {
        public Acg178Item()
        {
            this.InitializeComponent();
        }

        private void Thumbnail_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = (Image)sender;
            var bind = img.GetBindingExpression(Image.SourceProperty);
            Debugger.Break();
        }
    }
}