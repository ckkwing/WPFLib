using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Theme.Converters;
using Theme.Models;

namespace Theme
{
    static class Utility
    {
        public static string PathEllipsis(string path, int maxLength)
        {
            if (string.IsNullOrEmpty(path) || maxLength >= path.Length)
            {
                return path;
            }

            char[] seprators = new char[] { '\\', '/' };
            int lastPos = path.LastIndexOfAny(seprators);
            if (lastPos < 0 || path.Length - lastPos >= maxLength)
            {
                return path;
            }

            string ellipsispath = "...";
            int ellipsisPos = path.LastIndexOfAny(seprators, lastPos - path.Length + maxLength);
            if (ellipsisPos > 0)
            {
                ellipsispath = path.Substring(0, ellipsisPos + 1);
                ellipsispath += "...";
            }
            ellipsispath += path.Substring(lastPos, path.Length - lastPos);

            return ellipsispath;
        }

        public static double GetTextMetrics(string source, Typeface face, double fontSize)
        {
            if (string.IsNullOrEmpty(source) || fontSize <= 0.0 || face == null)
            {
                return 0.0;
            }

            double notrimWidth = 0.0;
            try
            {
                //try to get GlyphTypeface.
                GlyphTypeface glyphTypeface = null;
                face.TryGetGlyphTypeface(out glyphTypeface);
                if (glyphTypeface == null)
                {
                    throw new OperationCanceledException();
                }

                //calculate end string 's display width.

                foreach (char c in source)
                {
                    ushort w;
                    glyphTypeface.CharacterToGlyphMap.TryGetValue(c, out w);
                    notrimWidth += glyphTypeface.AdvanceWidths[w] * fontSize;
                }
            }
            catch (System.Exception ex)
            {
                //NLogger.LogHelper.UILogger.Error("Failed", ex);
                notrimWidth = 7.5;
            }
            return notrimWidth;
        }

        public class ToolTipHelper
        {
            /// <summary>
            /// Method to create tooltip object of given template type
            /// </summary>
            public static ToolTip CreateTooltip(ToolTipData toolTipData, ToolTipTemplateType type, Binding binding = null)
            {
                if (Application.Current == null)
                {
                    return null;
                }

                ToolTip toolTip = new ToolTip();
                toolTip.DataContext = toolTipData;
                // To read tooltip with Narrator
                AutomationProperties.SetName(toolTip, toolTipData.Description);

                switch (type)
                {
                    case ToolTipTemplateType.Classic:
                        Debug.Assert(false, "Error, didn't implemented yet");
                        break;
                    case ToolTipTemplateType.Simple:
                        Debug.Assert(false, "Error, didn't implemented yet");
                        break;
                    default:
                        toolTip.Style = (Style)Application.Current.TryFindResource("ToolTipStandardStyle_Global");
                        break;
                }

                if (null != binding)
                {
                    //// Bind the tooltip to IsToolTipsEnabled property for visibility.
                    //Binding binding = new Binding();
                    //binding.Source = AppSettings.Instance;
                    //binding.Path = new PropertyPath("IsToolTipsEnabled");
                    //binding.Converter = new ObjectToVisibilityConverter();

                    toolTip.SetBinding(ToolTip.VisibilityProperty, binding);
                }
                else
                    toolTip.Visibility = Visibility.Visible;

                return toolTip;
            }

            /// <summary>
            /// Method to create tooltip object of default (standard) template type
            /// </summary>
            public static ToolTip CreateTooltip(ToolTipData toolTipData)
            {
                return CreateTooltip(toolTipData, ToolTipTemplateType.Standard);
            }


        }
    }
}
