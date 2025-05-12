using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Theme.Behaviors
{
    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty ReachedBottomProperty =
            DependencyProperty.RegisterAttached(
                "ReachedBottom",
                typeof(ICommand),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(null, OnReachedBottomChanged));

        public static void SetReachedBottom(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ReachedBottomProperty, value);
        }

        public static ICommand GetReachedBottom(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ReachedBottomProperty);
        }

        private static void OnReachedBottomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                if (e.NewValue != null)
                {
                    scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                }
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (!scrollViewer.IsLoaded)
                return;
            if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 0.1)
            {
                var command = GetReachedBottom(scrollViewer);
                if (command?.CanExecute(null) == true)
                {
                    command.Execute(null);
                }
            }
        }
    }
}
