using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace CloudSpritzers1.Src.Converter
{
    public class HelpfulBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool selected = value is bool b && b;
            Color helpfulBackgroundColor = Color.FromArgb(255, 232, 247, 236);
            Color defaultBackgroundColor = Color.FromArgb(255, 248, 249, 251);
            return new SolidColorBrush(selected
                ? helpfulBackgroundColor
                : defaultBackgroundColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class HelpfulForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool selected = value is bool b && b;
            Color helpfulForegroundColor = Color.FromArgb(255, 21, 128, 61);
            Color defaultForegroundColor = Color.FromArgb(255, 107, 114, 128);
            return new SolidColorBrush(selected
                ? helpfulForegroundColor
                : defaultForegroundColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class HelpfulBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool selected = value is bool b && b;
            return new SolidColorBrush(selected
                ? Color.FromArgb(255, 134, 239, 172)
                : Color.FromArgb(255, 209, 213, 219));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class NotHelpfulBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool selected = value is bool b && b;
            return new SolidColorBrush(selected
                ? Color.FromArgb(255, 253, 236, 236)
                : Color.FromArgb(255, 248, 249, 251));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class NotHelpfulForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool selected = value is bool b && b;
            return new SolidColorBrush(selected
                ? Color.FromArgb(255, 180, 35, 24)
                : Color.FromArgb(255, 107, 114, 128));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class NotHelpfulBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool selected = value is bool b && b;
            return new SolidColorBrush(selected
                ? Color.FromArgb(255, 245, 181, 176)
                : Color.FromArgb(255, 209, 213, 219));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
