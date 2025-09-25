using CommonUtility.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Theme.Helper
{
    /// <summary>
    /// A static class to check if two objects intersect or not.
    /// </summary>
    /// <remarks>
    /// This class is not fully tested. Use with caution.
    ///</remarks>
    [Warning("This class is not fully tested. Use with caution.")]
    public static class IntersectChecker
    {
        /// <summary>
        /// 使用Intersect方法判断是否有交集
        /// </summary>
        public static bool AreRectanglesDisjointUsingIntersect(Rect rect1, Rect rect2)
        {
            Rect intersection = Rect.Intersect(rect1, rect2);

            // 如果交集矩形是空的（Width或Height为0或负数），说明没有实际交集
            return intersection.IsEmpty || intersection.Width <= 0 || intersection.Height <= 0;
        }

        /// <summary>
        /// 判断两个矩形是否没有交集
        /// </summary>
        /// <param name="rect1">第一个矩形</param>
        /// <param name="rect2">第二个矩形</param>
        /// <returns>如果没有交集返回true，否则返回false</returns>
        public static bool AreRectanglesDisjoint(Rect rect1, Rect rect2)
        {
            return !rect1.IntersectsWith(rect2);
        }

        /// <summary>
        /// 判断多个矩形是否两两之间都没有交集
        /// </summary>
        public static bool AreAllRectanglesDisjoint(params Rect[] rectangles)
        {
            for (int i = 0; i < rectangles.Length; i++)
            {
                for (int j = i + 1; j < rectangles.Length; j++)
                {
                    if (rectangles[i].IntersectsWith(rectangles[j]))
                    {
                        return false; // 发现交集
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取两个矩形的交集区域（如果没有交集返回空矩形）
        /// </summary>
        public static Rect GetIntersectionArea(Rect rect1, Rect rect2)
        {
            Rect intersection = Rect.Intersect(rect1, rect2);

            if (intersection.IsEmpty || intersection.Width <= 0 || intersection.Height <= 0)
            {
                return Rect.Empty;
            }

            return intersection;
        }

        /// <summary>
        /// 检测两个矩形是否有交集，并且交集面积占第一个矩形面积的百分比达到指定值
        /// </summary>
        /// <param name="rectToCalculateIntersection">第一个矩形</param>
        /// <param name="rectToCompare">第二个矩形</param>
        /// <param name="percent">指定的百分比，默认值为50%</param>
        /// <returns>如果交集面积占第一个矩形面积的百分比达到指定值返回true，否则返回false</returns>
        public static bool HasIntersectionByPercent(Rect rectToCalculateIntersection, Rect rectToCompare, double percent = 0.5)
        {
            Rect intersection = Rect.Intersect(rectToCalculateIntersection, rectToCompare);

            if (intersection.IsEmpty)
                return false;

            double areaToCalculateIntersection = rectToCalculateIntersection.Width * rectToCalculateIntersection.Height;
            if (areaToCalculateIntersection == 0)
                return false;

            double areaIntersection = intersection.Width * intersection.Height;
            return (areaIntersection / areaToCalculateIntersection) >= percent;
        }

    }
}
