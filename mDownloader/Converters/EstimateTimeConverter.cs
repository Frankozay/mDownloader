using System;
using System.Globalization;
using System.Windows.Data;

namespace mDownloader.Converters
{
    public class EstimateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double seconds)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

                if (timeSpan.TotalDays >= 1)
                {
                    // Format as "xdayxhxminxs"
                    return $"{timeSpan.Days}day{timeSpan.Hours}h{timeSpan.Minutes}min{timeSpan.Seconds}s";
                }
                else if (timeSpan.TotalHours >= 1)
                {
                    // Format as "xhxminxs"
                    return $"{timeSpan.Hours}h{timeSpan.Minutes}min{timeSpan.Seconds}s";
                }
                else if (timeSpan.TotalMinutes >= 1)
                {
                    // Format as "xminxs"
                    return $"{timeSpan.Minutes}min{timeSpan.Seconds}s";
                }
                else
                {
                    // Format as "xs"
                    return $"{timeSpan.Seconds}s";
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
