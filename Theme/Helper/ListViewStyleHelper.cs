using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Theme.Helper
{
    public static class ListViewStyleHelper
    {
        #region Sort Default Style
        /// <summary>
        /// View sort style, deafult is null
        /// </summary>
        public static readonly DependencyProperty SortDefaultStyleProperty =
            DependencyProperty.RegisterAttached("SortDefaultStyle", typeof(DataTemplate), typeof(ListViewStyleHelper));

        public static void SetSortDefaultStyle(DependencyObject element, DataTemplate value)
        {
            if (element == null) return;
            element.SetValue(SortDefaultStyleProperty, value);
        }

        public static DataTemplate GetSortDefaultStyle(DependencyObject element)
        {
            if (element == null) return null;
            return element.GetValue(SortDefaultStyleProperty) as DataTemplate;
        }
        #endregion

        #region sort Asc Style
        /// <summary>
        /// view sort style ,asc arrow
        /// </summary>
        public static readonly DependencyProperty SortAscStyleProperty =
                   DependencyProperty.RegisterAttached("SortAscStyle", typeof(DataTemplate), typeof(ListViewStyleHelper));

        public static void SetSortAscStyle(DependencyObject element, DataTemplate value)
        {
            if (element == null) return;
            element.SetValue(SortAscStyleProperty, value);
        }

        public static DataTemplate GetSortAscStyle(DependencyObject element)
        {
            if (element == null) return null;
            return element.GetValue(SortAscStyleProperty) as DataTemplate;
        }
        #endregion

        #region sort Desc Style
        /// <summary>
        /// view sort style, desc arrow
        /// </summary>
        public static readonly DependencyProperty SortDescStyleProperty =
                   DependencyProperty.RegisterAttached("SortDescStyle", typeof(DataTemplate), typeof(ListViewStyleHelper));

        public static void SetSortDescStyle(DependencyObject element, DataTemplate value)
        {
            if (element == null) return;
            element.SetValue(SortDescStyleProperty, value);
        }

        public static DataTemplate GetSortDescStyle(DependencyObject element)
        {
            if (element == null) return null;
            return element.GetValue(SortDescStyleProperty) as DataTemplate;
        }
        #endregion
    }
}
