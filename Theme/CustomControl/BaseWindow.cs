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
using System.Windows.Media;
using System.Windows.Shell;
using Theme.Command;

namespace Theme.CustomControl
{
    public partial class BaseWindow : Window
    {
        public Brush TitlebarBackground
        {
            get { return (Brush)GetValue(TitlebarBackgroundProperty); }
            set { SetValue(TitlebarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty TitlebarBackgroundProperty =
            DependencyProperty.Register("TitlebarBackground", typeof(Brush),
             typeof(BaseWindow), new UIPropertyMetadata(Brushes.Transparent));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
             typeof(BaseWindow), new UIPropertyMetadata(new CornerRadius(0)));

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

        //public ICommand EscKeyCommand
        //{
        //    get;
        //    set;
        //}

        //public ICommand AltF4KeyCommand
        //{
        //    get;
        //    set;
        //}
        #endregion

        public BaseWindow()
        {
            InitializeStyle();
            Loaded += delegate
            {
                //InitializeWindowChrome();
                InitializeEvent();
            };
        }

        private void InitializeWindowChrome()
        {
            WindowChrome.SetWindowChrome(this,
                new WindowChrome()
                {
                    CaptionHeight = 0,
                    UseAeroCaptionButtons = false,
                    GlassFrameThickness = new Thickness(0),
                    CornerRadius = CornerRadius,
                    ResizeBorderThickness = new Thickness(6),
                    NonClientFrameEdges = NonClientFrameEdges.None
                    //NonClientFrameEdges = NonClientFrameEdges.Left | NonClientFrameEdges.Top | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom
                });
        }

        private void InitializeStyle()
        {
            if (null == Application.Current)
                return;
            if (!Application.Current.Resources.Contains("BaseWindowStyle"))
                return;
            Style = (Style)Application.Current.Resources["BaseWindowStyle"];
        }

        private void InitializeEvent()
        {
            try
            {
                ControlTemplate baseWindowTemplate = (ControlTemplate)Application.Current.Resources["BaseWindowControlTemplate"];
                Grid tb = (Grid)baseWindowTemplate.FindName("TitleBar", this);
                if (null == tb)
                    return;

                tb.MouseLeftButtonDown += delegate
                {
                    try
                    {
                        this.DragMove();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Trace.TraceError("DragMove exception", ex);
                    }
                };

                //this.InputBindings.Add(new KeyBinding() { Command = EscKeyCommand, CommandParameter = Key.Escape, CommandTarget = this, Key = Key.Escape });
                //this.InputBindings.Add(new KeyBinding() { Command = AltF4KeyCommand, CommandParameter = "Alt+F4", CommandTarget = this, Key = Key.F4, Modifiers = ModifierKeys.Alt });
            }
            catch (Exception ex)
            {
                Trace.TraceError("InitializeEvent", ex);
            }
        }
    }

    public partial class BaseWindow : INotifyPropertyChanged
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
