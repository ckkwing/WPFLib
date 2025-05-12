using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Theme.CustomControl
{
    /// <summary>
    /// Common password box
    /// </summary>
    [TemplatePart(Name = PART_PasswordBox, Type = typeof(PasswordBox))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class CommonPasswordBoxWithToggle : UserControl
    {
        #region Const
        public const string PART_PasswordBox = "PART_PasswordBox";
        public const string PART_TextBox = "PART_TextBox";
        #endregion

        #region Properties
        /// <summary>
        /// Password box
        /// </summary>
        protected PasswordBox PARTPasswordBox { get; set; }
        /// <summary>
        /// Text box
        /// </summary>
        protected TextBox PARTTextBox { get; set; }
        #endregion

        #region Dependency properties

        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(CommonPasswordBoxWithToggle), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Password max length
        /// </summary>
        public int PasswordMaxLength
        {
            get { return (int)GetValue(PasswordMaxLengthProperty); }
            set { SetValue(PasswordMaxLengthProperty, value); }
        }

        public static readonly DependencyProperty PasswordMaxLengthProperty =
            DependencyProperty.Register("PasswordMaxLength", typeof(int), typeof(CommonPasswordBoxWithToggle), new PropertyMetadata(6));

        /// <summary>
        /// Whether to display password
        /// </summary>
        public bool IsPasswordVisible
        {
            get { return (bool)GetValue(IsPasswordVisibleProperty); }
            set { SetValue(IsPasswordVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPasswordVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPasswordVisibleProperty =
            DependencyProperty.Register("IsPasswordVisible", typeof(bool), typeof(CommonPasswordBoxWithToggle), new PropertyMetadata(false));

        /// <summary>
        /// Left icon button content
        /// </summary>
        /// </summary>
        public ImageSource LeftIconButtonContent
        {
            get { return (ImageSource)GetValue(LeftIconButtonContentProperty); }
            set { SetValue(LeftIconButtonContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftIconButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftIconButtonContentProperty =
            DependencyProperty.Register("LeftIconButtonContent", typeof(ImageSource), typeof(CommonPasswordBoxWithToggle), new PropertyMetadata(null));

        /// <summary>
        /// Left icon button display
        /// </summary>
        /// </summary>
        public Visibility LeftIconButtonVisibility
        {
            get { return (Visibility)GetValue(LeftIconButtonVisibilityProperty); }
            set { SetValue(LeftIconButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftIconButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftIconButtonVisibilityProperty =
            DependencyProperty.Register("LeftIconButtonVisibility", typeof(Visibility), typeof(CommonPasswordBoxWithToggle), new PropertyMetadata(Visibility.Collapsed));


        public string WaterText
        {
            get { return (string)GetValue(WaterTextProperty); }
            set { SetValue(WaterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaterTextProperty =
            DependencyProperty.Register("WaterText", typeof(string), typeof(CommonPasswordBoxWithToggle), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Hide password icon
        /// </summary>
        public ImageSource HidePasswordIcon
        {
            get { return (ImageSource)GetValue(HidePasswordIconProperty); }
            set { SetValue(HidePasswordIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HidePasswordIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HidePasswordIconProperty =
            DependencyProperty.Register("HidePasswordIcon", typeof(ImageSource), typeof(CommonPasswordBoxWithToggle), new PropertyMetadata(null));

        /// <summary>
        /// Display password icon
        /// </summary>
        public ImageSource ShowPasswordIcon
        {
            get { return (ImageSource)GetValue(ShowPasswordIconProperty); }
            set { SetValue(ShowPasswordIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowPasswordIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPasswordIconProperty =
            DependencyProperty.Register("ShowPasswordIcon", typeof(ImageSource), typeof(CommonPasswordBoxWithToggle), new PropertyMetadata(null));


        #endregion
        #region Constructure
        static CommonPasswordBoxWithToggle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommonPasswordBoxWithToggle), new FrameworkPropertyMetadata(typeof(CommonPasswordBoxWithToggle)));
        }
        #endregion

        #region Methods

        override public void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            try
            {
                PARTPasswordBox = Template.FindName(PART_PasswordBox, this) as PasswordBox;
                PARTPasswordBox.PasswordChanged -= PARTPasswordBox_PasswordChanged;
                PARTPasswordBox.PasswordChanged += PARTPasswordBox_PasswordChanged;
                PARTPasswordBox = Template.FindName(PART_PasswordBox, this) as PasswordBox;
                PARTPasswordBox.PreviewTextInput -= PARTPasswordBoxAndTextBox_PreviewTextInput;
                PARTPasswordBox.PreviewTextInput += PARTPasswordBoxAndTextBox_PreviewTextInput;
                PARTPasswordBox.PreviewKeyDown -= PARTPasswordBoxAndTextBox_PreviewKeyDown;
                PARTPasswordBox.PreviewKeyDown += PARTPasswordBoxAndTextBox_PreviewKeyDown;
                DataObject.AddPastingHandler(PARTPasswordBox, OnPaste);

                PARTTextBox = Template.FindName(PART_TextBox, this) as TextBox;
                PARTTextBox.TextChanged -= PARTTextBox_TextChanged;
                PARTTextBox.TextChanged += PARTTextBox_TextChanged;
                PARTTextBox = Template.FindName(PART_TextBox, this) as TextBox;
                PARTTextBox.PreviewTextInput -= PARTPasswordBoxAndTextBox_PreviewTextInput;
                PARTTextBox.PreviewTextInput += PARTPasswordBoxAndTextBox_PreviewTextInput;
                PARTTextBox.PreviewKeyDown -= PARTPasswordBoxAndTextBox_PreviewKeyDown;
                PARTTextBox.PreviewKeyDown += PARTPasswordBoxAndTextBox_PreviewKeyDown;
                DataObject.AddPastingHandler(PARTTextBox, OnPaste);
            }
            catch (Exception ex)
            {

            }
        }

        protected void PARTTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsPasswordVisible)
            {
                PARTPasswordBox.Password = PARTTextBox.Text;
            }
            Password = PARTTextBox.Text;
        }

        protected void PARTPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = PARTPasswordBox.Password;
        }

        protected virtual void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
        }

        protected virtual void PARTPasswordBoxAndTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

        protected virtual void PARTPasswordBoxAndTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
        }

        #endregion
    }
}
