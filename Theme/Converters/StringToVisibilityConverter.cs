using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Theme.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
            if (Inverse)
            {
                visibility = visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
