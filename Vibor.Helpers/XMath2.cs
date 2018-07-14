// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XMath2
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;

namespace Vibor.Helpers
{
  public class XMath2
  {
    public static double GetPercentage(int a, int b)
    {
      return Math.Abs((double) (a - b) / (double) b) * 100.0;
    }

    public static bool FindingClosestToZero(Dictionary<short, short> averageValues, out int bestKey)
    {
      short num1 = 0;
      short num2 = 0;
      bool flag = false;
      bestKey = 0;
      foreach (KeyValuePair<short, short> averageValue in averageValues)
      {
        if (!flag)
        {
          flag = true;
          num1 = averageValue.Value;
          num2 = averageValue.Key;
        }
        else if (XMath2.FindClosestToZero((int) num2, (int) num1, (int) averageValue.Key, (int) averageValue.Value, out bestKey))
          return true;
      }
      return false;
    }

    public static bool FindClosestToZero(int prevKey, int preValue, int newKey, int newValue, out int bestKey)
    {
      if (Math.Sign(preValue) != Math.Sign(newValue))
      {
        bestKey = Math.Abs(preValue) >= Math.Abs(newValue) ? newKey : prevKey;
        return true;
      }
      bestKey = 0;
      return false;
    }

    public static void SinFit(IList<short> phases, List<short> pixelValues, out float pixelValueAmplitude, out float pixelValueZero, out float phaseCount)
    {
      float num1 = float.MinValue;
      float num2 = float.MaxValue;
      float num3 = 0.0f;
      int num4 = 0;
      int num5 = 0;
      for (int index = 0; index < pixelValues.Count; ++index)
      {
        if ((double) pixelValues[index] > (double) num1)
        {
          num1 = (float) pixelValues[index];
          num5 = index;
        }
        if ((double) pixelValues[index] < (double) num2)
        {
          num2 = (float) pixelValues[index];
          num4 = index;
        }
        num3 += (float) pixelValues[index];
      }
      int num6 = 1;
      if (num4 > num5)
      {
        int num7 = num4;
        num4 = num5;
        num5 = num7;
        num6 = -1;
      }
      pixelValueAmplitude = (float) (((double) num1 - (double) num2) / 2.0);
      pixelValueZero = (float) (((double) num1 + (double) num2) / 2.0);
      float num8 = 0.0f;
      float num9 = 0.0f;
      float num10 = 0.0f;
      float num11 = 0.0f;
      for (int index = num4; index <= num5; ++index)
      {
        short phase = phases[index];
        short pixelValue = pixelValues[index];
        num8 += (float) phase;
        num11 += (float) ((int) phase * (int) phase);
        float num7 = ((float) pixelValue - pixelValueZero) / pixelValueAmplitude;
        if ((double) num7 < -1.0)
          num7 = -1f;
        if ((double) num7 > 1.0)
          num7 = 1f;
        float num12 = (float) Math.Asin((double) num7);
        num10 += num12;
        num9 += (float) phase * num12;
      }
      float num13 = (float) ((double) (Math.Abs(num5 - num4) + 1) * (double) num11 - (double) num8 * (double) num8);
      float num14 = (float) ((double) num11 * (double) num10 - (double) num8 * (double) num9) / num13;
      phaseCount = (float) (-((double) num14 - Math.PI / 2.0) * 16.0 / Math.PI) * (float) num6;
      float num15 = (float) ((double) (Math.Abs(num5 - num4) + 1) * (double) num9 - (double) num8 * (double) num10) / num13;
    }

    public static double PointToLineDist(int x0, int y0, int x1, int y1, int px, int py)
    {
      return (double) Math.Abs((x1 - x0) * (y1 - py) - (x1 - px) * (y1 - y0)) / Math.Sqrt((double) ((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)));
    }

    public static bool PointToLine(int x0, int y0, int x1, int y1, int px, int py, double tol)
    {
      return XMath2.PointToLineDist(x0, y0, x1, y1, px, py) < tol;
    }

    public static double PointToPointDist(int x0, int y0, int px, int py)
    {
      return Math.Sqrt((double) ((px - x0) * (px - x0) + (py - y0) * (py - y0)));
    }

    public static bool PointToPoint(int x0, int y0, int px, int py, double tol)
    {
      return XMath2.PointToPointDist(x0, y0, px, py) < tol;
    }

    public static ushort CalculateChecksum(IList<ushort> words)
    {
      ushort num = 0;
      foreach (ushort word in (IEnumerable<ushort>) words)
        num += word;
      return num;
    }

    public static ushort CalculateChecksum(IList<byte> bytes)
    {
      return XMath2.CalculateChecksum((IList<ushort>) BitByteConverter.BytesToWords(bytes));
    }

    public static ushort CalculateCrc(IList<byte> bytes)
    {
      return new Crc16().Calculate(bytes);
    }

    public static ushort CalculateCrcFp41(IList<byte> bytes)
    {
      return new Crc16().CalculateFp41(bytes);
    }
  }
}
