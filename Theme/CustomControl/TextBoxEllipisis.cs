using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Theme.CustomControl
{
    public class TextBoxEllipisis : TextBox
    {
        #region OriginalText
        public string OriginalText
        {
            get { return (string)GetValue(OriginalTextProperty); }
            set { SetValue(OriginalTextProperty, value); }
        }

        public static readonly DependencyProperty OriginalTextProperty =
            DependencyProperty.Register("OriginalText", typeof(string), typeof(TextBoxEllipisis),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OriginalTextPropertyChangedCallback)));

        static void OriginalTextPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TextBoxEllipisis tb = obj as TextBoxEllipisis;
            if (tb == null) return;
            if (args.NewValue == null)
            {
                tb.Text = string.Empty;
            }
            else
            {
                string newText = args.NewValue.ToString();
                tb.Text = newText;
                int lastIndex = tb.GetCharacterIndexFromPoint(new Point(tb.ActualWidth, 0), true);
                if (lastIndex < newText.Length - 1)
                {
                    tb.Unit = tb.ActualWidth / (double)lastIndex;
                    tb.UpdateEllipisisWords();
                }
            }
        }
        #endregion

        #region constructor
        public TextBoxEllipisis()
        {
            this.GotFocus += new RoutedEventHandler(TextBoxEllipisis_GotFocus);
            this.LostFocus += new RoutedEventHandler(TextBoxEllipisis_LostFocus);
            this.SizeChanged += new SizeChangedEventHandler(TextBoxEllipisis_SizeChanged);
            Loaded += new RoutedEventHandler(TextBoxEllipisis_Loaded);
            this.KeyUp += new KeyEventHandler(TextBoxEllipisis_KeyUp);
        }
        #endregion

        #region member data
        public double Unit { get; set; }
        #endregion

        #region event
        void TextBoxEllipisis_Loaded(object sender, RoutedEventArgs e)
        {
            int lastIndex = this.GetCharacterIndexFromPoint(new Point(this.ActualWidth, 0), true);
            if (lastIndex < this.Text.Length - 1)
            {
                this.Text = Utility.PathEllipsis(this.Text, lastIndex);
                Unit = this.ActualWidth / (double)lastIndex;
            }
        }

        void TextBoxEllipisis_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsFocused)
            {
                this.OriginalText = this.Text;
            }
        }

        void TextBoxEllipisis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.IsFocused)
            {
                UpdateEllipisisWords();
            }
        }

        void TextBoxEllipisis_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateEllipisisWords();
        }

        void TextBoxEllipisis_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Text = OriginalText;
        }
        #endregion

        #region family method
        public void UpdateEllipisisWords()
        {
            if (Unit != 0)
            {
                double words = this.ActualWidth / Unit;
                this.Text = Utility.PathEllipsis(OriginalText, (int)words);
            }
        }
        #endregion
    }
}
