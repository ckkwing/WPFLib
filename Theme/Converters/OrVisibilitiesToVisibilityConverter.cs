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
    // OrBooleansToVisibilityConverter Can't used in visibility property if MultiBinding include other converters.
    // but it can be used in IsEnable property, if MultiBinding have other converters
    public class OrVisibilitiesToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return values.OfType<Visibility>().Any(item => item == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;

            }
            catch (InvalidCastException)
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
