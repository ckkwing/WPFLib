using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows;

namespace Theme.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    ///<i:Interaction.Behaviors>
    ///    <behaviors:RoutedEventToCommandBehavior EventName = "OnTimeLineItemTriggered" 
    ///                     Command="{Binding TimeLineTriggeredCommand}" EventArgsConverter="{StaticResource RoutedEventArgsConverter}"/>
    ///</i:Interaction.Behaviors>
    /// </example>
    public class RoutedEventToCommandBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(RoutedEventToCommandBehavior));

        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(RoutedEventToCommandBehavior));

        public static readonly DependencyProperty EventArgsConverterProperty =
            DependencyProperty.Register("EventArgsConverter", typeof(IValueConverter), typeof(RoutedEventToCommandBehavior));

        public static readonly DependencyProperty EventArgsConverterParameterProperty =
            DependencyProperty.Register("EventArgsConverterParameter", typeof(object), typeof(RoutedEventToCommandBehavior));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(RoutedEventToCommandBehavior));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public string EventName
        {
            get => (string)GetValue(EventNameProperty);
            set => SetValue(EventNameProperty, value);
        }

        public IValueConverter EventArgsConverter
        {
            get => (IValueConverter)GetValue(EventArgsConverterProperty);
            set => SetValue(EventArgsConverterProperty, value);
        }

        public object EventArgsConverterParameter
        {
            get => GetValue(EventArgsConverterParameterProperty);
            set => SetValue(EventArgsConverterParameterProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        protected override void OnAttached()
        {
            AttachHandler(EventName);
        }

        protected override void OnDetaching()
        {
            DetachHandler(EventName);
        }

        private void AttachHandler(string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return;

            var eventInfo = AssociatedObject.GetType().GetEvent(eventName);
            if (eventInfo != null)
            {
                var handler = Delegate.CreateDelegate(
                    eventInfo.EventHandlerType,
                    this,
                    nameof(OnEventTriggered));

                eventInfo.AddEventHandler(AssociatedObject, handler);
            }
        }

        private void DetachHandler(string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return;

            var eventInfo = AssociatedObject.GetType().GetEvent(eventName);
            if (eventInfo != null)
            {
                var handler = Delegate.CreateDelegate(
                    eventInfo.EventHandlerType,
                    this,
                    nameof(OnEventTriggered));

                eventInfo.RemoveEventHandler(AssociatedObject, handler);
            }
        }

        private void OnEventTriggered(object sender, RoutedEventArgs e)
        {
            object parameter = CommandParameter ?? e;

            if (EventArgsConverter != null)
            {
                parameter = EventArgsConverter.Convert(
                    e,
                    typeof(object),
                    EventArgsConverterParameter,
                    CultureInfo.CurrentCulture);
            }

            if (Command?.CanExecute(parameter) == true)
            {
                Command.Execute(parameter);
            }
        }
    }
}
