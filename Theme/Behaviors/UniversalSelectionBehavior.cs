using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace Theme.Behaviors
{
    public class UniversalSelectionBehavior : Behavior<ListBox>
    {
        #region Dependency Property

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                "IsEnabled",
                typeof(bool),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(true, OnIsEnabledChanged));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register(
                "SelectionBrush",
                typeof(Brush),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(40, 0, 204, 204))));

        public Brush SelectionBrush
        {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        public static readonly DependencyProperty SelectionBorderBrushProperty =
            DependencyProperty.Register(
                "SelectionBorderBrush",
                typeof(Brush),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(Brushes.Cyan));

        public Brush SelectionBorderBrush
        {
            get { return (Brush)GetValue(SelectionBorderBrushProperty); }
            set { SetValue(SelectionBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty SelectionRectangleStyleProperty =
            DependencyProperty.Register(
                "SelectionRectangleStyle",
                typeof(Style),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(null));

        public Style SelectionRectangleStyle
        {
            get { return (Style)GetValue(SelectionRectangleStyleProperty); }
            set { SetValue(SelectionRectangleStyleProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemStyleProperty =
            DependencyProperty.Register(
                "SelectedItemStyle",
                typeof(Style),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(null));

        public Style SelectedItemStyle
        {
            get { return (Style)GetValue(SelectedItemStyleProperty); }
            set { SetValue(SelectedItemStyleProperty, value); }
        }

        public static readonly DependencyProperty ClearOnEmptyClickProperty =
            DependencyProperty.Register(
                "ClearOnEmptyClick",
                typeof(bool),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(true));

        public bool ClearOnEmptyClick
        {
            get { return (bool)GetValue(ClearOnEmptyClickProperty); }
            set { SetValue(ClearOnEmptyClickProperty, value); }
        }

        public static readonly DependencyProperty ClosePopupsOnEmptyClickProperty =
            DependencyProperty.Register(
                "ClosePopupsOnEmptyClick",
                typeof(bool),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(true));

        public bool ClosePopupsOnEmptyClick
        {
            get { return (bool)GetValue(ClosePopupsOnEmptyClickProperty); }
            set { SetValue(ClosePopupsOnEmptyClickProperty, value); }
        }

        public static readonly DependencyProperty ExcludedControlTypesProperty =
            DependencyProperty.Register(
                "ExcludedControlTypes",
                typeof(string),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata("Button,ComboBox,TextBox,Slider"));

        public string ExcludedControlTypes
        {
            get { return (string)GetValue(ExcludedControlTypesProperty); }
            set { SetValue(ExcludedControlTypesProperty, value); }
        }

        // Ctrl+A 全选功能是否启用
        public static readonly DependencyProperty EnableCtrlASelectAllProperty =
            DependencyProperty.Register(
                "EnableCtrlASelectAll",
                typeof(bool),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(true));

        public bool EnableCtrlASelectAll
        {
            get { return (bool)GetValue(EnableCtrlASelectAllProperty); }
            set { SetValue(EnableCtrlASelectAllProperty, value); }
        }

        // 是否启用标准选择行为（Shift/Ctrl选择）
        public static readonly DependencyProperty EnableStandardSelectionProperty =
            DependencyProperty.Register(
                "EnableStandardSelection",
                typeof(bool),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(true));

        public bool EnableStandardSelection
        {
            get { return (bool)GetValue(EnableStandardSelectionProperty); }
            set { SetValue(EnableStandardSelectionProperty, value); }
        }

        // 右键点击时是否保持选择
        public static readonly DependencyProperty KeepSelectionOnRightClickProperty =
            DependencyProperty.Register(
                "KeepSelectionOnRightClick",
                typeof(bool),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(true));

        public bool KeepSelectionOnRightClick
        {
            get { return (bool)GetValue(KeepSelectionOnRightClickProperty); }
            set { SetValue(KeepSelectionOnRightClickProperty, value); }
        }

        // 右键点击时保持选择后是否保持消息继续传递
        public static readonly DependencyProperty KeepEventBubledOnRightClickUpProperty =
            DependencyProperty.Register(
                "KeepEventBubledOnRightClickUp",
                typeof(bool),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(true));

        public bool KeepEventBubledOnRightClickUp
        {
            get { return (bool)GetValue(KeepEventBubledOnRightClickUpProperty); }
            set { SetValue(KeepEventBubledOnRightClickUpProperty, value); }
        }

        public static readonly DependencyProperty AutoFocusProperty =
            DependencyProperty.Register(
                "AutoFocus",
                typeof(bool),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(true));

        public bool AutoFocus
        {
            get { return (bool)GetValue(AutoFocusProperty); }
            set { SetValue(AutoFocusProperty, value); }
        }

        #endregion

        #region 私有字段

        private Canvas _selectionCanvas;
        private Rectangle _selectionRectangle;
        private Point _startPoint;
        private bool _isDragging;
        private bool _isInitialized;
        private List<Type> _excludedTypes = new List<Type>();
        private bool _isCtrlKeyPressed = false;
        private bool _isShiftKeyPressed = false;
        private object _lastSelectedItem = null;
        private bool _isRightClicking = false; // 跟踪右键点击状态
        private object _rightClickItem = null; // 右键点击的项目

        #endregion

        #region Commands

        public static readonly DependencyProperty SelectionStartedCommandProperty =
            DependencyProperty.Register("SelectionStartedCommand",
                typeof(ICommand),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(null));

        public ICommand SelectionStartedCommand
        {
            get { return (ICommand)GetValue(SelectionStartedCommandProperty); }
            set { SetValue(SelectionStartedCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectionCompletedCommandProperty =
            DependencyProperty.Register("SelectionCompletedCommand",
                typeof(ICommand),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(null));

        public ICommand SelectionCompletedCommand
        {
            get { return (ICommand)GetValue(SelectionCompletedCommandProperty); }
            set { SetValue(SelectionCompletedCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectionRectChangedCommandProperty =
            DependencyProperty.Register("SelectionRectChangedCommand",
                typeof(ICommand),
                typeof(UniversalSelectionBehavior),
                new PropertyMetadata(null));

        public ICommand SelectionRectChangedCommand
        {
            get { return (ICommand)GetValue(SelectionRectChangedCommandProperty); }
            set { SetValue(SelectionRectChangedCommandProperty, value); }
        }

        #endregion

        #region Behavior 生命周期

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.Unloaded += OnUnloaded;

            // 监听选择变化，更新最后选中的项目
            AssociatedObject.SelectionChanged += OnListBoxSelectionChanged;

            if (AssociatedObject.IsLoaded)
            {
                Initialize();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            Cleanup();

            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded -= OnLoaded;
                AssociatedObject.Unloaded -= OnUnloaded;
                AssociatedObject.SelectionChanged -= OnListBoxSelectionChanged;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Initialize();

            // 确保 ListBox 获得焦点
            if (AutoFocus)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (AssociatedObject.Focusable)
                    {
                        AssociatedObject.Focus();
                        Keyboard.Focus(AssociatedObject);
                    }
                }), DispatcherPriority.ContextIdle);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Cleanup();
        }

        // 监听ListBox选择变化
        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果正在处理右键点击，不更新最后选中的项目
            if (_isRightClicking) return;

            if (AssociatedObject.SelectedItems.Count > 0)
            {
                // 更新最后选中的项目
                _lastSelectedItem = AssociatedObject.SelectedItems[AssociatedObject.SelectedItems.Count - 1];
            }
        }

        #endregion

        #region 初始化

        private void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                // 初始化排除的类型
                InitializeExcludedTypes();

                // 创建选择画布
                CreateSelectionCanvas();

                // 将选择画布添加到视觉树中
                AttachSelectionCanvas();

                // 附加事件
                AttachEventHandlers();

                _isInitialized = true;

                Debug.WriteLine("UniversalSelectionBehavior 初始化成功");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UniversalSelectionBehavior 初始化失败: {ex.Message}");
            }
        }

        private void InitializeExcludedTypes()
        {
            _excludedTypes.Clear();
            var typeNames = ExcludedControlTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var typeName in typeNames)
            {
                try
                {
                    var type = Type.GetType($"System.Windows.Controls.{typeName}, PresentationFramework") ??
                               Type.GetType($"System.Windows.Controls.Primitives.{typeName}, PresentationFramework") ??
                               Type.GetType(typeName);

                    if (type != null)
                    {
                        _excludedTypes.Add(type);
                    }
                }
                catch
                {
                    // 忽略无法加载的类型
                }
            }

            // 默认添加一些常见类型
            if (!_excludedTypes.Contains(typeof(Button)))
                _excludedTypes.Add(typeof(Button));
            if (!_excludedTypes.Contains(typeof(ComboBox)))
                _excludedTypes.Add(typeof(ComboBox));
            if (!_excludedTypes.Contains(typeof(TextBox)))
                _excludedTypes.Add(typeof(TextBox));
        }

        private void CreateSelectionCanvas()
        {
            _selectionCanvas = new Canvas
            {
                IsHitTestVisible = false,
                Background = Brushes.Transparent,
                ClipToBounds = true
            };

            _selectionRectangle = new Rectangle
            {
                Visibility = Visibility.Collapsed,
                IsHitTestVisible = false
            };

            // 应用自定义样式或默认样式
            if (SelectionRectangleStyle != null)
            {
                _selectionRectangle.Style = SelectionRectangleStyle;
            }
            else
            {
                ApplyDefaultStyle(_selectionRectangle);
            }

            _selectionCanvas.Children.Add(_selectionRectangle);
        }

        private void ApplyDefaultStyle(Rectangle rectangle)
        {
            rectangle.Fill = SelectionBrush;
            rectangle.Stroke = SelectionBorderBrush;
            rectangle.StrokeThickness = 1;
            rectangle.Opacity = 0;

            // 添加阴影效果
            rectangle.Effect = new DropShadowEffect
            {
                ShadowDepth = 1,
                Color = Colors.Black,
                BlurRadius = 3,
                Opacity = 0.3
            };
        }

        private void AttachSelectionCanvas()
        {
            // 尝试多种方法附加选择画布
            if (!TryAttachToScrollViewer() &&
                !TryAttachToBorder() &&
                !TryAttachToAdornerLayer())
            {
                // 如果所有方法都失败，记录警告
                Debug.WriteLine("警告：无法附加选择画布，框选功能可能无法正常工作");
            }
        }

        private bool TryAttachToScrollViewer()
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(AssociatedObject);
            if (scrollViewer == null) return false;

            var content = scrollViewer.Content as UIElement;
            if (content == null) return false;

            var grid = new Grid();
            scrollViewer.Content = null;
            grid.Children.Add(content);
            grid.Children.Add(_selectionCanvas);
            scrollViewer.Content = grid;

            return true;
        }

        private bool TryAttachToBorder()
        {
            if (VisualTreeHelper.GetChildrenCount(AssociatedObject) == 0) return false;

            var child = VisualTreeHelper.GetChild(AssociatedObject, 0);
            if (child is Border border)
            {
                var originalChild = border.Child as UIElement;
                if (originalChild == null) return false;

                var grid = new Grid();
                border.Child = null;
                grid.Children.Add(originalChild);
                grid.Children.Add(_selectionCanvas);
                border.Child = grid;

                return true;
            }

            return false;
        }

        private bool TryAttachToAdornerLayer()
        {
            var adornerLayer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(AssociatedObject);
            if (adornerLayer == null) return false;

            adornerLayer.Add(new SelectionAdorner(AssociatedObject, _selectionCanvas));
            return true;
        }

        #endregion

        #region 事件处理 - 通用逻辑

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsEnabled) return;

            // ?? No needs below codes, because already set focus at the beginning
            // 如果启用自动焦点，确保 ListBox 获得焦点
            if (AutoFocus)
            {
                EnsureListBoxHasFocus();
            }

            // 更新按键状态
            _isCtrlKeyPressed = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            _isShiftKeyPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

            // 处理右键点击
            if (e.ChangedButton == MouseButton.Right)
            {
                HandleRightMouseDown(e);
                return;
            }

            // 处理左键点击
            if (e.ChangedButton != MouseButton.Left) return;

            // 分析点击
            var clickInfo = AnalyzeClick(e);

            // 处理点击
            switch (clickInfo)
            {
                case ClickInfo.EmptyArea:
                    HandleEmptyAreaClick(e);
                    break;

                case ClickInfo.ExcludedControl:
                    // 排除的控件，不处理
                    e.Handled = false;
                    break;

                case ClickInfo.ListBoxItem:
                    HandleListItemClick(e);
                    break;

                default:
                    e.Handled = false;
                    break;
            }
        }

        // 处理右键点击
        private void HandleRightMouseDown(MouseButtonEventArgs e)
        {
            _isRightClicking = true;

            // 获取点击的原始元素
            var originalSource = e.OriginalSource as DependencyObject;

            // 检查是否点击在ListBoxItem上
            if (IsListBoxItemOrChild(originalSource))
            {
                var listBoxItem = FindParent<ListBoxItem>(originalSource);
                if (listBoxItem != null)
                {
                    // 获取对应的数据项
                    var item = AssociatedObject.ItemContainerGenerator.ItemFromContainer(listBoxItem);
                    if (item != DependencyProperty.UnsetValue)
                    {
                        _rightClickItem = item;

                        // 检查是否已经选中
                        bool isAlreadySelected = AssociatedObject.SelectedItems.Contains(item);

                        // 如果 KeepSelectionOnRightClick 为 true 且项目已经被选中
                        // 不要清除选择
                        if (KeepSelectionOnRightClick && isAlreadySelected)
                        {
                            // 保持当前选择，只是打开右键菜单
                            // 不设置 e.Handled，让事件继续冒泡
                            e.Handled = false;
                            return;
                        }

                        // 否则处理标准右键选择逻辑
                        HandleRightClickSelection(item, e);
                    }
                }
            }

            // 处理空白区域右键
            HandleRightClickEmptyArea(e);
        }

        // 处理右键点击空白区域
        private void HandleRightClickEmptyArea(MouseButtonEventArgs e)
        {
            // 关闭打开的Popup
            if (ClosePopupsOnEmptyClick)
            {
                CloseOpenPopups();
            }

            // 如果没有按下 Ctrl 键或 Shift 键，清除选择
            if (ClearOnEmptyClick && !_isCtrlKeyPressed && !_isShiftKeyPressed)
            {
                ClearSelections();
                _lastSelectedItem = null;
            }
        }

        // 处理右键选择
        private void HandleRightClickSelection(object item, MouseButtonEventArgs e)
        {
            // 检查是否按了Ctrl或Shift
            bool hasModifier = (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) != 0;

            if (!hasModifier)
            {
                //// 检查项目是否已经选中
                //bool isAlreadySelected = AssociatedObject.SelectedItems.Contains(item);

                //// 如果项目已经选中，不改变选择状态（保持选中）
                //if (isAlreadySelected)
                //{
                //    // 保持当前选择，不进行任何操作
                //    _lastSelectedItem = item;
                //    return;
                //}

                // 普通右键点击
                if (ClearOnEmptyClick)
                {
                    ClearSelections();
                }

                // 选中当前项
                AssociatedObject.SelectedItems.Add(item);
                _lastSelectedItem = item;
            }
            else if (_isShiftKeyPressed && _lastSelectedItem != null)
            {
                // Shift+右键：范围选择
                HandleShiftClickSelection(item);
            }
            else if (_isCtrlKeyPressed)
            {
                // Ctrl+右键：多选
                HandleCtrlClickSelection(item, null);
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            // 处理右键释放
            if (e.ChangedButton == MouseButton.Right)
            {
                _isRightClicking = false;
                _rightClickItem = null;
                return;
            }

            // 处理左键释放
            if (!_isDragging) return;

            _isDragging = false;

            // 释放鼠标捕获
            if (AssociatedObject.IsMouseCaptured)
            {
                AssociatedObject.ReleaseMouseCapture();
            }

            // 淡出动画
            var animation = new System.Windows.Media.Animation.DoubleAnimation(0.8, 0,
                TimeSpan.FromMilliseconds(150));
            animation.Completed += (s, args) =>
            {
                if (_selectionRectangle != null)
                {
                    _selectionRectangle.Visibility = Visibility.Collapsed;
                }
            };
            _selectionRectangle.BeginAnimation(UIElement.OpacityProperty, animation);

            e.Handled = true;
        }

        private ClickInfo AnalyzeClick(MouseButtonEventArgs e)
        {
            // 获取点击的原始元素
            var originalSource = e.OriginalSource as DependencyObject;

            // 检查是否是排除的控件类型
            if (IsExcludedControl(originalSource))
            {
                return ClickInfo.ExcludedControl;
            }

            // 检查是否点击在ListBoxItem上
            if (IsListBoxItemOrChild(originalSource))
            {
                return ClickInfo.ListBoxItem;
            }

            // 检查是否点击在Popup内容上
            if (IsInPopup(originalSource))
            {
                return ClickInfo.ExcludedControl;
            }

            // 否则认为是空白区域
            return ClickInfo.EmptyArea;
        }

        private bool IsExcludedControl(DependencyObject element)
        {
            while (element != null)
            {
                var elementType = element.GetType();

                // 检查是否是排除的类型
                foreach (var excludedType in _excludedTypes)
                {
                    if (excludedType.IsAssignableFrom(elementType))
                    {
                        return true;
                    }
                }

                element = VisualTreeHelper.GetParent(element);
            }

            return false;
        }

        private bool IsListBoxItemOrChild(DependencyObject element)
        {
            while (element != null && element != AssociatedObject)
            {
                if (element is ListBoxItem)
                {
                    return true;
                }
                element = VisualTreeHelper.GetParent(element);
            }

            return false;
        }

        private bool IsInPopup(DependencyObject element)
        {
            // 通过检查元素是否在Popup的视觉树中判断
            var popups = FindOpenPopups();
            foreach (var popup in popups)
            {
                if (popup.Child != null && IsVisualChildOf(popup.Child, element))
                {
                    return true;
                }
            }

            return false;
        }

        private List<Popup> FindOpenPopups()
        {
            var popups = new List<Popup>();
            FindOpenPopupsRecursive(Application.Current.MainWindow, popups);
            return popups;
        }

        private void FindOpenPopupsRecursive(DependencyObject parent, List<Popup> result)
        {
            if (parent == null) return;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Popup popup && popup.IsOpen)
                {
                    result.Add(popup);
                }

                FindOpenPopupsRecursive(child, result);
            }
        }

        private bool IsVisualChildOf(DependencyObject parent, DependencyObject child)
        {
            while (child != null)
            {
                if (child == parent)
                {
                    return true;
                }
                child = VisualTreeHelper.GetParent(child);
            }
            return false;
        }

        private void HandleEmptyAreaClick(MouseButtonEventArgs e)
        {
            // 关闭打开的Popup
            if (ClosePopupsOnEmptyClick)
            {
                CloseOpenPopups();
            }

            // 如果没有按下 Ctrl 键或 Shift 键，清除选择
            if (ClearOnEmptyClick && !_isCtrlKeyPressed && !_isShiftKeyPressed)
            {
                ClearSelections();
                _lastSelectedItem = null;
            }

            // 开始框选
            StartSelection(e);

            e.Handled = true;
        }

        private void HandleListItemClick(MouseButtonEventArgs e)
        {
            if (!EnableStandardSelection)
            {
                // 如果禁用标准选择行为，使用原有逻辑
                HandleListItemClickOriginal(e);
                return;
            }

            // 获取点击的 ListBoxItem
            var originalSource = e.OriginalSource as DependencyObject;
            var listBoxItem = FindParent<ListBoxItem>(originalSource);

            if (listBoxItem != null)
            {
                // 获取对应的数据项
                var item = AssociatedObject.ItemContainerGenerator.ItemFromContainer(listBoxItem);

                if (item != DependencyProperty.UnsetValue)
                {
                    // 处理 Shift 键范围选择
                    if (_isShiftKeyPressed && _lastSelectedItem != null)
                    {
                        HandleShiftClickSelection(item);
                        e.Handled = true;
                        return;
                    }

                    // 处理 Ctrl 键多选
                    if (_isCtrlKeyPressed)
                    {
                        HandleCtrlClickSelection(item, listBoxItem);
                        e.Handled = true;
                        return;
                    }

                    // 普通点击
                    HandleNormalClickSelection(item);
                    e.Handled = true;
                    return;
                }
            }

            // 如果上面的逻辑没有处理，使用原有逻辑
            e.Handled = false;
        }

        // 保持原有的逻辑（当禁用标准选择时使用）
        private void HandleListItemClickOriginal(MouseButtonEventArgs e)
        {
            // 检查是否按了Ctrl或Shift
            bool hasModifier = (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) != 0;

            if (!hasModifier && ClearOnEmptyClick)
            {
                // 普通点击，可能需要在鼠标释放时清除其他选择
                // 这里让ListBox处理点击
                e.Handled = false;
            }
            else
            {
                // 有修饰键，开始框选
                StartSelection(e);
                e.Handled = true;
            }
        }

        // 处理普通点击选择
        private void HandleNormalClickSelection(object item)
        {
            if (AssociatedObject.SelectionMode != SelectionMode.Multiple &&
                AssociatedObject.SelectionMode != SelectionMode.Extended)
                return;

            // 清除其他选择
            if (ClearOnEmptyClick)
            {
                ClearSelections();
            }

            // 选中当前项
            AssociatedObject.SelectedItems.Add(item);
            _lastSelectedItem = item;
        }

        // 新增：处理 Shift 键范围选择
        private void HandleShiftClickSelection(object item)
        {
            if (AssociatedObject.SelectionMode != SelectionMode.Multiple &&
                AssociatedObject.SelectionMode != SelectionMode.Extended)
                return;

            // 如果没有最后选中的项目，只选中当前项
            if (_lastSelectedItem == null)
            {
                HandleNormalClickSelection(item);
                return;
            }

            // 获取两个项目的索引
            int lastIndex = AssociatedObject.Items.IndexOf(_lastSelectedItem);
            int currentIndex = AssociatedObject.Items.IndexOf(item);

            if (lastIndex < 0 || currentIndex < 0)
                return;

            // 确定范围
            int startIndex = Math.Min(lastIndex, currentIndex);
            int endIndex = Math.Max(lastIndex, currentIndex);

            // 如果没有按下 Ctrl 键，清除其他选择
            if (!_isCtrlKeyPressed)
            {
                ClearSelections();
            }

            // 选择范围内的所有项目
            for (int i = startIndex; i <= endIndex; i++)
            {
                var rangeItem = AssociatedObject.Items[i];
                if (!AssociatedObject.SelectedItems.Contains(rangeItem))
                {
                    AssociatedObject.SelectedItems.Add(rangeItem);
                }
            }

            // 更新最后选中的项目
            _lastSelectedItem = item;
        }

        // 处理 Ctrl 键点击选择
        private void HandleCtrlClickSelection(object item, ListBoxItem listBoxItem)
        {
            if (AssociatedObject.SelectionMode != SelectionMode.Multiple &&
                AssociatedObject.SelectionMode != SelectionMode.Extended)
                return;

            // 如果已经选中，则取消选中
            if (AssociatedObject.SelectedItems.Contains(item))
            {
                AssociatedObject.SelectedItems.Remove(item);
                // 如果取消了最后选中的项目，需要更新_lastSelectedItem
                if (item == _lastSelectedItem)
                {
                    _lastSelectedItem = AssociatedObject.SelectedItems.Count > 0 ?
                        AssociatedObject.SelectedItems[AssociatedObject.SelectedItems.Count - 1] : null;
                }
            }
            else
            {
                // 添加当前项到选择
                AssociatedObject.SelectedItems.Add(item);
                _lastSelectedItem = item;
            }
        }

        // 键盘按键处理（用于 Ctrl+A）
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEnabled) return;

            // 检查是否是 Ctrl+A
            if (EnableCtrlASelectAll && e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                HandleCtrlASelectAll();
                e.Handled = true;
            }
            // 更新按键状态
            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                _isCtrlKeyPressed = true;
            }
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                _isShiftKeyPressed = true;
            }
        }

        // 键盘按键释放处理
        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            // 更新按键状态
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                _isCtrlKeyPressed = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            }
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                _isShiftKeyPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            }
        }

        // 处理鼠标捕获丢失
        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            // 如果正在拖动，强制结束选择
            if (_isDragging)
            {
                ForceEndSelection();
            }
        }

        // 处理失去焦点
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            // 如果正在拖动，强制结束选择
            if (_isDragging)
            {
                ForceEndSelection();
            }
        }

        // 处理窗口失去激活状态
        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            // 如果正在拖动，强制结束选择
            if (_isDragging)
            {
                ForceEndSelection();
            }
        }

        // 处理窗口失去焦点
        private void OnWindowLostFocus(object sender, RoutedEventArgs e)
        {
            // 如果正在拖动，强制结束选择
            if (_isDragging)
            {
                ForceEndSelection();
            }
        }

        // 强制结束选择
        private void ForceEndSelection()
        {
            if (!_isDragging) return;

            _isDragging = false;

            // 淡出动画
            if (_selectionRectangle != null)
            {
                var animation = new DoubleAnimation(0.8, 0,
                    TimeSpan.FromMilliseconds(150));
                animation.Completed += (s, args) =>
                {
                    if (_selectionRectangle != null)
                    {
                        _selectionRectangle.Visibility = Visibility.Collapsed;
                    }
                };
                _selectionRectangle.BeginAnimation(UIElement.OpacityProperty, animation);
            }

            // 释放鼠标捕获
            if (AssociatedObject.IsMouseCaptured)
            {
                AssociatedObject.ReleaseMouseCapture();
            }
        }

        // 处理 Ctrl+A 全选
        private void HandleCtrlASelectAll()
        {
            if (AssociatedObject.SelectionMode == SelectionMode.Single)
                return;

            try
            {
                // 清空当前选择
                AssociatedObject.SelectedItems.Clear();

                // 添加所有项
                foreach (var item in AssociatedObject.Items)
                {
                    AssociatedObject.SelectedItems.Add(item);
                }

                // 更新最后选中的项目
                if (AssociatedObject.SelectedItems.Count > 0)
                {
                    _lastSelectedItem = AssociatedObject.SelectedItems[AssociatedObject.SelectedItems.Count - 1];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ctrl+A 全选失败: {ex.Message}");
            }
        }

        private void StartSelection(MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(_selectionCanvas);
            _isDragging = true;

            // 捕获鼠标，确保即使鼠标移出窗口也能收到鼠标事件
            if (!AssociatedObject.IsMouseCaptured)
            {
                AssociatedObject.CaptureMouse();
            }

            // 如果没有按下 Ctrl 键或 Shift 键，清除选择
            if (!_isCtrlKeyPressed && !_isShiftKeyPressed && ClearOnEmptyClick)
            {
                ClearSelections();
                _lastSelectedItem = null;
            }

            // 初始化选择矩形
            Canvas.SetLeft(_selectionRectangle, _startPoint.X);
            Canvas.SetTop(_selectionRectangle, _startPoint.Y);
            _selectionRectangle.Width = 0;
            _selectionRectangle.Height = 0;
            _selectionRectangle.Visibility = Visibility.Visible;

            // 淡入动画
            var animation = new System.Windows.Media.Animation.DoubleAnimation(0, 0.8,
                TimeSpan.FromMilliseconds(150));
            _selectionRectangle.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !IsEnabled) return;

            var currentPoint = e.GetPosition(_selectionCanvas);

            var left = Math.Min(_startPoint.X, currentPoint.X);
            var top = Math.Min(_startPoint.Y, currentPoint.Y);
            var width = Math.Abs(_startPoint.X - currentPoint.X);
            var height = Math.Abs(_startPoint.Y - currentPoint.Y);

            // 更新选择矩形
            Canvas.SetLeft(_selectionRectangle, left);
            Canvas.SetTop(_selectionRectangle, top);
            _selectionRectangle.Width = width;
            _selectionRectangle.Height = height;

            // 更新选中项目
            UpdateSelectedItems(new Rect(left, top, width, height));

            e.Handled = true;
        }

        #endregion

        #region 选择逻辑

        // 确保 ListBox 获得焦点的方法
        private void EnsureListBoxHasFocus()
        {
            if (!AssociatedObject.IsKeyboardFocusWithin && AssociatedObject.Focusable)
            {
                // 异步设置焦点，避免干扰当前事件处理
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    if (AssociatedObject.Focusable && !AssociatedObject.IsKeyboardFocusWithin)
                    {
                        bool success = AssociatedObject.Focus();
                        Debug.WriteLine($"Ensure focus: {success}, Current focus: {Keyboard.FocusedElement?.GetType().Name}");
                    }
                }), DispatcherPriority.Input);
            }
        }

        private void ClearSelections()
        {
            if (AssociatedObject.SelectionMode == SelectionMode.Multiple ||
                AssociatedObject.SelectionMode == SelectionMode.Extended)
            {
                AssociatedObject.SelectedItems.Clear();
            }
            else if (AssociatedObject.SelectionMode == SelectionMode.Single)
            {
                AssociatedObject.SelectedItem = null;
            }
        }

        private void UpdateSelectedItems(Rect selectionRect)
        {
            if (!IsEnabled || AssociatedObject.SelectionMode == SelectionMode.Single)
                return;

            // 转换坐标
            var listBoxRect = ConvertCanvasRectToListBoxRect(selectionRect);

            foreach (var item in AssociatedObject.Items)
            {
                var container = AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (container != null && container.IsVisible)
                {
                    try
                    {
                        var itemBounds = GetItemBoundsInListBox(container);
                        var isSelected = listBoxRect.IntersectsWith(itemBounds);

                        if (isSelected)
                        {
                            if (!AssociatedObject.SelectedItems.Contains(item))
                            {
                                AssociatedObject.SelectedItems.Add(item);
                                // 在框选时也更新最后选中的项目
                                _lastSelectedItem = item;
                            }

                            ApplySelectedStyle(container, true);
                        }
                        else
                        {
                            // 如果按下 Ctrl 键或 Shift 键，保持原有选择，否则移除
                            if (!_isCtrlKeyPressed && !_isShiftKeyPressed && AssociatedObject.SelectedItems.Contains(item))
                            {
                                AssociatedObject.SelectedItems.Remove(item);
                            }

                            ApplySelectedStyle(container, false);
                        }
                    }
                    catch
                    {
                        // 忽略错误
                    }
                }
            }
        }

        [Obsolete("开发中，请勿调用", false)]
        private void ApplySelectedStyle(ListBoxItem item, bool isSelected)
        {
            return;

            if (null == item)
                return;

            if (isSelected)
            {
                if (SelectedItemStyle != null)
                {
                    // 使用Style资源
                    if (item.Style != SelectedItemStyle)
                    {
                        item.Style = SelectedItemStyle;
                    }
                }
                else
                {
                    // 应用默认选中样式
                    item.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 204, 204));
                    item.BorderThickness = new Thickness(2);
                    item.Background = new LinearGradientBrush(
                        Color.FromArgb(30, 0, 204, 204),
                        Color.FromArgb(15, 0, 204, 204),
                        new Point(0, 0),
                        new Point(0, 1));
                }
            }
            else
            {
                // 恢复原始样式
                if (item.Style == SelectedItemStyle)
                {
                    item.ClearValue(FrameworkElement.StyleProperty);
                }

                // 清除动态设置的属性
                item.ClearValue(ListBoxItem.BorderBrushProperty);
                item.ClearValue(ListBoxItem.BorderThicknessProperty);
                item.ClearValue(ListBoxItem.BackgroundProperty);
            }
        }

        private Rect ConvertCanvasRectToListBoxRect(Rect canvasRect)
        {
            if (_selectionCanvas == null) return canvasRect;

            try
            {
                if (_selectionCanvas.Parent is Visual parentVisual)
                {
                    var transform = parentVisual.TransformToDescendant(AssociatedObject);
                    if (transform != null)
                    {
                        var topLeft = transform.Transform(new Point(canvasRect.Left, canvasRect.Top));
                        var bottomRight = transform.Transform(new Point(canvasRect.Right, canvasRect.Bottom));
                        return new Rect(topLeft, bottomRight);
                    }
                }
            }
            catch
            {
                // 忽略转换错误
            }

            return canvasRect;
        }

        private Rect GetItemBoundsInListBox(ListBoxItem item)
        {
            try
            {
                var transform = item.TransformToAncestor(AssociatedObject);
                var itemPosition = transform.Transform(new Point(0, 0));
                return new Rect(itemPosition, new Size(item.ActualWidth, item.ActualHeight));
            }
            catch
            {
                return new Rect(0, 0, 0, 0);
            }
        }

        #endregion

        #region 辅助方法

        private void AttachEventHandlers()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseDown;
            AssociatedObject.PreviewMouseRightButtonDown += OnMouseDown; // 右键按下
            AssociatedObject.PreviewMouseMove += OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += OnMouseUp;
            AssociatedObject.PreviewMouseRightButtonUp += OnMouseUp; // 右键释放
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown; // 键盘按下
            AssociatedObject.PreviewKeyUp += OnPreviewKeyUp;     // 新增盘释放
            //AssociatedObject.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), true); // 使用 handledEventsToo = true
            //AssociatedObject.AddHandler(UIElement.PreviewKeyUpEvent, new KeyEventHandler(OnPreviewKeyUp), true);
            // 处理鼠标捕获丢失和失去焦点
            AssociatedObject.LostMouseCapture += OnLostMouseCapture;
            AssociatedObject.LostFocus += OnLostFocus;

            // 监听窗口激活状态变化
            if (Window.GetWindow(AssociatedObject) != null)
            {
                Window.GetWindow(AssociatedObject).Deactivated += OnWindowDeactivated;
                Window.GetWindow(AssociatedObject).LostFocus += OnWindowLostFocus;
            }

            // 处理 ListBox 的默认行为
            AssociatedObject.AddHandler(ListBoxItem.PreviewMouseRightButtonDownEvent,
                new MouseButtonEventHandler(OnListBoxItemRightButtonDown), true);
            AssociatedObject.AddHandler(ListBoxItem.PreviewMouseRightButtonUpEvent,
                new MouseButtonEventHandler(OnListBoxItemRightButtonUp), true);
        }

        private void DetachEventHandlers()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseDown;
            AssociatedObject.PreviewMouseRightButtonDown -= OnMouseDown; // 右键按下
            AssociatedObject.PreviewMouseMove -= OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnMouseUp;
            AssociatedObject.PreviewMouseRightButtonUp -= OnMouseUp; // 右键释放
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown; // 键盘按下
            AssociatedObject.PreviewKeyUp -= OnPreviewKeyUp;     // 键盘释放
            //AssociatedObject.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));
            //AssociatedObject.RemoveHandler(UIElement.PreviewKeyUpEvent, new KeyEventHandler(OnPreviewKeyUp));
            // 移除鼠标捕获和焦点事件
            AssociatedObject.LostMouseCapture -= OnLostMouseCapture;
            AssociatedObject.LostFocus -= OnLostFocus;

            // 移除窗口事件
            var window = Window.GetWindow(AssociatedObject);
            if (window != null)
            {
                window.Deactivated -= OnWindowDeactivated;
                window.LostFocus -= OnWindowLostFocus;
            }

            // 移除 ListBoxItem 的事件处理
            AssociatedObject.RemoveHandler(ListBoxItem.PreviewMouseRightButtonDownEvent,
                new MouseButtonEventHandler(OnListBoxItemRightButtonDown));
            AssociatedObject.RemoveHandler(ListBoxItem.PreviewMouseRightButtonUpEvent,
                new MouseButtonEventHandler(OnListBoxItemRightButtonUp));
        }

        // 处理 ListBoxItem 的右键按下事件
        private void OnListBoxItemRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsEnabled) return;

            var listBoxItem = FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);
            if (listBoxItem != null)
            {
                // 标记为已处理，防止 ListBox 的默认行为清除选择
                if (KeepSelectionOnRightClick && AssociatedObject.SelectedItems.Count > 0)
                {
                    // 保持选择，但不阻止事件传播
                    // 事件应该继续传播以触发右键菜单
                    e.Handled = true;
                }
            }
        }

        // 处理 ListBoxItem 的右键释放事件
        private void OnListBoxItemRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsEnabled) return;

            var listBoxItem = FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);
            if (listBoxItem != null)
            {
                // 如果右键点击已选中的项目，保持选择
                if (KeepSelectionOnRightClick)
                {
                    if (!KeepEventBubledOnRightClickUp)
                        e.Handled = true;
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                {
                    return result;
                }

                var descendant = FindVisualChild<T>(child);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        // 查找父元素
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }

        private void CloseOpenPopups()
        {
            var popups = FindOpenPopups();
            foreach (var popup in popups)
            {
                popup.IsOpen = false;
            }
        }

        private void Cleanup()
        {
            if (!_isInitialized) return;

            DetachEventHandlers();

            // 确保释放鼠标捕获
            if (AssociatedObject.IsMouseCaptured)
            {
                AssociatedObject.ReleaseMouseCapture();
            }

            if (_selectionCanvas != null && _selectionCanvas.Parent is Panel parentPanel)
            {
                parentPanel.Children.Remove(_selectionCanvas);
            }

            _selectionCanvas = null;
            _selectionRectangle = null;
            _isInitialized = false;
            _lastSelectedItem = null;
            _isRightClicking = false;
            _rightClickItem = null;
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (UniversalSelectionBehavior)d;
            if (!(bool)e.NewValue && behavior._isDragging)
            {
                behavior._isDragging = false;
                if (behavior._selectionRectangle != null)
                {
                    behavior._selectionRectangle.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region 内部类型

        private enum ClickInfo
        {
            EmptyArea,
            ExcludedControl,
            ListBoxItem
        }

        private class SelectionAdorner : System.Windows.Documents.Adorner
        {
            private readonly Canvas _canvas;

            public SelectionAdorner(UIElement adornedElement, Canvas canvas)
                : base(adornedElement)
            {
                _canvas = canvas;
                AddVisualChild(canvas);
            }

            protected override int VisualChildrenCount => 1;

            protected override Visual GetVisualChild(int index) => _canvas;

            protected override Size ArrangeOverride(Size finalSize)
            {
                _canvas.Arrange(new Rect(finalSize));
                return finalSize;
            }
        }

        #endregion
    }
}
