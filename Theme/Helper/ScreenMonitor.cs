using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using Theme.ViewModels;

namespace Theme.Helper
{
    /// <summary>
    /// A simplified screen detection class that supports detecting horizontally arranged multiple screens, unit is pixel
    /// </summary>
    public class ScreenMonitor : NotifyPropertyObject, IDisposable
    {
        #region Events

        public event EventHandler DisplaySettingsChanged;

        #endregion

        public Screen PrimaryScreen
        {
            get => Screen.PrimaryScreen;
        }

        private IList<Screen> _sortedScreens;
        public IList<Screen> SortedScreens
        {
            get
            {
                if (null == _sortedScreens)
                {
                    _sortedScreens = Screen.AllScreens.OrderBy(s => s.Bounds.X).ToList();
                }
                return _sortedScreens;
            }
        }

        private Rectangle _workingArea = new Rectangle();
        public Rectangle WorkingArea
        {
            get
            {
                if (_workingArea.IsEmpty)
                {
                    int? x = null;
                    int? y = null;
                    int width = 0;
                    int height = 0;
                    foreach (Screen screen in SortedScreens)
                    {
                        if (!x.HasValue)
                            x = screen.WorkingArea.X;
                        if (!y.HasValue)
                            y = screen.WorkingArea.Y;
                        width += screen.WorkingArea.Width;
                        height += screen.WorkingArea.Height;
                    }
                    _workingArea = new Rectangle(x.Value, y.Value, width, height);
                }
                return _workingArea;
            }
        }

        private Rectangle _screenArea = new Rectangle();
        public Rectangle ScreenArea
        {
            get
            {
                if (_screenArea.IsEmpty)
                {
                    int? x = null;
                    int? y = null;
                    int width = 0;
                    int height = 0;
                    foreach (Screen screen in SortedScreens)
                    {
                        if (!x.HasValue)
                            x = screen.Bounds.X;
                        if (!y.HasValue)
                            y = screen.Bounds.Y;
                        width += screen.Bounds.Width;
                        height += screen.Bounds.Height;
                    }
                    _screenArea = new Rectangle(x.Value, y.Value, width, height);
                }
                return _screenArea;
            }
        }

        public ScreenMonitor()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Reset();
            DisplaySettingsChanged?.Invoke(this, e);
        }

        public void Reset()
        {
            _sortedScreens = null;
            _workingArea = new Rectangle();
            _screenArea = new Rectangle();
        }

        public bool IsPointOnAnyScreen(Point point)
        {
            return Screen.AllScreens.Any(screen => screen.Bounds.Contains(point));
        }

        public bool IsPointOnAnyWorkingArea(Point point)
        {
            return Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(point));
        }

        public bool AreDisjoint(System.Windows.Rect rect1, System.Windows.Rect rect2)
        {
            return !rect1.IntersectsWith(rect2);
        }

        public bool IsDisjointScreen(System.Windows.Rect rect)
        {
            return AreDisjoint(rect, RectangleToRect(ScreenArea));
        }

        public bool IsDisjointWorkingArea(System.Windows.Rect rect)
        {
            return AreDisjoint(rect, RectangleToRect(WorkingArea));
        }

        private System.Windows.Rect RectangleToRect(Rectangle rectangle)
        {
            return new System.Windows.Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public bool ContentHasIntersectionWithScreenByPercent(System.Windows.Rect rect, double percent = 0.5)
        {
            return IntersectChecker.HasIntersectionByPercent(rect, RectangleToRect(ScreenArea), percent);
        }

        public bool ContentHasIntersectionWithWorkingAreaByPercent(System.Windows.Rect rect, double percent = 0.5)
        {
            return IntersectChecker.HasIntersectionByPercent(rect, RectangleToRect(WorkingArea), percent);
        }

        #region Static functions

        public static Screen GetCurrentScreen(System.Windows.Window window)
        {
            var windowInteropHelper = new WindowInteropHelper(window);
            IntPtr handle = windowInteropHelper.Handle;

            return Screen.FromHandle(handle);
        }

        public static Dictionary<Screen, string> GetMonitorPositions()
        {
            var result = new Dictionary<Screen, string>();
            var screens = Screen.AllScreens;

            if (screens.Length == 1)
            {
                result.Add(screens[0], "单一显示器");
                return result;
            }

            // 按X坐标排序显示器
            var sortedScreens = screens.OrderBy(s => s.Bounds.X).ToArray();

            for (int i = 0; i < sortedScreens.Length; i++)
            {
                string position;

                if (i == 0)
                {
                    position = "最左侧显示器";
                }
                else if (i == sortedScreens.Length - 1)
                {
                    position = "最右侧显示器";
                }
                else
                {
                    position = $"中间显示器 ({i + 1}/{sortedScreens.Length})";
                }

                result.Add(sortedScreens[i], position);
            }

            return result;
        }

        public static Dictionary<Screen, string> GetMonitorPositionsRelativeToPrimary()
        {
            var result = new Dictionary<Screen, string>();
            var screens = Screen.AllScreens;
            var primaryScreen = Screen.PrimaryScreen;

            foreach (var screen in screens)
            {
                if (screen.Equals(primaryScreen))
                {
                    result.Add(screen, "主显示器");
                    continue;
                }

                // 判断相对位置
                if (screen.Bounds.X < primaryScreen.Bounds.X)
                {
                    result.Add(screen, "位于主显示器左侧");
                }
                else if (screen.Bounds.X > primaryScreen.Bounds.X)
                {
                    result.Add(screen, "位于主显示器右侧");
                }
                else
                {
                    // X坐标相同，检查Y坐标
                    if (screen.Bounds.Y < primaryScreen.Bounds.Y)
                    {
                        result.Add(screen, "位于主显示器上方");
                    }
                    else if (screen.Bounds.Y > primaryScreen.Bounds.Y)
                    {
                        result.Add(screen, "位于主显示器下方");
                    }
                    else
                    {
                        result.Add(screen, "与主显示器位置重叠");
                    }
                }
            }

            return result;
        }

        #endregion

        #region IDisposable Members

        protected bool _alreadyDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
                return;
            if (isDisposing)
            {
                //release managed resources
                SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            }
            //release unmanaged resources
            _alreadyDisposed = true;
        }

        ~ScreenMonitor()
        {
            Dispose(false);
        }

        #endregion
    }
}
