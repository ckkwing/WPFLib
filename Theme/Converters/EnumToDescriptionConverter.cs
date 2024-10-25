using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CommonUtility.Extensions;

namespace Theme.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        #region IValueConverter Members

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
                return fieldInfo.GetDescription();
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

        #endregion

        public static string Parse(object value)
        {
            return (string)new EnumToDescriptionConverter().Convert(value, null, null, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
