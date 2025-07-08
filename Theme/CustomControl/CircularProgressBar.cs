using Microsoft.Expression.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Theme.CustomControl
{
    public class CircularProgressBar : ProgressBar
    {
        private FrameworkElement _track;
        /// <summary>
        /// Gets or sets progress bar fill direction
        /// </summary>
        public double ArcWidth
        {
            get => (double)GetValue(ArcWidthProperty);
            set => SetValue(ArcWidthProperty, value);
        }

        public static readonly DependencyProperty ArcWidthProperty =
            DependencyProperty.Register("ArcWidth", typeof(double), typeof(CircularProgressBar),
                new UIPropertyMetadata(10d));

        /// <summary>
        /// Gets or sets content presentation
        /// </summary>
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(CircularProgressBar),
                new UIPropertyMetadata(null));

        static CircularProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularProgressBar), new FrameworkPropertyMetadata(typeof(CircularProgressBar)));
        }

        private void SetPartTrackValue()
        {
            double minimum = this.Minimum;
            double maximum = this.Maximum;
            double value = this.Value;
            double num = (maximum <= minimum) ? 1.0 : ((value - minimum) / (maximum - minimum));

            var EndAngle = num * 360;

            if (_track != null)
            {
                var arc = _track as Arc;
                arc.EndAngle = EndAngle;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _track = GetTemplateChild("PART_Track") as FrameworkElement;
            SetPartTrackValue();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            SetPartTrackValue();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            SetPartTrackValue();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            SetPartTrackValue();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            SetPartTrackValue();
        }
    }
}
