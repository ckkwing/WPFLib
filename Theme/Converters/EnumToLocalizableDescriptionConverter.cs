using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Theme.Extensions;

namespace Theme.Converters
{
    public class EnumToLocalizableDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return string.Empty;

                string valueString = value.ToString();

                if (string.IsNullOrEmpty(valueString))
                    return string.Empty;

                ICustomAttributeProvider fieldInfo = value.GetType().GetField(valueString);
                return fieldInfo.GetLocalizedDescription();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
