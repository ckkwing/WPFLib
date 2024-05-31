using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Theme.XamlHelpers
{
    public static class FocusAssistant
    {
        #region Example
        //  .xaml
        //  <Window xmlns:xamlHelpers="clr-namespace:BackItUp.Common.XamlHelpers" ...
        //  ...
        //  <TextBox ...
        //    xamlHelpers:FocusAssistant.IsBound="True"
        //    xamlHelpers:FocusAssistant.Binding="{Binding Path=IsFocused, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
        //    
        //   .cs
        //   public bool IsFocused { get { return _isFocused;} set {_isFocused = value; OnPropertyChanged("IsFocused")} }
        //  ...
        // IsFocused = true;
        #endregion

        #region Property
        /// <summary>
        /// use this property as the bridge between property of Control and the one from code behind
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
           DependencyProperty.RegisterAttached("Binding", typeof(bool), typeof(FocusAssistant), new FrameworkPropertyMetadata(false, OnBindingChanged));

        /// <summary>
        /// this property indicates if we will use this attached property
        /// </summary>
        public static readonly DependencyProperty IsBoundProperty = DependencyProperty.RegisterAttached(
             "IsBound", typeof(bool), typeof(FocusAssistant), new PropertyMetadata(false, OnIsBoundChanged));
        #endregion Property

        #region Public Methods
        /// <summary>
        /// set IsBound
        /// </summary>
        public static void SetIsBound(DependencyObject dp, bool value)
        {
            dp.SetValue(IsBoundProperty, value);
        }

        /// <summary>
        /// get IsBound
        /// </summary>
        public static bool GetIsBound(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsBoundProperty);
        }

        /// <summary>
        /// get Binding
        /// </summary>
        public static string GetBinding(DependencyObject dp)
        {
            return (string)dp.GetValue(BindingProperty);
        }

        /// <summary>
        /// set Binding
        /// </summary>
        public static void SetBinding(DependencyObject dp, bool value)
        {
            dp.SetValue(BindingProperty, value);
        }
        #endregion Public Methods

        #region Methods
        /// <summary>
        /// when the property changes, we notify the ui.
        /// </summary>
        private static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control col = d as Control;

            if (d == null)
            {
                return;
            }

            bool isFouced = (bool)e.NewValue;

            if (isFouced)
            {
                col.Focus();
            }
        }

        /// <summary>
        /// if this attached property is used, add GotFocus and LostFocus event handler to the UI Control,or else not.
        /// </summary>
        private static void OnIsBoundChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            Control col = dp as Control;

            if (col == null)
            {
                return;
            }

            bool wasBind = (bool)(e.OldValue);
            bool needToBind = (bool)(e.NewValue);

            if (wasBind)
            {
                col.GotFocus -= GotFocus;
                col.LostFocus -= LostFocus;
            }

            if (needToBind)
            {
                col.GotFocus += GotFocus;
                col.LostFocus += LostFocus;
            }
        }

        /// <summary>
        /// handle UI control GotFocus event
        /// </summary>
        private static void GotFocus(object sender, RoutedEventArgs e)
        {
            Control col = sender as Control;

            SetBinding(col, col.IsFocused);
        }

        /// <summary>
        /// handle UI control LostFocus event
        /// </summary>
        private static void LostFocus(object sender, RoutedEventArgs e)
        {
            Control col = sender as Control;

            SetBinding(col, col.IsFocused);
        }
        #endregion Methods
    }
}
