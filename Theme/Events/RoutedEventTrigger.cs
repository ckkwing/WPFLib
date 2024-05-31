using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Theme.Events
{

    /// <summary>
    /// This class support to trigger attached events
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///<Image Source = "{StaticResource IMG_AI_BOTSONIC}" Width="42" Height="45" SnapsToDevicePixels="True" Canvas.Bottom="40" Canvas.Right="40"
    ///       behavior:DragMoveBehavior.CanDragMove="True" events:MouseAttachedEvents.IsEnabled="True">
    ///    <i:Interaction.Triggers>
    ///        <events:RoutedEventTrigger RoutedEvent = "events:MouseAttachedEvents.ShortPress" >
    ///            < i:InvokeCommandAction Command = "{Binding Path=AIStoryCommand}" />
    ///        </ events:RoutedEventTrigger>
    ///    </i:Interaction.Triggers>
    ///</Image>
    /// ]]>
    /// </example>
    public class RoutedEventTrigger : EventTriggerBase<DependencyObject>
    {
        private RoutedEvent _routedEvent;
        public RoutedEvent RoutedEvent
        {
            get
            {
                return _routedEvent;
            }
            set { _routedEvent = value; }
        }
        protected override string GetEventName()
        {
            return RoutedEvent.Name;
        }

        private void OnRoutedEvent(object sender, RoutedEventArgs args)
        {
            base.OnEvent(args);
        }

        protected override void OnAttached()
        {
            Behavior behavior = AssociatedObject as Behavior;
            FrameworkElement associatedElement = AssociatedObject as FrameworkElement;
            if (behavior != null)
            {
                associatedElement = ((IAttachedObject)behavior).AssociatedObject as FrameworkElement;
            }
            if (associatedElement == null)
            {
                throw new ArgumentException("Routed Event trigger can only be associated to framework elements");
            }
            if (RoutedEvent != null)
            {
                associatedElement.AddHandler(RoutedEvent, new RoutedEventHandler(OnRoutedEvent));
            }
        }
    }
}
