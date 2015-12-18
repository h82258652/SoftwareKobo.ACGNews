using Windows.Storage;

namespace SoftwareKobo.ACGNews.Datas
{
    public static class AppSetting
    {
        public static bool NavigateBackBySlideToRight
        {
            get
            {
                var value = ApplicationData.Current.LocalSettings.Values[nameof(NavigateBackBySlideToRight)];
                if (value is bool)
                {
                    return (bool)value;
                }
                return false;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values[nameof(NavigateBackBySlideToRight)] = value;
            }
        }
    }
}