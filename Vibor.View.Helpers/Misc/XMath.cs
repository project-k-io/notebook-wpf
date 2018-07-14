using System;
using System.Windows;
using System.Windows.Media;

namespace Vibor.View.Helpers.Misc
{
    public class XMath
    {
        public static int FindClosestPointIndex(PointCollection points, Point p)
        {
            var num1 = double.MaxValue;
            var num2 = -1;
            for (var index = 0; index < points.Count; ++index)
            {
                var num3 = Math.Abs(points[index].X - p.X);
                if (num3 < num1)
                {
                    num1 = num3;
                    num2 = index;
                }
            }

            return num2;
        }
    }
}