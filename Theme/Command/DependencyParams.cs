using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Theme.Command
{
    /// <summary>
    /// Provide a Data property to wrap the parameter from Xaml binding
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///<infrastrcutureHelper:BindingProxy Data="{Binding}" x:Key="bindingProxy" />
    /// 
    ///< Button Content = "{StaticResource IDS_OK}" Command = "{Binding SaveAllCommand}" Width = "80" Height = "22" Margin = "0,0,20,0" >
    ///    < Button.CommandParameter >
    ///        < infrastrcutureHelper:DependencyParams Data = "{Binding Path=Data.CommpandParams, Source={StaticResource bindingProxy}}" />
    ///    </ Button.CommandParameter >
    ///</ Button >
    /// ]]></example>
    public class DependencyParams : DependencyObject
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
           "Data", typeof(object), typeof(DependencyParams), new FrameworkPropertyMetadata(null));

        public object Data
        {
            get { return GetValue(DataProperty); }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        public static object GetValueToCompare(DependencyObject obj)
        {
            return obj.GetValue(ValueToCompareProperty);
        }

        public static void SetValueToCompare(DependencyObject obj, object value)
        {
            obj.SetValue(ValueToCompareProperty, value);
        }

        public static readonly DependencyProperty ValueToCompareProperty =
            DependencyProperty.RegisterAttached("ValueToCompare", typeof(object),
                                                  typeof(DependencyParams));
    }
}
