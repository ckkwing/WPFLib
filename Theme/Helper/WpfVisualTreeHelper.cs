using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Theme.Helper
{
    public class WpfVisualTreeHelper
    {
        /// <summary>
        /// Finds all items with a certain name in a subtree of the visual tree. 
        /// Adds all th found items to the result.
        /// </summary>
        public static List<FrameworkElement> FindChildren(DependencyObject parent, string theName)
        {
            List<FrameworkElement> children = new List<FrameworkElement>();
            int n = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < n; ++i)
            {
                DependencyObject d = VisualTreeHelper.GetChild(parent, i);

                var fe = d as FrameworkElement;
                if (fe != null)
                {
                    string name = fe.Name;
                    if (name == theName)
                        children.Add(fe);
                }

                var list = FindChildren(d, theName);
                children.AddRange(list);
            }

            return children;
        }
    }
}
