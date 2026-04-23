using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace CloudSpritzers1.Src.Converter
{
        public class BooleanToGlyphConverter : IValueConverter
        {
        private const string IconWhenExpanded = "\uE70D";
        private const string IconWhenNotExpanded = "\uE76C";

        public object Convert(object value, Type targetType, object parameter, string language)
            {
                bool isExpanded = value is bool b && b;
                return isExpanded ? IconWhenExpanded : IconWhenNotExpanded;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                throw new NotImplementedException();
            }
        }
}
