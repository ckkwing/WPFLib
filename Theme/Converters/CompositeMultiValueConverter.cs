using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Theme.Converters
{
    /// <summary>
    /// A multiple composite converter to wrap other converters
    /// </summary>
    /// <seealso cref="System.Windows.Data.IMultiValueConverter" />
    /// <example>
    /// <nconv:CompositeMultiValueConverter x:Key="AndBooleansToVisibilityConverter">
    /// 	<nconv:CompositeMultiValueConverter.MultiValueConverter>
    /// 		<nconv:AndValuesToBooleanConverter/>
    /// 	</nconv:CompositeMultiValueConverter.MultiValueConverter>
    /// 	<BooleanToVisibilityConverter/>
    /// </nconv:CompositeMultiValueConverter>
    /// </example>
    [ContentProperty("Converters")]
    public class CompositeMultiValueConverter : IMultiValueConverter
    {
        private readonly CompositeConverter compositeConverter = new CompositeConverter();

        public IMultiValueConverter MultiValueConverter { get; set; }

        public Collection<IValueConverter> Converters
        {
            get
            {
                return this.compositeConverter.Converters;
            }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Trace.Assert(this.MultiValueConverter != null);
            if (this.MultiValueConverter == null)
                return null;

            return this.compositeConverter.Convert(this.MultiValueConverter.Convert(values, targetType, parameter, culture), targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
