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
    /// Converts the given corner radius into a new one, using the given parameter string.
    /// The possible commands in the parameter string:
    /// <list type="bullet">
    /// <item><term>$</term><description>take original</description></item>
    /// <item><term>0 or lt or tl</term><description>take left top corner</description></item>
    /// <item><term>1 or rt or tr</term><description>take right top corner</description></item>
    /// <item><term>2 or rb or rb</term><description>take right bottom corner</description></item>
    /// <item><term>3 or lb or bl</term><description>take left bottom corner</description></item>
    /// <item><term>l</term><description>take left corners</description></item>
    /// <item><term>t</term><description>take top corners</description></item>
    /// <item><term>r</term><description>take right corners</description></item>
    /// <item><term>b</term><description>take bottom corners</description></item>
    /// <item><term>i</term><description>increase all values by 1</description></item>
    /// <item><term>d</term><description>decrease all values not less than 1 by 1</description></item>
    /// <item><term>x</term><description>exchange current partial result and original</description></item>
    /// <item><term>z</term><description>start from empty</description></item>
    /// </list>
    /// </summary>
    public class PartialCornerRadiusConverter : IValueConverter
    {
        const char LT = '\x1';
        const char RT = '\x2';
        const char RB = '\x3';
        const char LB = '\x4';
        const char empty = '\0';

        /// <summary>
        /// Converts the given corner radius into a new one.
        /// </summary>
        /// <param name="value">the ininial value of the CornerRadius</param>
        /// <param name="parameter">Contains one or more commands, possibly separated by spaces:
        /// <list type="bullet">
        /// <item><term>$</term><description>take original</description></item>
        /// <item><term>0 or lt or tl</term><description>take left top corner</description></item>
        /// <item><term>1 or rt or tr</term><description>take right top corner</description></item>
        /// <item><term>2 or rb or rb</term><description>take right bottom corner</description></item>
        /// <item><term>3 or lb or bl</term><description>take left bottom corner</description></item>
        /// <item><term>l</term><description>take left corners</description></item>
        /// <item><term>t</term><description>take top corners</description></item>
        /// <item><term>r</term><description>take right corners</description></item>
        /// <item><term>b</term><description>take bottom corners</description></item>
        /// <item><term>i</term><description>increase all values by 1</description></item>
        /// <item><term>d</term><description>decrease all values not less than 1 by 1</description></item>
        /// <item><term>z</term><description>start from empty</description></item>
        /// </list>
        /// </param>
        /// <param name="culture"></param>
        /// <param name="targetType"></param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var orig = (CornerRadius)value;
            string includes = ((string)parameter ?? "$") + ' ';
            char saved = empty;
            var cr = new CornerRadius();
            var one = new CornerRadius(1);
            var minusone = new CornerRadius(-1);
            char lastaction = empty;
            bool suppressed = false;
            foreach (char c in includes)
            {
                // 1. translate
                char translated = this.translate(char.ToLowerInvariant(c), ref saved);
                if (translated == empty)
                    continue;
                // 2. process
                bool result = true;
                if (translated == '0')
                {
                    suppressed = true;
                    lastaction = empty;
                }
                else if (translated == '+')
                {
                    if (lastaction == empty)
                        throw new ArgumentException(@"No last action found", "parameter");
                    result = this.Modify(ref cr, lastaction, one);
                }
                else if (translated == '-')
                {
                    if (lastaction == empty)
                        throw new ArgumentException(@"No last action found", "parameter");
                    result = this.Modify(ref cr, lastaction, minusone);
                }
                else
                {
                    lastaction = translated;
                    if (!suppressed)
                        result = this.Modify(ref cr, translated, orig);
                    suppressed = false;
                }
                if (!result)
                    throw new ArgumentException(@"Unrecognized character in parameter: " + lastaction, "parameter");
            }
            if (saved != empty)
                throw new ArgumentException(@"Unpaired compound character in parameter: " + saved, "parameter");
            return cr;
        }

        private bool Modify(ref CornerRadius cr, char translated, CornerRadius pattern)
        {
            switch (translated)
            {
                case LT:
                    cr.TopLeft += pattern.TopLeft;
                    break;
                case RT:
                    cr.TopRight += pattern.TopRight;
                    break;
                case RB:
                    cr.BottomRight += pattern.BottomRight;
                    break;
                case LB:
                    cr.BottomLeft += pattern.BottomLeft;
                    break;
                case 'l':
                    cr.TopLeft += pattern.TopLeft;
                    cr.BottomLeft += pattern.BottomLeft;
                    break;
                case 't':
                    cr.TopLeft += pattern.TopLeft;
                    cr.TopRight += pattern.TopRight;
                    break;
                case 'r':
                    cr.TopRight += pattern.TopRight;
                    cr.BottomRight += pattern.BottomRight;
                    break;
                case 'b':
                    cr.BottomRight += pattern.BottomRight;
                    cr.BottomLeft += pattern.BottomLeft;
                    break;
                case '$':
                    cr.TopLeft += pattern.TopLeft; if (cr.TopLeft < 0) cr.TopLeft = 0;
                    cr.TopRight += pattern.TopRight; if (cr.TopRight < 0) cr.TopRight = 0;
                    cr.BottomRight += pattern.BottomRight; if (cr.BottomRight < 0) cr.BottomRight = 0;
                    cr.BottomLeft += pattern.BottomLeft; if (cr.BottomLeft < 0) cr.BottomLeft = 0;
                    break;
                default:
                    return false;
            }
            return true;
        }

        private char translate(char c, ref char saved)
        {
            switch (c)
            {
                case 'l':
                case 't':
                case 'r':
                case 'b':
                    if (saved != empty)
                    {
                        c = translateLTRB(saved, c);
                        saved = empty;
                        return c;
                    }

                    saved = c;
                    return empty;
                case ' ':
                    c = saved;
                    saved = empty;
                    return c;
                default:
                    if (saved != empty)
                    {
                        char tmp = c;
                        c = saved;
                        saved = tmp;
                    }
                    return c;
            }
        }

        private static char translateLTRB(char c1, char c2)
        {
            switch (c1)
            {
                case 'l':
                    switch (c2)
                    {
                        case 't': return LT;
                        case 'b': return LB;
                    }
                    break;
                case 't':
                    switch (c2)
                    {
                        case 'l': return LT;
                        case 'r': return RB;
                    }
                    break;
                case 'r':
                    switch (c2)
                    {
                        case 't': return RT;
                        case 'b': return RB;
                    }
                    break;
                case 'b':
                    switch (c2)
                    {
                        case 'l': return LB;
                        case 'r': return RB;
                    }
                    break;
            }
            throw new ArgumentException(@"Unexpected pair: " + c1 + c2, "c1");
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
