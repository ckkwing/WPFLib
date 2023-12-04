using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Theme
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private object _obj = new object();
        protected bool _isLoaded = false;
        public virtual void Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        public virtual void Unloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
        }
        protected bool SetProperty<T>(ref T storage, T value, params string[] propertyNames)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;

            RaisePropertyChanged(propertyNames);
            return true;
        }

        protected void RunOnUIThread(Action action)
        {
            if (null == action || Application.Current == null)
            {
                return;
            }

            try
            {
                Dispatcher dispatcher = Application.Current.Dispatcher;
                bool isSameThread = false;
                lock (_obj)
                {
                    isSameThread = dispatcher.CheckAccess();
                }

                if (isSameThread)
                {
                    action();
                }
                else
                {
                    dispatcher.Invoke((Delegate)(action));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("RunOnUIThread error:{0}", ex.Message));
            }
        }

        protected void RunOnUIThreadAsync(Action action)
        {
            if (null == action || Application.Current == null)
            {
                return;
            }

            try
            {
                Dispatcher dispatcher = Application.Current.Dispatcher;
                bool isSameThread = false;
                lock (_obj)
                {
                    isSameThread = dispatcher.CheckAccess();
                }

                if (isSameThread)
                {
                    action();
                }
                else
                {
                    dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate)(action));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("RunOnUIThreadAsync error:{0}", ex.Message));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null)
                Handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
            CommandManager.InvalidateRequerySuggested();
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");

            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }

            CommandManager.InvalidateRequerySuggested();
        }
        #endregion
    }
}
