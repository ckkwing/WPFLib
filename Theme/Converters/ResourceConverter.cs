using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Theme.Converters
{
    public class ResourceConverter : IMultiValueConverter
    {
        public string ResourceKey { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length > 0 && values[0] is FrameworkElement element)
            {
                // 查找资源
                var resource = element.TryFindResource(ResourceKey) ??
                              Application.Current.TryFindResource(ResourceKey);

                return resource ?? string.Empty;
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
