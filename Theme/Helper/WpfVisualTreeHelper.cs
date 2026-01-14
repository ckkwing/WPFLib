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

        /// <summary>
        /// Finds all items with a certain name in a subtree of the visual tree. 
        /// Adds all th found items to the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // Check the current child element
                if (child is T result &&
                    ((child is FrameworkElement fe && fe.Name == childName) ||
                     (child is FrameworkElement fe2 && fe2.Tag?.ToString() == childName)))
                {
                    return result;
                }

                // Recursive search
                var childResult = FindChild<T>(child, childName);
                if (childResult != null) return childResult;
            }

            return null;
        }
    }
}
