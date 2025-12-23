using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Theme.CustomControl
{
    public class BaseButton : Button
    {
        public Brush OverBackground
        {
            get { return (Brush)GetValue(OverBackgroundProperty); }
            set { SetValue(OverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty OverBackgroundProperty =
            DependencyProperty.Register("OverBackground", typeof(Brush),
             typeof(BaseButton), new UIPropertyMetadata(Brushes.Transparent));

        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush),
             typeof(BaseButton), new UIPropertyMetadata(Brushes.Transparent));

        public Brush FocusedBackground
        {
            get { return (Brush)GetValue(FocusedBackgroundProperty); }
            set { SetValue(FocusedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty FocusedBackgroundProperty =
            DependencyProperty.Register("FocusedBackground", typeof(Brush),
             typeof(BaseButton), new UIPropertyMetadata(Brushes.Transparent));

        public Brush DisabledBackground
        {
            get { return (Brush)GetValue(DisabledBackgroundProperty); }
            set { SetValue(DisabledBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundProperty =
            DependencyProperty.Register("DisabledBackground", typeof(Brush),
             typeof(BaseButton), new UIPropertyMetadata(Brushes.Transparent));

        static BaseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseButton), new FrameworkPropertyMetadata(typeof(BaseButton)));
        }
    }
}
