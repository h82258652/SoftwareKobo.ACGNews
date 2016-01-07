using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Services;
using SoftwareKobo.ACGNews.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.Views
{
    public partial class ChannelView
    {
        public ChannelView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ChannelViewModel;
            var t = vm?.Channels;
            Debugger.Break();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            AppSetting.Instance.CurrentChannel = (Channel)e.ClickedItem;
        }
    }
}