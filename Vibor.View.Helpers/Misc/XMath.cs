// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Misc.XMath
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.Windows;
using System.Windows.Media;

namespace Vibor.View.Helpers.Misc
{
  public class XMath
  {
    public static int FindClosestPointIndex(PointCollection points, Point p)
    {
      double num1 = double.MaxValue;
      int num2 = -1;
      for (int index = 0; index < points.Count; ++index)
      {
        double num3 = Math.Abs(points[index].X - p.X);
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
