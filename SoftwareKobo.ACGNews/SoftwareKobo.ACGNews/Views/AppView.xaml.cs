using SoftwareKobo.ACGNews.Models;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SoftwareKobo.ACGNews.Views
{
    public partial class AppView
    {
        public AppView()
        {
            InitializeComponent();
            Instance = this;
        }

        public static AppView Instance
        {
            get;
            private set;
        }

        public Page CurrentView
        {
            get
            {
                if (Detail.Visibility == Visibility.Visible)
                {
                    return Detail;
                }
                if (FlipView.SelectedIndex == 1)
                {
                    return Index;
                }
                if (FlipView.SelectedIndex == 0)
                {
                    return Channel;
                }
                if (FlipView.SelectedIndex == 2)
                {
                    return Setting;
                }
                return null;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.BackPressedEventArgs"))
            {
                if (Detail.IsFullScreen)
                {
                    e.Handled = true;
                    await Detail.ExitFullScreen();
                    return;
                }
                if (Detail.Visibility == Visibility.Visible)
                {
                    e.Handled = true;
                    Detail.Hide();
                    return;
                }
                if (FlipView.SelectedIndex == 0 || FlipView.SelectedIndex == 2)
                {
                    e.Handled = true;
                    FlipView.SelectedIndex = 1;
                    return;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        public Task NavigateToDetail(FeedBase feed, string detail, Point? showAnimateCenter = null)
        {
            return Detail.ShowAsync(feed, detail, showAnimateCenter);
        }
    }
}