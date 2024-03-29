﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Theme.Native;

namespace Theme.Helper
{
    public class WindowParameters
    {
        private static Thickness? _paddedBorderThickness = null;
        /// <summary>
        /// returns the border thickness padding around captioned windows,in pixels. Windows XP/2000:  This value is not supported.
        /// </summary>
        public static Thickness PaddedBorderThickness
        {
            [SecurityCritical]
            get
            {
                if (_paddedBorderThickness == null)
                {
                    var paddedBorder = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXPADDEDBORDER);
                    //var dpi = GetDpi();
                    Size frameSize = new Size(paddedBorder, paddedBorder);
                    Size frameSizeInDips = DpiHelper.DeviceSizeToLogical(frameSize);

                    //frameSizeInDips = new Size(4,4); the correct value are 4,4

                    _paddedBorderThickness = new Thickness(frameSizeInDips.Width, frameSizeInDips.Height, frameSizeInDips.Width, frameSizeInDips.Height);
                }

                return _paddedBorderThickness.Value;
            }
        }


    }
}
