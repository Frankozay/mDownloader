using System;
using System.Globalization;
using System.Windows.Data;

namespace mDownloader.Converters
{
    public class TransferRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double rate)
            {
                const double kb = 1024;
                const double mb = 1024 * kb;
                const double gb = 1024 * mb;

                if (rate >= gb)
                {
                    return $"{rate / gb:F2} GB/s";
                }
                else if (rate >= mb)
                {
                    return $"{rate / mb:F2} MB/s";
                }
                else if (rate >= kb)
                {
                    return $"{rate / kb:F2} KB/s";
                }
                else
                {
                    return $"{rate:F2} B/s";
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
