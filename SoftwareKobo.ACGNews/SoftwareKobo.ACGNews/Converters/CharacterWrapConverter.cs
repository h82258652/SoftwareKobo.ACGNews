using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace SoftwareKobo.ACGNews.Converters
{
    public class CharacterWrapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var s = value as string;
            return s == null ? value : string.Join("\u200B", s.ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var s = value as string;
            return s == null ? value : s.Replace("\u200B", string.Empty);
        }
    }
}