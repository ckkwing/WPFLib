using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Theme.Native;

namespace Theme.Extensions
{
    public static class ApplicationExtensions
    {
        ///// <summary>
        ///// Checks if code is runnung in designer, e.g. in Blend.
        ///// </summary>
        ///// <returns><c>True</c> if running in designer.</returns>
        //public static bool IsInDesignMode(this Application self)
        //{
        //    // This only provides diagnostic information, so it should not throw ArgumentNullExceptions.
        //    if (self == null)
        //        return false;

        //    // This assembly is directly referenced, so it's in the same folder or at least in a sub folder
        //    // If this assembly is in a totally different folder than the application, we assume it's the designer.
        //    var applicationAssemblyFolder = Path.GetDirectoryName(self.GetType().Assembly.Location);
        //    var executingAssemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //    return !executingAssemblyFolder.StartsWith(applicationAssemblyFolder, StringComparison.OrdinalIgnoreCase);
        //}

        ///// <summary>
        ///// Find the active window that is suitable as the owner of a new message box or modal dialog.
        ///// </summary>
        ///// <returns>The active window, or null if there is no suitable window.</returns>
        ///// <remarks>
        ///// The active window might be a popup, so the outermost enabled owner of the active window is returned.
        ///// </remarks>
        //public static Window FindActiveWindow(this Application self)
        //{
        //    if (self != null)
        //    {
        //        var activeWindows = self.Windows.OfType<Window>()
        //            .Where(window => window.IsLoaded && window.IsVisible && window.IsWindowEnabled() && !(window is BalloonWindow) && window.IsActive);

        //        if (activeWindows != null)
        //        {
        //            var activeWindow = activeWindows.FirstOrDefault();

        //            while (activeWindow != null)
        //            {
        //                var owner = activeWindow.Owner;
        //                if ((owner == null) || !owner.IsWindowEnabled() || !owner.IsVisible)
        //                {
        //                    return activeWindow;
        //                }

        //                activeWindow = owner;
        //            }
        //        }

        //        // Sometimes (mainly when running tests) no window is active: 
        //        // Use the window with the most owned windows that is available. 
        //        var enabledWindow = self.Windows.OfType<Window>()
        //            .Where(window => window.IsLoaded && window.IsVisible && window.IsWindowEnabled() && !(window is BalloonWindow))
        //            .OrderByDescending(window => window.OwnedWindows.Count)
        //            .FirstOrDefault();
        //        if (enabledWindow != null)
        //        {
        //            return enabledWindow;
        //        }
        //        //fixed bug 61108, when auto play, if the update required dialog popuped, the mainwindow will unenabled. 
        //        return self.Windows.OfType<Window>()
        //            .Where(window => window.IsLoaded && window.IsVisible && !(window is BalloonWindow))
        //            .OrderByDescending(window => window.OwnedWindows.Count)
        //            .FirstOrDefault();

        //    }

        //    return null;
        //}

        ///// <summary>
        ///// Find the handle of the active window that is suitable as the owner of a new message box or modal dialog.
        ///// </summary>
        ///// <returns>The handle of the active window, or null if there is no suitable window.</returns>
        ///// <remarks>
        ///// IMPORTANT!!!
        ///// The active window might be a license dialog; 
        ///// it will not be the WPF owner-owned chain and has to be searched with Win32 window factions
        ///// </remarks>
        //public static IntPtr FindActiveWindowHandle(this Application self)
        //{
        //    if (self != null)
        //    {
        //        Window mainWindow = self.Windows.OfType<Window>()
        //            .Where(window => window.IsLoaded && window.IsVisible)
        //            .OrderByDescending(window => window.OwnedWindows.Count)
        //            .FirstOrDefault();
        //        IntPtr popupHandle = mainWindow.GetHandle();
        //        // For avoiding using non-modal window, like a MiniHub, try to use the top enabled window
        //        while (IntPtr.Zero != popupHandle && !NativeMethods.IsWindowEnabled(popupHandle))
        //        {
        //            IntPtr ownedHandle = NativeMethods.GetWindow(popupHandle, NativeMethods.GW_ENABLEDPOPUP);
        //            if (IntPtr.Zero == ownedHandle)
        //                break;
        //            popupHandle = ownedHandle;
        //        }
        //        return popupHandle;
        //    }

        //    return IntPtr.Zero;
        //}

    }
}
