using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Theme.AttachedProperties
{
    public class BookmarkItem : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(BookmarkItem),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(BookmarkItem),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(BookmarkItem),
                new PropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTip", typeof(object), typeof(BookmarkItem),
                new PropertyMetadata(null));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public object ToolTip
        {
            get => GetValue(ToolTipProperty);
            set => SetValue(ToolTipProperty, value);
        }
    }

    public static class SliderBookmarkAttachedProperties
    {
        #region Bookmarks attached properties

        public static readonly DependencyProperty BookmarksProperty =
            DependencyProperty.RegisterAttached(
                "Bookmarks",
                typeof(ObservableCollection<BookmarkItem>),
                typeof(SliderBookmarkAttachedProperties),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnBookmarksChanged));

        public static void SetBookmarks(DependencyObject element, ObservableCollection<BookmarkItem> value)
        {
            element.SetValue(BookmarksProperty, value);
        }

        public static ObservableCollection<BookmarkItem> GetBookmarks(DependencyObject element)
        {
            return (ObservableCollection<BookmarkItem>)element.GetValue(BookmarksProperty);
        }

        #endregion

        #region BookmarkTemplate attached properties

        public static readonly DependencyProperty BookmarkTemplateProperty =
            DependencyProperty.RegisterAttached(
                "BookmarkTemplate",
                typeof(DataTemplate),
                typeof(SliderBookmarkAttachedProperties),
                new PropertyMetadata(null, OnBookmarkTemplateChanged));

        public static void SetBookmarkTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(BookmarkTemplateProperty, value);
        }

        public static DataTemplate GetBookmarkTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(BookmarkTemplateProperty);
        }

        #endregion

        #region BookmarkClickCommand attached properties

        public static readonly DependencyProperty BookmarkClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "BookmarkClickCommand",
                typeof(System.Windows.Input.ICommand),
                typeof(SliderBookmarkAttachedProperties),
                new PropertyMetadata(null));

        public static void SetBookmarkClickCommand(DependencyObject element, System.Windows.Input.ICommand value)
        {
            element.SetValue(BookmarkClickCommandProperty, value);
        }

        public static System.Windows.Input.ICommand GetBookmarkClickCommand(DependencyObject element)
        {
            return (System.Windows.Input.ICommand)element.GetValue(BookmarkClickCommandProperty);
        }

        #endregion

        #region BookmarkHeight attached properties

        public static readonly DependencyProperty BookmarkHeightProperty =
            DependencyProperty.RegisterAttached(
                "BookmarkHeight",
                typeof(double),
                typeof(SliderBookmarkAttachedProperties),
                new PropertyMetadata(15.0));

        public static void SetBookmarkHeight(DependencyObject element, double value)
        {
            element.SetValue(BookmarkHeightProperty, value);
        }

        public static double GetBookmarkHeight(DependencyObject element)
        {
            return (double)element.GetValue(BookmarkHeightProperty);
        }

        #endregion

        #region BookmarkWidth attached properties

        public static readonly DependencyProperty BookmarkWidthProperty =
            DependencyProperty.RegisterAttached(
                "BookmarkWidth",
                typeof(double),
                typeof(SliderBookmarkAttachedProperties),
                new PropertyMetadata(2.0));

        public static void SetBookmarkWidth(DependencyObject element, double value)
        {
            element.SetValue(BookmarkWidthProperty, value);
        }

        public static double GetBookmarkWidth(DependencyObject element)
        {
            return (double)element.GetValue(BookmarkWidthProperty);
        }

        #endregion

        #region Monitor changes in the Slider property

        private static readonly Dictionary<Slider, PropertyChangedEventHandler> _sliderHandlers =
            new Dictionary<Slider, PropertyChangedEventHandler>();

        // Store the Slider and its corresponding decorator
        private static readonly Dictionary<Slider, BookmarkAdorner> _sliderAdorners =
            new Dictionary<Slider, BookmarkAdorner>();

        private static void RegisterSliderPropertyChanges(Slider slider)
        {
            if (_sliderHandlers.ContainsKey(slider)) return;

            var handler = new PropertyChangedEventHandler((s, e) =>
            {
                if (e.PropertyName == "Maximum" || e.PropertyName == "Minimum" ||
                    e.PropertyName == "ActualWidth" || e.PropertyName == "ActualHeight")
                {
                    // Delay the update until the layout is completed
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateBookmarkPositions(slider);
                    }), DispatcherPriority.Render);
                }
            });

            var descriptor = DependencyPropertyDescriptor.FromProperty(Slider.MaximumProperty, typeof(Slider));
            descriptor?.AddValueChanged(slider, (s, e) => handler?.Invoke(s, new PropertyChangedEventArgs("Maximum")));

            var minDescriptor = DependencyPropertyDescriptor.FromProperty(Slider.MinimumProperty, typeof(Slider));
            minDescriptor?.AddValueChanged(slider, (s, e) => handler?.Invoke(s, new PropertyChangedEventArgs("Minimum")));

            // Monitor the SizeChanged event to handle changes in ActualWidth/Height
            slider.SizeChanged += (s, e) =>
            {
                if (e.WidthChanged || e.HeightChanged)
                {
                    handler?.Invoke(s, new PropertyChangedEventArgs("Size"));
                }
            };

            slider.Unloaded += (s, e) => UnregisterSliderPropertyChanges(slider);

            _sliderHandlers[slider] = handler;
        }

        private static void UnregisterSliderPropertyChanges(Slider slider)
        {
            if (_sliderHandlers.TryGetValue(slider, out var handler))
            {
                // Cleanup event listener
                var descriptor = DependencyPropertyDescriptor.FromProperty(Slider.MaximumProperty, typeof(Slider));
                descriptor?.RemoveValueChanged(slider, (s, e) => handler?.Invoke(s, new PropertyChangedEventArgs("Maximum")));

                var minDescriptor = DependencyPropertyDescriptor.FromProperty(Slider.MinimumProperty, typeof(Slider));
                minDescriptor?.RemoveValueChanged(slider, (s, e) => handler?.Invoke(s, new PropertyChangedEventArgs("Minimum")));

                _sliderHandlers.Remove(slider);
            }
        }

        //private static void UpdateBookmarkPositions(Slider slider)
        //{
        //    var container = slider.Template?.FindName("BookmarkContainer", slider) as Canvas;
        //    if (container == null) return;

        //    var bookmarks = GetBookmarks(slider);
        //    if (bookmarks == null) return;

        //    // Update the locations of all bookmarks
        //    for (int i = 0; i < container.Children.Count && i < bookmarks.Count; i++)
        //    {
        //        var bookmarkControl = container.Children[i];
        //        var bookmark = bookmarks[i];

        //        if (bookmarkControl is FrameworkElement element)
        //        {
        //            UpdateBookmarkPosition(element, slider, bookmark);
        //        }
        //    }
        //}
        private static void UpdateBookmarkPositions(Slider slider)
        {
            if (_sliderAdorners.TryGetValue(slider, out var adorner))
            {
                var container = adorner.BookmarkContainer;
                var bookmarks = GetBookmarks(slider);

                if (bookmarks == null) return;

                // Ensure that the number of bookmarks in the container matches the number of bookmarks
                if (container.Children.Count != bookmarks.Count)
                {
                    // If there is any inconsistency, recreate the bookmark
                    ClearBookmarks(slider);
                    CreateBookmarks(slider, bookmarks);
                    return;
                }

                // Update the locations of all bookmarks
                for (int i = 0; i < container.Children.Count && i < bookmarks.Count; i++)
                {
                    var bookmarkControl = container.Children[i];
                    var bookmark = bookmarks[i];

                    if (bookmarkControl is FrameworkElement element)
                    {
                        UpdateBookmarkPosition(element, slider, bookmark);
                    }
                }
            }
        }


        #endregion

        #region Private methods

        private static void OnBookmarksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Slider slider)
            {
                // Clear old bookmarks
                ClearBookmarks(slider);

                //if (e.NewValue is ObservableCollection<BookmarkItem> bookmarks && bookmarks.Count > 0)
                if (e.NewValue is ObservableCollection<BookmarkItem> bookmarks && null != bookmarks)
                {
                    // Monitor collection changes
                    if (e.OldValue is ObservableCollection<BookmarkItem> oldCollection)
                    {
                        oldCollection.CollectionChanged -= OnBookmarksCollectionChanged;
                    }

                    bookmarks.CollectionChanged += (sender, args) => UpdateBookmarks(slider);

                    // Register for Slider attribute change listener
                    RegisterSliderPropertyChanges(slider);

                    // Create a bookmark
                    CreateBookmarks(slider, bookmarks);
                }
            }
        }

        private static void OnBookmarkTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Slider slider)
            {
                // Recreate a bookmark
                var bookmarks = GetBookmarks(slider);
                if (bookmarks != null)
                {
                    ClearBookmarks(slider);
                    CreateBookmarks(slider, bookmarks);
                }
            }
        }

        private static void OnBookmarksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // In practical use, we update by binding to a specific Slider
        }

        private static void UpdateBookmarks(Slider slider)
        {
            ClearBookmarks(slider);
            var bookmarks = GetBookmarks(slider);
            if (bookmarks != null)
            {
                CreateBookmarks(slider, bookmarks);
            }
        }

        private static void ClearBookmarks(Slider slider)
        {
            // Method 1: Using a decorator
            if (_sliderAdorners.TryGetValue(slider, out var adorner))
            {
                adorner.BookmarkContainer.Children.Clear();
            }
            else
            {
                // Method 2: Try to find and remove it from the visual tree of the Slider
                var canvas = FindChild<Canvas>(slider, "BookmarkContainer");
                if (canvas != null)
                {
                    canvas.Children.Clear();
                }
            }
        }

        private static void CreateBookmarks(Slider slider, ObservableCollection<BookmarkItem> bookmarks)
        {
            // Waiting for Slider to load
            if (!slider.IsLoaded)
            {
                slider.Loaded += OnSliderLoaded;
                return;
            }

            InternalCreateBookmarks(slider, bookmarks);
        }

        private static void OnSliderLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Slider slider)
            {
                slider.Loaded -= OnSliderLoaded;
                var bookmarks = GetBookmarks(slider);
                if (bookmarks != null)
                {
                    InternalCreateBookmarks(slider, bookmarks);
                }
            }
        }

        private static void InternalCreateBookmarks(Slider slider, ObservableCollection<BookmarkItem> bookmarks)
        {
            // Get or create a bookmark container
            var container = GetOrCreateBookmarkContainer(slider);

            foreach (var bookmark in bookmarks)
            {
                var bookmarkControl = CreateBookmarkControl(slider, bookmark);
                if (bookmarkControl != null)
                {
                    container.Children.Add(bookmarkControl);
                }
            }
        }

        private static Canvas GetOrCreateBookmarkContainer(Slider slider)
        {
            if (_sliderAdorners.TryGetValue(slider, out var adorner))
            {
                return adorner.BookmarkContainer;
            }

            // Create new decorators and containers
            var container = new Canvas
            {
                Name = "BookmarkContainer",
                IsHitTestVisible = false, //TODO:Test, should set false
                ClipToBounds = true
            };

            var newAdorner = new BookmarkAdorner(slider, container);
            var adornerLayer = AdornerLayer.GetAdornerLayer(slider);
            if (adornerLayer != null)
            {
                adornerLayer.Add(newAdorner);
                _sliderAdorners[slider] = newAdorner;

                // Monitor the unloading event of Slider
                slider.Unloaded += OnSliderUnloaded;
            }

            return container;
        }

        private static void OnSliderUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is Slider slider && _sliderAdorners.TryGetValue(slider, out var adorner))
            {
                // Clean up decorators
                var adornerLayer = AdornerLayer.GetAdornerLayer(slider);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(adorner);
                }

                _sliderAdorners.Remove(slider);
                slider.Unloaded -= OnSliderUnloaded;
            }
        }

        private static FrameworkElement CreateBookmarkControl(Slider slider, BookmarkItem bookmark)
        {
            // Check if there are custom templates
            var template = GetBookmarkTemplate(slider);
            if (template != null)
            {
                var contentControl = new ContentControl
                {
                    Content = bookmark,
                    ContentTemplate = template,
                    DataContext = bookmark,
                    //Background = Brushes.Transparent,
                    //IsHitTestVisible = true,
                };

                // Set location
                contentControl.Loaded += (s, e) => UpdateBookmarkPosition(contentControl, slider, bookmark);

                // Add click event
                if (GetBookmarkClickCommand(slider) != null)
                {
                    contentControl.MouseLeftButtonDown += (s, e) =>
                    {
                        var command = GetBookmarkClickCommand(slider);
                        if (command?.CanExecute(bookmark) == true)
                        {
                            command.Execute(bookmark);
                        }
                    };
                }

                return contentControl;
            }
            else
            {
                // Default bookmark style
                var rectangle = new Rectangle
                {
                    Width = GetBookmarkWidth(slider),
                    Height = GetBookmarkHeight(slider),
                    Fill = bookmark.Color,
                    Stroke = Brushes.White,
                    StrokeThickness = 1,
                    ToolTip = bookmark.ToolTip ?? bookmark.Label,
                    //Cursor = System.Windows.Input.Cursors.Hand,
                    //IsHitTestVisible = true
                };

                // Set data binding
                rectangle.SetBinding(Shape.FillProperty, new Binding("Color") { Source = bookmark });
                rectangle.SetBinding(FrameworkElement.ToolTipProperty, new Binding("ToolTip") { Source = bookmark });
                //ToolTipService.SetShowDuration(rectangle, 5000);
                //ToolTipService.SetInitialShowDelay(rectangle, 1000);
                //ToolTipService.SetBetweenShowDelay(rectangle, 0);

                // Set position
                rectangle.Loaded += (s, e) => UpdateBookmarkPosition(rectangle, slider, bookmark);

                // Add click event
                rectangle.MouseLeftButtonDown += (s, e) =>
                {
                    slider.Value = bookmark.Value;
                    e.Handled = true;

                    var command = GetBookmarkClickCommand(slider);
                    if (command?.CanExecute(bookmark) == true)
                    {
                        command.Execute(bookmark);
                    }
                };

                return rectangle;
            }
        }

        private static Rect GetTrackBounds(Slider slider)
        {
            var track = slider.Template?.FindName("PART_Track", slider) as Track;
            if (track == null) return new Rect(0, 0, slider.ActualWidth, slider.ActualHeight);

            // Get the boundaries of the track within the Slider
            return track.TransformToAncestor(slider).TransformBounds(
                new Rect(0, 0, track.ActualWidth, track.ActualHeight));
        }

        private static void UpdateBookmarkPosition(FrameworkElement bookmarkControl, Slider slider, BookmarkItem bookmark)
        {
            if (slider.ActualWidth <= 0 || slider.Minimum > slider.Maximum) return;

            // Ensure that the bookmark value is within the specified range
            double bookmarkValue = Math.Max(slider.Minimum, Math.Min(bookmark.Value, slider.Maximum));

            double range = slider.Maximum - slider.Minimum;
            if (Math.Abs(range) < double.Epsilon) return;

            // Obtain track boundaries
            Rect trackBounds = GetTrackBounds(slider);

            // Get Thumb
            var thumb = slider.Template?.FindName("Thumb", slider) as Thumb;
            double thumbWidth = thumb?.ActualWidth ?? 24; // The default value is based on your style

            // Calculate the effective track range (taking into account the Thumb width)
            double effectiveTrackLeft = trackBounds.Left + thumbWidth / 2;
            double effectiveTrackWidth = trackBounds.Width - thumbWidth;
            if (effectiveTrackWidth <= 0) effectiveTrackWidth = trackBounds.Width;

            // Calculate the position of the bookmark on the valid track
            double relativePosition = (bookmarkValue - slider.Minimum) / range;
            double positionInSlider = effectiveTrackLeft + (relativePosition * effectiveTrackWidth);

            // Calculate the left position of the bookmark control
            double bookmarkHalfWidth = bookmarkControl.ActualWidth / 2;
            double leftPosition = positionInSlider - bookmarkHalfWidth;

            // Boundary check
            leftPosition = Math.Max(0, Math.Min(leftPosition, slider.ActualWidth - bookmarkControl.ActualWidth));

            // Calculate vertical position
            double verticalPosition;
            if (thumb != null && thumb.ActualHeight > 0)
            {
                Point thumbPosition = thumb.TransformToAncestor(slider).Transform(new Point(0, 0));
                double thumbCenterY = thumbPosition.Y + thumb.ActualHeight / 2;
                verticalPosition = thumbCenterY - bookmarkControl.ActualHeight / 2;
            }
            else
            {
                double trackCenterY = trackBounds.Top + trackBounds.Height / 2;
                verticalPosition = trackCenterY - bookmarkControl.ActualHeight / 2;
            }

            // Set Canvas Position
            Canvas.SetLeft(bookmarkControl, leftPosition);
            Canvas.SetTop(bookmarkControl, verticalPosition);
        }

        // Auxiliary method: Find child elements in the visual tree
        private static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // Check the current child element
                if (child is T result &&
                    ((child is FrameworkElement fe && fe.Name == childName) ||
                     (child is FrameworkElement fe2 && fe2.Tag?.ToString() == childName)))
                {
                    return result;
                }

                // Recursive search
                var childResult = FindChild<T>(child, childName);
                if (childResult != null) return childResult;
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// Bookmark Decorator
    /// </summary>
    public class BookmarkAdorner : Adorner
    {
        private readonly Canvas _bookmarkContainer;

        public BookmarkAdorner(UIElement adornedElement, Canvas bookmarkContainer)
            : base(adornedElement)
        {
            _bookmarkContainer = bookmarkContainer;
            AddVisualChild(bookmarkContainer);
        }

        public Canvas BookmarkContainer => _bookmarkContainer;

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _bookmarkContainer;

        protected override Size ArrangeOverride(Size finalSize)
        {
            _bookmarkContainer.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }
    }
}
