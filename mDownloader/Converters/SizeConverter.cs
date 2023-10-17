using System;
using System.Globalization;
using System.Windows.Data;

namespace mDownloader.Converters
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long size)
            {
                const double kb = 1024;
                const double mb = 1024 * kb;
                const double gb = 1024 * mb;

                if (size >= gb)
                {
                    return $"{size / gb:F2}GB";
                }
                else if (size >= mb)
                {
                    return $"{size / mb:F2}MB";
                }
                else if (size >= kb)
                {
                    return $"{size / kb:F2}KB";
                }
                else
                {
                    return $"{size:F2}B";
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
