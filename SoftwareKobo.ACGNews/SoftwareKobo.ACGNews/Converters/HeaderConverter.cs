using SoftwareKobo.ACGNews.Controls;
using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Services;
using System;
using Windows.UI.Xaml.Data;

namespace SoftwareKobo.ACGNews.Converters
{
    public class HeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Channel channel;
            if (value is Channel)
            {
                channel = (Channel)value;
            }
            else
            {
                channel = AppSetting.Instance.CurrentChannel;
            }
            switch (channel)
            {
                case Channel.Acg17173:
                    return new Acg17173Header();

                case Channel.Acg178:
                    return new Acg178Header();

                case Channel.Acgdoge:
                    return new AcgdogeHeader();

                case Channel.AcgGamersky:
                    return new AcgGamerskyHeader();

                case Channel.TencentComic:
                    return new TencentComicHeader();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}