using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.Extensions;

namespace SoftwareKobo.ACGNews.Controls
{
    public partial class Bubble
    {
        public Bubble()
        {
            InitializeComponent();
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