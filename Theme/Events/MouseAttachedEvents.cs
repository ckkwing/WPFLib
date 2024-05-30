using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Theme.Events
{
    //Sample

    //<Image Source = "{StaticResource IMG_AI_BOTSONIC}" Width="42" Height="45" SnapsToDevicePixels="True" Canvas.Bottom="40" Canvas.Right="40"
    //       behavior:DragMoveBehavior.CanDragMove="True" events:MouseAttachedEvents.IsEnabled="True">
    //    <i:Interaction.Triggers>
    //        <events:RoutedEventTrigger RoutedEvent = "events:MouseAttachedEvents.ShortPress" >
    //            < i:InvokeCommandAction Command = "{Binding Path=AIStoryCommand}" />
    //        </ events:RoutedEventTrigger>
    //    </i:Interaction.Triggers>
    //</Image>


    public static class MouseAttachedEvents
    {
        public readonly static TimeSpan LongPressInterval = TimeSpan.FromSeconds(1);

        public class MouseBehaviorsRecord
        {
            public DateTime LastClickTime { get; set; } = DateTime.Now;
        }

        private static IDictionary<UIElement, MouseBehaviorsRecord> _dictElementRecord = new Dictionary<UIElement, MouseBehaviorsRecord>();

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
                "IsEnabled", typeof(bool), typeof(MouseAttachedEvents), new PropertyMetadata(default(bool), OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        public static readonly RoutedEvent LongPressEvent = EventManager.RegisterRoutedEvent(
            "LongPress", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(MouseAttachedEvents));

        public static void AddLongPressHandler(UIElement element, EventHandler<RoutedEventArgs> handler)
        {
            element?.AddHandler(LongPressEvent, handler);
        }

        public static void RemoveLongPressHandler(UIElement element, EventHandler<RoutedEventArgs> handler)
        {
            element?.RemoveHandler(LongPressEvent, handler);
        }

        public static readonly RoutedEvent ShortPressEvent = EventManager.RegisterRoutedEvent(
            "ShortPress", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(MouseAttachedEvents));

        public static void AddShortPressHandler(DependencyObject d, EventHandler<RoutedEventArgs> handler)
        {
            (d as UIElement)?.AddHandler(ShortPressEvent, handler);
        }

        public static void RemoveShortPressHandler(DependencyObject d, EventHandler<RoutedEventArgs> handler)
        {
            (d as UIElement)?.RemoveHandler(ShortPressEvent, handler);
        }


        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;
            if (element == null)
                return;

            if ((bool)e.NewValue)
            {
                element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
                element.MouseMove += Element_MouseMove;
                _dictElementRecord.Add(element, new MouseBehaviorsRecord());
            }
            else
            {
                element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
                element.MouseLeftButtonUp -= Element_MouseLeftButtonUp;
                element.MouseMove -= Element_MouseMove;
                _dictElementRecord.Remove(element);
            }
        }

        private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null)
                return;

            element.CaptureMouse();
            MouseBehaviorsRecord mouseBehaviorsRecord = null;
            if (_dictElementRecord.TryGetValue(element, out mouseBehaviorsRecord))
            {
                if (null != mouseBehaviorsRecord)
                {
                    mouseBehaviorsRecord.LastClickTime = DateTime.Now;
                }
            }
        }

        private static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null)
                return;

            element.ReleaseMouseCapture();
            MouseBehaviorsRecord mouseBehaviorsRecord = null;
            if (_dictElementRecord.TryGetValue(element, out mouseBehaviorsRecord))
            {
                if (null != mouseBehaviorsRecord)
                {
                    DateTime nowTime = DateTime.Now;
                    if ((nowTime - mouseBehaviorsRecord.LastClickTime) > LongPressInterval)
                        element?.RaiseEvent(new RoutedEventArgs(LongPressEvent));
                    else
                        element?.RaiseEvent(new RoutedEventArgs(ShortPressEvent));

                    mouseBehaviorsRecord.LastClickTime = nowTime;
                }
            }
        }

        private static void Element_MouseMove(object sender, MouseEventArgs e)
        {
            // Implement your logic for determining a long press here.
            // For example, you can check if the time since MouseDown has exceeded a threshold.
        }

    }
}
