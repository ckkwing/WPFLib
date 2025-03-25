using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Theme.Events
{
    public class DataRoutedEventArgs : RoutedEventArgs
    {
        public object Data { get; set; }
        public DataRoutedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }
    }
}
