using SoftwareKobo.ACGNews.Services;
using System;
using Windows.UI.Xaml.Data;

namespace SoftwareKobo.ACGNews.Converters
{
    public class ChannelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Channel == false)
            {
                return value;
            }
            var channel = (Channel)value;
            switch (channel)
            {
                case Channel.Acg17173:
                    return "17173动漫频道";

                case Channel.Acg178:
                    return "178动漫频道";

                case Channel.Acgdoge:
                    return "ACGdoge";

                case Channel.AcgGamersky:
                    return "动漫星空";

                case Channel.Anitama:
                    return "Anitama";

                case Channel.TencentComic:
                    return "腾讯动漫频道";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}