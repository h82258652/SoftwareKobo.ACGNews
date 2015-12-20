using SoftwareKobo.ACGNews.Models;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace SoftwareKobo.ACGNews.Views
{
    public sealed partial class AppView
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.BackPressedEventArgs"))
            {
                if (Detail.Visibility == Visibility.Visible)
                {
                    e.Handled = true;
                    Detail.Hide();
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