using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace CloudSpritzers1.Src.Converter
{
    public class DateTimeToLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dto)
            {
                // 1. Ensure we start from the absolute UTC moment
                DateTime utcTime = dto.UtcDateTime;

                // 2. Get the specific TimeZone for Romania (or your local system)
                // "GTB Standard Time" covers UTC+2/+3 for Romania/Greece/Turkey/etc.
                TimeZoneInfo localZone = TimeZoneInfo.Local;

                // 3. Convert UTC specifically to that Zone's current rules
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localZone);

                return localTime.ToString("MMM dd, HH:mm");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
