using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Theme.Behaviors
{
    public static class DragMoveBehavior
    {
        public static readonly DependencyProperty CanDragMoveProperty =
            DependencyProperty.RegisterAttached("CanDragMove", typeof(bool), typeof(DragMoveBehavior), new PropertyMetadata(false, OnCanDragMoveChanged));

        public static bool GetCanDragMove(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDragMoveProperty);
        }

        public static void SetCanDragMove(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDragMoveProperty, value);
        }

        private static void OnCanDragMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement control)
            {
                if ((bool)e.NewValue)
                {
                    control.MouseLeftButtonDown += Control_MouseLeftButtonDown;
                    control.MouseLeftButtonUp += Control_MouseLeftButtonUp;
                    control.MouseMove += Control_MouseMove;
                }
                else
                {
                    control.MouseLeftButtonDown -= Control_MouseLeftButtonDown;
                    control.MouseLeftButtonUp -= Control_MouseLeftButtonUp;
                    control.MouseMove -= Control_MouseMove;
                }
            }
        }

        private static void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement control && e.LeftButton == MouseButtonState.Pressed)
            {
                control.CaptureMouse();
                control.Cursor = Cursors.Hand;
                control.RenderTransform = new TranslateTransform();
            }
        }

        private static void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement control && e.LeftButton == MouseButtonState.Released)
            {
                control.ReleaseMouseCapture();
                control.Cursor = Cursors.Arrow;
            }
        }

        private static void Control_MouseMove(object sender, MouseEventArgs e)
        {
            CanvasMove(sender, e);
            //TransformMove(sender, e);
        }

        private static void CanvasMove(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement control && e.LeftButton == MouseButtonState.Pressed)
            {
                FrameworkElement parent = control.Parent as FrameworkElement;
                var position = e.GetPosition(parent);
                //var left = position.X - (control.ActualWidth / 2);
                //var top = position.Y - (control.ActualHeight / 2);

                var left = Math.Min(Math.Max(0, position.X - control.DesiredSize.Width / 2), parent.ActualWidth - control.DesiredSize.Width);
                var top = Math.Min(Math.Max(0, position.Y - control.DesiredSize.Height / 2), parent.ActualHeight - control.DesiredSize.Height);

                Canvas.SetLeft(control, left);
                Canvas.SetTop(control, top);
                //Debug.WriteLine($"Left:{left}, Top:{top}");
            }
        }

        private static void TransformMove(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement control && e.LeftButton == MouseButtonState.Pressed)
            {
                FrameworkElement parent = control.Parent as FrameworkElement;
                var transform = control.RenderTransform as TranslateTransform;
                var position = e.GetPosition(parent);
                transform.X = Math.Min(Math.Max(0, position.X - control.DesiredSize.Width / 2), parent.ActualWidth - control.DesiredSize.Width);
                transform.Y = Math.Min(Math.Max(0, position.Y - control.DesiredSize.Height / 2), parent.ActualHeight - control.DesiredSize.Height);
            }

        }
    }
}
