using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Views
{
    public sealed partial class DetailView
    {
        public DetailView()
        {
            InitializeComponent();
        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var value = e.Value;
            if (value == "goback")
            {
                Hide();
            }
        }

        public async Task SetContentAsync(string content)
        {
            await WebView.InvokeScriptAsync("setContent", new[]
            {
                content
            });
        }

        private void BtnShare_Click(object sender, RoutedEventArgs e)
        {
        }

        public void Show()
        {
            VisualStateManager.GoToState(this, "ShowState", true);
        }

        public void Hide()
        {
            VisualStateManager.GoToState(this, "HideState", true);
        }
    }
}