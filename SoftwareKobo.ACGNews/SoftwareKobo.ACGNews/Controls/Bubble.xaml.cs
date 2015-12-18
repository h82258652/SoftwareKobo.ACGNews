using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SoftwareKobo.ACGNews.Controls
{
    public sealed partial class Bubble : UserControl
    {
        public Bubble()
        {
            this.InitializeComponent();
        }

        private void Bubble_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = this.GetFirstAncestorOfType<BubblePanel>();
            if (panel != null)
            {
                var panelWidth = panel.ActualWidth;
                Translate.X = App.GlobalRand.NextDouble() * panelWidth - panelWidth / 2;
                var scale = App.GlobalRand.NextDouble() + 0.25;
                Scale.ScaleX = scale;
                Scale.ScaleY = scale;
            }
        }
    }
}