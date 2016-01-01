using SoftwareKobo.ACGNews.Services;
using System;
using Windows.UI.Xaml.Data;

namespace SoftwareKobo.ACGNews.Converters
{
    public class ChannelToFeedCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Channel)
            {
                var channel = (Channel)value;
                return channel.CreateFeedCollection();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}