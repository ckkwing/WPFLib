using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using Theme.Command;

namespace Theme.CustomControl
{
    public partial class ChromeBaseWindow : Window
    {
        #region Command
        public ICommand CloseWindowCommand => new GenericCommand()
        {
            CanExecuteCallback = (obj) => { return true; },
            ExecuteCallback = (obj) => Close(),
        };

        public ICommand MinimizeWindowCommand => new GenericCommand()
        {
            CanExecuteCallback = (obj) => { return true; },
            ExecuteCallback = (obj) => WindowState = WindowState.Minimized
        };

        public ICommand MaximizeWindowCommand => new GenericCommand()
        {
            CanExecuteCallback = (obj) => { return true; },
            ExecuteCallback = (obj) =>
            {
                WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            }
        };
        #endregion

        public ChromeBaseWindow()
        {
            InitializeStyle();
            Loaded += delegate
            {
            };
        }

        private void InitializeStyle()
        {
            if (null == Application.Current)
                return;
            if (!Application.Current.Resources.Contains("WindowChromeStyle"))
                return;
            Style = (Style)Application.Current.Resources["WindowChromeStyle"];
        }
    }

    public partial class ChromeBaseWindow : INotifyPropertyChanged
    {
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
    }
}
