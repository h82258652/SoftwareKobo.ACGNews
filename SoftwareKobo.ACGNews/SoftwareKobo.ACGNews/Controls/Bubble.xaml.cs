using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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