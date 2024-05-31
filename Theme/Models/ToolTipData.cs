using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Theme.Resources;

namespace Theme.Models
{
    /// <summary>
    /// Tool tip templated enums 
    /// </summary>
    public enum ToolTipTemplateType
    {
        Classic,
        Simple,
        Standard
    }

    /// <summary>
    /// Class used to handle tooltip data
    /// </summary>
    public class ToolTipData
    {
        #region Constructor
        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="description"></param>
        public ToolTipData(string tooltipUID)
        {
            ParseTooltipByUID(tooltipUID, null);
        }


        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="description"></param>
        public ToolTipData(string tooltipUID, params object[] args)
        {
            ParseTooltipByUID(tooltipUID, args);
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        public ToolTipData(string heading, string description, string iconSource)
        {
            this.Heading = heading;
            this.Description = description;
            this.Icon = GetIcon(iconSource);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the heading.
        /// </summary>
        /// <value>The heading.</value>
        public string Heading { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public Image Icon { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        #endregion

        #region Utility Methods
        /// <summary>
        /// Parses the resource into heading and description. If the args are passed then first replaces
        /// the arguments in the string and then tries to parse it.
        /// </summary>
        /// <param name="tooltipUID">Resource unique ID of the tooltip</param>
        /// <param name="args">List of arguments to replace in the tooltip string</param>
        void ParseTooltipByUID(string tooltipUID, params object[] args)
        {
            string tooltip = ResourceProvider.LoadString(tooltipUID);
            ParseTooltip(tooltip, args);
        }

        /// <summary>
        /// Parses the resource into heading and description. If the args are passed then first replaces
        /// the arguments in the string and then tries to parse it.
        /// </summary>
        /// <param name="tooltipUID">Resource unique ID of the tooltip</param>
        /// <param name="args">List of arguments to replace in the tooltip string</param>
        void ParseTooltip(string tooltip, params object[] args)
        {
            if (string.IsNullOrEmpty(tooltip)) return;

            if (args != null && args.Length > 0)
            {
                tooltip = string.Format(tooltip, args);
            }

            string pattern = @"\[?\]";
            Match match1 = Regex.Match(tooltip, pattern);
            if (match1.Success)
            {
                Match match2 = match1.NextMatch();
                if (match2.Success)
                {
                    this.Heading = tooltip.Substring(match1.Index + 1, match2.Index - match1.Index - match2.Length - 2);
                    this.Description = tooltip.Substring(match2.Index + 1);
                }
                else
                    this.Description = tooltip.Substring(match1.Index + 1);
            }
            else
                Description = tooltip;
        }



        /// <summary>
        /// Creates an icon object out of the given source path.
        /// </summary>
        /// <param name="iconSource">Icon source path</param>
        /// <returns></returns>
        Image GetIcon(string iconSource)
        {
            if (string.IsNullOrEmpty(iconSource))
            {
                return null;
            }

            Image icon = new Image();
            icon.Source = new BitmapImage(new Uri(iconSource, UriKind.Relative));
            return icon;
        }
        #endregion
    }
}
