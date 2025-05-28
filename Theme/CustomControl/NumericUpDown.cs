using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Theme.CustomControl
{
    /// <summary>
    /// Represents a Windows spin box (also known as an up-down control) that displays numeric values.
    /// </summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class NumericUpDown : Control
    {
        #region Field

        private TextBox _textBox;
        #endregion

        #region Constructor

        static NumericUpDown()
        {
            InitializeCommands();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown),
                new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public NumericUpDown()
            : base()
        {
            this.ExcludedValues = new List<int>();
        }

        #endregion

        #region Dependency Property

        #region Value

        /// <summary>
        /// Gets or sets the value assigned to the control.
        /// </summary>
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(int), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnValueChanged),
                    new CoerceValueCallback(ValueCoerceCallback)));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            NumericUpDown control = (NumericUpDown)obj;
            control.CheckUpDownStatus();

            try
            {
                BindingExpression valueBinding = control.GetBindingExpression(NumericUpDown.ValueProperty);
                if (valueBinding != null)
                {
                    if ((valueBinding.ParentBinding.Mode == BindingMode.TwoWay || valueBinding.ParentBinding.Mode == BindingMode.OneWayToSource)
                        && valueBinding.ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.Explicit)
                    {
                        object dataItem = valueBinding.DataItem;
                        if (dataItem != null)
                        {
                            PropertyInfo pInfo = dataItem.GetType().GetProperty(valueBinding.ParentBinding.Path.Path);
                            if (pInfo != null)
                            {
                                pInfo.SetValue(dataItem, control.Value, null);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.Assert(false, e.Message);
            }
        }

        private static object ValueCoerceCallback(DependencyObject obj, object value)
        {
            int inputValue = (int)value;
            int minValue = (int)obj.GetValue(NumericUpDown.MinValueProperty);
            int maxValue = (int)obj.GetValue(NumericUpDown.MaxValueProperty);

            int coercedValue = inputValue;
            if ((bool)obj.GetValue(NumericUpDown.ForceIncrementCheckProperty))
            {
                int increment = (int)obj.GetValue(NumericUpDown.IncrementProperty);
                int remainder = coercedValue % increment;
                if (remainder != 0)
                {
                    // ExcludedValues
                    bool flag = false;
                    IEnumerable<int> ExcludedValues = obj.GetValue(NumericUpDown.ExcludedValuesProperty) as IEnumerable<int>;
                    if (ExcludedValues != null)
                    {
                        foreach (object v in ExcludedValues)
                        {
                            if (v is int && (int)v == coercedValue)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        coercedValue += remainder >= ((increment + 1) / 2) ? (increment - remainder) : (-remainder);
                    }
                }
            }
            if (coercedValue > maxValue)
            {
                coercedValue = maxValue;
            }
            if (coercedValue < minValue)
            {
                coercedValue = minValue;
            }

            return coercedValue;
        }

        #endregion

        #region MinValue

        /// <summary>
        /// Gets or sets the min value.
        /// </summary>
        /// <value>The min value.</value>
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(int.MinValue));

        #endregion

        #region MaxValue

        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        /// <value>The max value.</value>
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(int.MaxValue));


        #endregion

        #region Increment

        /// <summary>
        /// Gets or sets the increment.
        /// </summary>
        /// <value>The increment.</value>
        public int Increment
        {
            get { return (int)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Increment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(1));


        #endregion

        #region IsIncreasable

        /// <summary>
        /// Gets or sets a value indicating whether the value is large than maximum value.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is increasable; otherwise, <c>false</c>.
        /// </value>
        public bool IsIncreasable
        {
            get { return (bool)GetValue(IsIncreasableProperty); }
            set { SetValue(IsIncreasableProperty, value); }
        }

        public static readonly DependencyProperty IsIncreasableProperty =
            DependencyProperty.Register("IsIncreasable", typeof(bool), typeof(NumericUpDown),
            new UIPropertyMetadata(true));

        #endregion

        #region IsDecreasable

        /// <summary>
        /// Gets or sets a value indicating whether the value is less than minimum value.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is decreasable; otherwise, <c>false</c>.
        /// </value>
        public bool IsDecreasable
        {
            get { return (bool)GetValue(IsDecreasableProperty); }
            set { SetValue(IsDecreasableProperty, value); }
        }

        public static readonly DependencyProperty IsDecreasableProperty =
            DependencyProperty.Register("IsDecreasable", typeof(bool), typeof(NumericUpDown),
            new UIPropertyMetadata(true));

        #endregion

        #region ExcludedValues

        public List<int> ExcludedValues
        {
            get { return (List<int>)GetValue(ExcludedValuesProperty); }
            set { SetValue(ExcludedValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExcludedValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExcludedValuesProperty =
            DependencyProperty.Register("ExcludedValues", typeof(List<int>), typeof(NumericUpDown), new UIPropertyMetadata(null));

        #endregion

        #region ForceIncrementCheck


        /// <summary>
        /// Gets or sets a value indicating whether [force increment check].
        /// </summary>
        /// <value><c>true</c> if [force increment check]; otherwise, <c>false</c>.</value>
        public bool ForceIncrementCheck
        {
            get { return (bool)GetValue(ForceIncrementCheckProperty); }
            set { SetValue(ForceIncrementCheckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceIncrementCheck.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceIncrementCheckProperty =
            DependencyProperty.Register("ForceIncrementCheck", typeof(bool), typeof(NumericUpDown), new UIPropertyMetadata(false));



        #endregion

        #endregion

        #region Commands

        public static RoutedCommand IncreaseCommand
        {
            get
            {
                return _increaseCommand;
            }
        }
        public static RoutedCommand DecreaseCommand
        {
            get
            {
                return _decreaseCommand;
            }
        }

        private static void InitializeCommands()
        {
            _increaseCommand = new RoutedCommand("IncreaseCommand", typeof(NumericUpDown));
            CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown), new CommandBinding(_increaseCommand, OnIncreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(NumericUpDown), new InputBinding(_increaseCommand, new KeyGesture(Key.Up)));

            _decreaseCommand = new RoutedCommand("DecreaseCommand", typeof(NumericUpDown));
            CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown), new CommandBinding(_decreaseCommand, OnDecreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(NumericUpDown), new InputBinding(_decreaseCommand, new KeyGesture(Key.Down)));
        }

        private static void OnIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;
            if (control != null)
            {
                control.OnIncrease();
            }
        }
        private static void OnDecreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;
            if (control != null)
            {
                control.OnDecrease();
            }
        }

        private static RoutedCommand _increaseCommand;
        private static RoutedCommand _decreaseCommand;

        #endregion

        #region Override Methods

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (_textBox == null)
            {
                return;
            }

            var selectionStart = _textBox.SelectionStart;
            var selectionLength = _textBox.SelectionLength;
            var currentText = _textBox.Text;

            var newText = e.Text;

            try
            {
                if (currentText.Length > selectionStart && currentText.Length > selectionLength)
                {
                    newText = currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, e.Text);
                }
            }
            catch (Exception)
            {
            }

            e.Handled = !IsTextAllowed(newText);

            if (!e.Handled)
            {
                base.OnPreviewTextInput(e);
            }
        }

        private bool IsTextAllowed(string text)
        {
            return Regex.IsMatch(text, @"^-?\d*$");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_textBox != null)
            {
                _textBox.LostFocus -= new RoutedEventHandler(TextBox_LostFocus);
                _textBox.GotFocus -= new RoutedEventHandler(TextBox_GotFocus);
                _textBox.TextChanged -= new TextChangedEventHandler(TextBox_TextChanged);
                _textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
                //_textBox.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(TextBox_LostFocus);
            }
            _textBox = base.GetTemplateChild("PART_TextBox") as TextBox;
            if (_textBox != null)
            {
                _textBox.GotFocus += new RoutedEventHandler(TextBox_GotFocus);
                _textBox.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
                _textBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
                _textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
                //_textBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(TextBox_LostFocus);
            }
        }

        void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            var text = textBox.Text;

            if (!IsTextAllowed(text))
            {
                return;
            }

            string correctedText = Regex.Replace(text, @"[^0-9\-]", "");
            if (correctedText.IndexOf('-') > 0)
            {
                correctedText = correctedText.Replace("-", "");
                correctedText = "-" + correctedText;
            }

            if (correctedText != text)
            {
                textBox.Text = correctedText;
                textBox.CaretIndex = correctedText.Length;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    binding.UpdateTarget();
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void TextBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.IsFocused)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameworkElementAutomationPeer(this);
        }

        #endregion

        #region Help Method

        /// <summary>
        /// Checks whether can increase and decrease value.
        /// </summary>
        private void CheckUpDownStatus()
        {
            IsIncreasable = Value < MaxValue;
            IsDecreasable = Value > MinValue;
        }

        protected virtual void OnIncrease()
        {
            this.Value += this.Increment;
        }
        protected virtual void OnDecrease()
        {
            this.Value -= this.Increment;
        }

        #endregion
    }
}
