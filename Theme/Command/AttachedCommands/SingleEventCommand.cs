﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Theme.Command.AttachedCommands
{
    #region SingleEventCommand Class
    /// <summary>
    /// This class allows a single command to event mappings.  
    /// It is used to wire up View events to a
    /// ViewModel ICommand implementation.  
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    /// <Grid Background="WhiteSmoke"
    ///    Nero.BackItUp.Commands:SingleEventCommand.RoutedEventName="MouseDown"
    ///      Nero.BackItUp.Commands:SingleEventCommand.TheCommandToRun=
    ///       "{Binding Path=ShowWindowCommand}"/>
    /// 
    /// ]]>
    /// </example>
    public static class SingleEventCommand
    {
        #region TheCommandToRun

        /// <summary>
        /// TheCommandToRun : The actual ICommand to run
        /// </summary>
        public static readonly DependencyProperty TheCommandToRunProperty =
            DependencyProperty.RegisterAttached("TheCommandToRun",
                typeof(ICommand),
                typeof(SingleEventCommand),
                new FrameworkPropertyMetadata((ICommand)null));

        /// <summary>
        /// Gets the TheCommandToRun property.  
        /// </summary>
        public static ICommand GetTheCommandToRun(DependencyObject d)
        {
            return (ICommand)d.GetValue(TheCommandToRunProperty);
        }

        /// <summary>
        /// Sets the TheCommandToRun property.  
        /// </summary>
        public static void SetTheCommandToRun(DependencyObject d, ICommand value)
        {
            d.SetValue(TheCommandToRunProperty, value);
        }
        #endregion

        #region RoutedEventName

        /// <summary>
        /// RoutedEventName : The event that should actually execute the
        /// ICommand
        /// </summary>
        public static readonly DependencyProperty RoutedEventNameProperty =
            DependencyProperty.RegisterAttached("RoutedEventName", typeof(String),
            typeof(SingleEventCommand),
                new FrameworkPropertyMetadata((String)String.Empty,
                    new PropertyChangedCallback(OnRoutedEventNameChanged)));

        /// <summary>
        /// Gets the RoutedEventName property.  
        /// </summary>
        public static String GetRoutedEventName(DependencyObject d)
        {
            return (String)d.GetValue(RoutedEventNameProperty);
        }

        /// <summary>
        /// Sets the RoutedEventName property.  
        /// </summary>
        public static void SetRoutedEventName(DependencyObject d, String value)
        {
            d.SetValue(RoutedEventNameProperty, value);
        }

        /// <summary>
        /// Hooks up a Dynamically created EventHandler (by using the 
        /// <see cref="EventHooker">EventHooker</see> class) that when
        /// run will run the associated ICommand
        /// </summary>
        private static void OnRoutedEventNameChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            String routedEvent = (String)e.NewValue;

            if (d == null || String.IsNullOrEmpty(routedEvent))
                return;


            //If the RoutedEvent string is not null, create a new
            //dynamically created EventHandler that when run will execute
            //the actual bound ICommand instance (usually in the ViewModel)
            EventHooker eventHooker = new EventHooker();
            eventHooker.ObjectWithAttachedCommand = d;

            EventInfo eventInfo = d.GetType().GetEvent(routedEvent,
                BindingFlags.Public | BindingFlags.Instance);

            //Hook up Dynamically created event handler
            if (eventInfo != null)
            {
                eventInfo.RemoveEventHandler(d,
                    eventHooker.GetNewEventHandlerToRunCommand(eventInfo));

                eventInfo.AddEventHandler(d,
                    eventHooker.GetNewEventHandlerToRunCommand(eventInfo));
            }

        }
        #endregion
    }
    #endregion

    #region EventHooker Class
    /// <summary>
    /// Contains the event that is hooked into the source RoutedEvent
    /// that was specified to run the ICommand
    /// </summary>
    sealed class EventHooker
    {
        #region Public Methods/Properties
        /// <summary>
        /// The DependencyObject, that holds a binding to the actual
        /// ICommand to execute
        /// </summary>
        public DependencyObject ObjectWithAttachedCommand { get; set; }

        /// <summary>
        /// Creates a Dynamic EventHandler that will be run the ICommand
        /// when the user specified RoutedEvent fires
        /// </summary>
        /// <param name="eventInfo">The specified RoutedEvent EventInfo</param>
        /// <returns>An Delegate that points to a new EventHandler
        /// that will be run the ICommand</returns>
        public Delegate GetNewEventHandlerToRunCommand(EventInfo eventInfo)
        {
            Delegate del = null;

            if (eventInfo == null)
                throw new ArgumentNullException("eventInfo");

            if (eventInfo.EventHandlerType == null)
                throw new ArgumentException("EventHandlerType is null");

            if (del == null)
                del = Delegate.CreateDelegate(eventInfo.EventHandlerType, this,
                      GetType().GetMethod("OnEventRaised",
                        BindingFlags.NonPublic |
                        BindingFlags.Instance));

            return del;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Runs the ICommand when the requested RoutedEvent fires
        /// </summary>
        private void OnEventRaised(object sender, EventArgs e)
        {
            ICommand command = (ICommand)(sender as DependencyObject).
                GetValue(SingleEventCommand.TheCommandToRunProperty);

            if (command != null)
            {
                command.Execute(sender);
            }
        }
        #endregion
    }
    #endregion
}
