using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities.Extension;

namespace Theme.Converters
{
    public class AutoListViewColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //ListView listview = value as ListView;
            //double width = listview.Width;
            //GridView gv = listview.View as GridView;
            //for (int i = 0; i < gv.Columns.Count; i++)
            //{
            //    if (!Double.IsNaN(gv.Columns[i].Width))
            //        width -= gv.Columns[i].Width;
            //}
            //return width - 5;// this is to take care of margin/padding

            ListView listView = value as ListView;
            double width = double.NaN;

            if (!double.IsNaN(listView.Width))
                width = listView.Width;
            else if (!double.IsNaN(listView.ActualWidth))
                width = listView.ActualWidth;
            GridView gv = listView.View as GridView;
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (!Double.IsNaN(gv.Columns[i].Width))
                    width -= gv.Columns[i].Width;
            }
            return width - 5;// this is to take care of margin/padding
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class AutoListViewColumnWidthMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = double.NaN;
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue )
                return width;
            if (null == values[0] || null == values[1] )
                return width;

            try
            {
                width = (double)values[0];
                ListView listView = values[1] as ListView;
                GridView gv = listView.View as GridView;
                for (int i = 0; i < gv.Columns.Count; i++)
                {
                    if (!Double.IsNaN(gv.Columns[i].Width))
                        width -= gv.Columns[i].Width;
                }
            }
            catch(Exception e)
            {

            }
            return width - 5;// this is to take care of margin/padding
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
