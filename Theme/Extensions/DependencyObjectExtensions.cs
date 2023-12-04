using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Theme.Native;

namespace Theme.Extensions
{
    public static class DependencyObjectExtensions
    {
        #region Window related extension methods

        /// <summary>
        /// Get the window handle (Hwnd) of the native window this object belongs to.
        /// </summary>
        public static IntPtr GetHandle(this DependencyObject window)
        {
            if (window != null)
            {
                var hwndSource = HwndSource.FromDependencyObject(window) as HwndSource;
                if (hwndSource != null)
                {
                    return hwndSource.Handle;
                }
                else
                {
                    Trace.Fail("unable to get window handle");
                }
            }
            else
            {
                Trace.TraceWarning("calling GetHandle() extension method on null window");
            }

            return IntPtr.Zero;
        }

        #endregion
    }
}
