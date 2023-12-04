using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Theme.Native;

namespace Theme.Extensions
{
    public static class WindowExtensions
    {
        /// <summary>
        /// Checks if a window is natively enabled.
        /// </summary>
        /// <returns>true if WS_DISABLED is set, false otherwise</returns>
        public static bool IsWindowEnabled(this Window window)
        {
            return NativeMethods.IsWindowEnabled(window.GetHandle());
        }
    }
}
