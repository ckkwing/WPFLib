using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theme
{
    static class Utility
    {
        internal static string PathEllipsis(string path, int maxLength)
        {
            if (string.IsNullOrEmpty(path) || maxLength >= path.Length)
            {
                return path;
            }

            char[] seprators = new char[] { '\\', '/' };
            int lastPos = path.LastIndexOfAny(seprators);
            if (lastPos < 0 || path.Length - lastPos >= maxLength)
            {
                return path;
            }

            string ellipsispath = "...";
            int ellipsisPos = path.LastIndexOfAny(seprators, lastPos - path.Length + maxLength);
            if (ellipsisPos > 0)
            {
                ellipsispath = path.Substring(0, ellipsisPos + 1);
                ellipsispath += "...";
            }
            ellipsispath += path.Substring(lastPos, path.Length - lastPos);

            return ellipsispath;
        }
    }
}
