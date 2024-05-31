using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Theme.Models;
using static Theme.Utility;

namespace Theme.CustomControl
{
    public class PathTextBlock : TextBlock
    {
        public PathTextBlock()
        {
            this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            Loaded += new RoutedEventHandler(OnLoaded);
        }

        #region OriginalText
        public string OriginalText
        {
            get { return (string)GetValue(OriginalTextProperty); }
            set { SetValue(OriginalTextProperty, value); }
        }

        public static readonly DependencyProperty OriginalTextProperty =
            DependencyProperty.Register("OriginalText", typeof(string), typeof(PathTextBlock),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OriginalTextPropertyChangedCallback)));

        static void OriginalTextPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PathTextBlock tb = obj as PathTextBlock;
            if (tb == null) return;
            if (args.NewValue == null)
            {
                tb.Text = string.Empty;
            }
            else
            {
                string newText = args.NewValue.ToString();
                tb.Text = newText;
                tb.UpdateEllipisisWords();
            }
        }
        #endregion

        private double _widthCharacter = 8.0;
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _widthCharacter = Utility.GetTextMetrics("a", new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), this.FontSize);
                UpdateEllipisisWords();
            }
            catch (Exception ex)
            {
                //NLogger.LogHelper.UILogger.Error("PathTextBlock::OnLoaded exception", ex);
                Debug.WriteLine($"PathTextBlock::OnLoaded exception: {ex}");
            }
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateEllipisisWords();
        }

        private void UpdateEllipisisWords()
        {
            bool tooltipEnabled = false;
            if (this.ActualWidth > 1.0
                && !string.IsNullOrEmpty(OriginalText))
            {
                double unit = this.ActualWidth / _widthCharacter;
                if (unit > 0.5)
                {
                    this.Text = Utility.PathEllipsis(OriginalText, (int)unit);
                    tooltipEnabled = string.Compare(this.Text, OriginalText, true) != 0;
                }
            }

            if (tooltipEnabled)
            {
                this.ToolTip = ToolTipHelper.CreateTooltip(new ToolTipData(null, OriginalText, null));
            }
            ToolTipService.SetIsEnabled(this, tooltipEnabled);
        }


    }
}
