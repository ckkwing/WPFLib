using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Theme.Converters
{
    /// <summary>
    /// This class represents a converter which can customize a given Thickness.
    /// The customization is done according to the parameter string:
    /// <list type="bullet">
    /// <item><term>$</term><description>add original</description></item>
    /// <item><term>l</term><description>add left part</description></item>
    /// <item><term>t</term><description>add top part</description></item>
    /// <item><term>r</term><description>add right part</description></item>
    /// <item><term>b</term><description>add bottom part</description></item>
    /// <item><term>-</term><description>substitute additions to subtractions</description></item>
    /// <item><term>-</term><description>revert subtractions to additions</description></item>
    /// <item><term>1-9</term><description>increase all values by the parameter value</description></item>
    /// <item><term>i[number](arg)</term><description>increase the given part by one or given whole number, arg is l, t, r, b or $</description></item>
    /// </list>
    /// </summary>
    public class PartialThicknessConverter : IValueConverter
    {
        /// <summary>
        /// Converts the given Thickness to a customized one.
        /// </summary>
        /// <param name="value">the starting thickness</param>
        /// <param name="parameter">the customization parameter</param>
        /// <param name="culture"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var orig = (Thickness)value;
            var template = orig;
            var includes = (string)parameter ?? "$";
            var t = new Thickness();
            int sign = 1;
            bool expectArg = false;
            int numericArg = -1;
            foreach (char c in includes)
            {
                if (!expectArg)
                {
                    switch (char.ToLowerInvariant(c))
                    {
                        case 'l':
                            t.Left += template.Left * sign;
                            break;
                        case 't':
                            t.Top += template.Top * sign;
                            break;
                        case 'r':
                            t.Right += template.Right * sign;
                            break;
                        case 'b':
                            t.Bottom += template.Bottom * sign;
                            break;
                        case '$':
                            t.Left += template.Left * sign;
                            t.Top += template.Top * sign;
                            t.Right += template.Right * sign;
                            t.Bottom += template.Bottom * sign;
                            break;
                        case '+':
                            sign = 1;
                            break;
                        case '-':
                            sign = -1;
                            break;
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            t.Left += sign * (c - '0');
                            t.Top += sign * (c - '0');
                            t.Right += sign * (c - '0');
                            t.Bottom += sign * (c - '0');
                            break;
                        case 'i':
                            expectArg = true;
                            numericArg = -1;
                            break;
                        default:
                            throw new ArgumentException(@"Unrecognized character in parameter: " + c, "parameter");
                    }
                }
                else
                {
                    if (c >= '0' && c <= '9')
                    {
                        if (numericArg == -1)
                            numericArg = 0;
                        numericArg *= 10;
                        numericArg += (c - '0');
                    }
                    else
                    {
                        if (numericArg == -1)
                            numericArg = 1;
                        numericArg *= sign;
                        expectArg = false;
                        switch (char.ToLowerInvariant(c))
                        {
                            case 'l':
                                t.Left += numericArg;
                                break;
                            case 't':
                                t.Top += numericArg;
                                break;
                            case 'r':
                                t.Right += numericArg;
                                break;
                            case 'b':
                                t.Bottom += numericArg;
                                break;
                            case '$':
                                t.Left += numericArg;
                                t.Top += numericArg;
                                t.Right += numericArg;
                                t.Bottom += numericArg;
                                break;
                            default:
                                throw new ArgumentException(@"Unrecognized argument for `i' in parameter: " + c, "parameter");
                        }
                    }
                }
            }
            if (expectArg)
                throw new ArgumentException(@"No argument for `i' in parameter", "parameter");
            return t;
        }

        /// <summary>
        /// Inverse conversion is not supported
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
