using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Theme.Native;
using static Theme.Native.NativeMethods;

namespace Theme.Helper
{
    /// <summary>
    /// 物理像素 (px) = DIP × (当前DPI缩放比例)
    /// 缩放比例	系统 DPI	缩放因子
    ///100%	96	1.0
    ///125%	120	1.25
    ///150%	144	1.5
    ///175%	168	1.75
    ///200%	192	2.0
    ///250%	240	2.5
    /// </summary>
    public static class DpiHelper
    {
        #region Windows 10+
        /// <example>
        /// IntPtr hwnd = new WindowInteropHelper(this).Handle;
        /// uint dpi = NativeMethods.Win10PlusNativeMethods.GetDpiForWindow(hwnd);
        /// </example>
        #endregion

        private static Matrix _transformToDevice;
        private static Matrix _transformToDip;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DpiHelper()
        {
            using (SafeDC desktop = SafeDC.GetDesktop())
            {
                // Can get these in the static constructor.  They shouldn't vary window to window,
                // and changing the system DPI requires a restart.
                int pixelsPerInchX = NativeMethods.GetDeviceCaps(desktop, DeviceCap.LOGPIXELSX);
                int pixelsPerInchY = NativeMethods.GetDeviceCaps(desktop, DeviceCap.LOGPIXELSY);

                _transformToDip = Matrix.Identity;
                _transformToDip.Scale(96d / (double)pixelsPerInchX, 96d / (double)pixelsPerInchY);
                _transformToDevice = Matrix.Identity;
                _transformToDevice.Scale((double)pixelsPerInchX / 96d, (double)pixelsPerInchY / 96d);
            }
        }

        /// <summary>
        /// Convert a point in device independent pixels (1/96") to a point in the system coordinates.
        /// </summary>
        /// <param name="logicalPoint">A point in the logical coordinate system.</param>
        /// <returns>Returns the parameter converted to the system's coordinates.</returns>
        public static Point LogicalPixelsToDevice(Point logicalPoint)
        {
            return _transformToDevice.Transform(logicalPoint);
        }

        /// <summary>
        /// Convert a point in system coordinates to a point in device independent pixels (1/96").
        /// </summary>
        /// <param name="logicalPoint">A point in the physical coordinate system.</param>
        /// <returns>Returns the parameter converted to the device independent coordinate system.</returns>
        public static Point DevicePixelsToLogical(Point devicePoint)
        {
            return _transformToDip.Transform(devicePoint);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Rect LogicalRectToDevice(Rect logicalRectangle)
        {
            Point topLeft = LogicalPixelsToDevice(new Point(logicalRectangle.Left, logicalRectangle.Top));
            Point bottomRight = LogicalPixelsToDevice(new Point(logicalRectangle.Right, logicalRectangle.Bottom));

            return new Rect(topLeft, bottomRight);
        }

        public static Rect DeviceRectToLogical(Rect deviceRectangle)
        {
            Point topLeft = DevicePixelsToLogical(new Point(deviceRectangle.Left, deviceRectangle.Top));
            Point bottomRight = DevicePixelsToLogical(new Point(deviceRectangle.Right, deviceRectangle.Bottom));

            return new Rect(topLeft, bottomRight);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Size LogicalSizeToDevice(Size logicalSize)
        {
            Point pt = LogicalPixelsToDevice(new Point(logicalSize.Width, logicalSize.Height));

            return new Size { Width = pt.X, Height = pt.Y };
        }

        public static Size DeviceSizeToLogical(Size deviceSize)
        {
            Point pt = DevicePixelsToLogical(new Point(deviceSize.Width, deviceSize.Height));

            return new Size(pt.X, pt.Y);
        }

        public static Thickness LogicalThicknessToDevice(Thickness logicalThickness)
        {
            Point topLeft = LogicalPixelsToDevice(new Point(logicalThickness.Left, logicalThickness.Top));
            Point bottomRight = LogicalPixelsToDevice(new Point(logicalThickness.Right, logicalThickness.Bottom));
            return new Thickness(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }

        #region WPF 4.6.1+ recommend
        public static double DipToPixels(double dip, Visual visual)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            return dip * dpi.DpiScaleX;
        }

        public static Point DipToPixels(Point dipPoint, Visual visual)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            return new Point(dipPoint.X * dpi.DpiScaleX, dipPoint.Y * dpi.DpiScaleY);
        }

        public static Size DipToPixels(Size dipSize, Visual visual)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            return new Size(dipSize.Width * dpi.DpiScaleX, dipSize.Height * dpi.DpiScaleY);
        }

        public static Rect DipToPixels(Rect dipRect, Visual visual)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            return new Rect(
                dipRect.X * dpi.DpiScaleX,
                dipRect.Y * dpi.DpiScaleY,
                dipRect.Width * dpi.DpiScaleX,
                dipRect.Height * dpi.DpiScaleY);
        }

        public static double PixelsToDip(double pixels, Visual visual)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            return pixels / dpi.DpiScaleX;
        }

        public static Point PixelsToDip(Point pixelsPoint, Visual visual)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            return new Point(pixelsPoint.X / dpi.DpiScaleX, pixelsPoint.Y / dpi.DpiScaleY);
        }

        public static Size PixelsToDip(Size pixelsSize, Visual visual)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            return new Size(pixelsSize.Width / dpi.DpiScaleX, pixelsSize.Height / dpi.DpiScaleY);
        }

        public static Rect PixelsToDip(Rect pixelsRect, Visual visual)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            return new Rect(
                pixelsRect.X / dpi.DpiScaleX,
                pixelsRect.Y / dpi.DpiScaleY,
                pixelsRect.Width / dpi.DpiScaleX,
                pixelsRect.Height / dpi.DpiScaleY);
        }
        #endregion


    }
}
