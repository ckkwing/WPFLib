using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Theme.Converters
{
    /// <summary>
    /// A composite converter to wrap other converters
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    /// <example>
    /// <local:CompositeConverter x:Key="ObjectExistsToVisibilityConverter">
    /// 	<local:ObjectExistsConverter/>
    /// 	<BooleanToVisibilityConverter/>
    /// </local:CompositeConverter>
    /// </example>
    [ContentProperty("Converters")]
    public class CompositeConverter : IValueConverter
    {
        private readonly IList<IValueConverter> converters = new List<IValueConverter>();

        public Collection<IValueConverter> Converters
        {
            get
            {
                return new Collection<IValueConverter>(this.converters);
            }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (var converter in this.Converters)
            {
                value = converter.Convert(value, targetType, parameter, culture);
            }
            return value;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (var converter in this.Converters.Reverse())
            {
                value = converter.ConvertBack(value, targetType, parameter, culture);
            }
            return value;
        }

        #endregion
    }
}
