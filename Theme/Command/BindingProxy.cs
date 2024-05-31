using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Theme.Command
{
    /// <summary>
    /// A proxy for passing data in Xaml
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
    /// ]]>
    /// </example>
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}

//< Button Content = "{StaticResource IDS_OK}" Command = "{Binding SaveAllCommand}" Width = "80" Height = "22" Margin = "0,0,20,0" >
//    < Button.CommandParameter >
//        < infrastrcutureHelper:DependencyParams Data = "{Binding Path=Data.CommpandParams, Source={StaticResource bindingProxy}}" />
//    </ Button.CommandParameter >
//</ Button >