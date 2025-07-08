using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Theme.Converters
{
    public class ProgressValueToStrokeDashArrayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int iProgress = (int)values[0];
            double progress = (double)iProgress;
            double width = (double)values[1];
            double strokeThickness = (double)values[2];
            Debug.WriteLine($"{progress}");

            //double diameter = width - strokeThickness;
            //double circumference = Math.PI * diameter;
            //double dashLength = circumference * (progress / 100);
            //return new DoubleCollection { dashLength, circumference };

            //double diameter = width - strokeThickness;
            //double radius = diameter / 2;
            //double circumference = 2 * Math.PI * radius; // 半径
            //double progressRatio = progress / 100;
            //double dashLength = circumference * progressRatio;
            //return new double[] { dashLength, circumference };

            //double ratio = progress / 100;
            //double diameter = width - strokeThickness;
            //double circumference = Math.PI * diameter;
            //return new DoubleCollection { circumference * ratio, circumference };


            //double diameter = width - strokeThickness;
            //double circumference = Math.PI * diameter;
            //double progressRatio = progress / 100;
            //double dashLength = circumference * progressRatio;
            //double solidsLength = double.MaxValue;
            //return new DoubleCollection { dashLength, solidsLength };

            double diameter = width - strokeThickness;
            double radius = diameter / 2;
            //StrokeDashArray 用于将边框变成虚线，它的值是一个 double 类型的有序集合，集合中的值指虚线中每一段的长度，长度单位是边框值的宽度
            var perimeter = 2 * Math.PI * radius / strokeThickness;
            var step = progress / 100 * perimeter;
            return new DoubleCollection() { step, double.MaxValue };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
