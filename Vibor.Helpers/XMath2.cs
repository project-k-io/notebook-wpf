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
            return Math.Abs((a - b) / (double) b) * 100.0;
        }

        public static bool FindingClosestToZero(Dictionary<short, short> averageValues, out int bestKey)
        {
            short num1 = 0;
            short num2 = 0;
            var flag = false;
            bestKey = 0;
            foreach (var averageValue in averageValues)
                if (!flag)
                {
                    flag = true;
                    num1 = averageValue.Value;
                    num2 = averageValue.Key;
                }
                else if (FindClosestToZero(num2, num1, averageValue.Key, averageValue.Value, out bestKey))
                {
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

        public static void SinFit(IList<short> phases, List<short> pixelValues, out float pixelValueAmplitude,
            out float pixelValueZero, out float phaseCount)
        {
            var num1 = float.MinValue;
            var num2 = float.MaxValue;
            var num3 = 0.0f;
            var num4 = 0;
            var num5 = 0;
            for (var index = 0; index < pixelValues.Count; ++index)
            {
                if (pixelValues[index] > (double) num1)
                {
                    num1 = pixelValues[index];
                    num5 = index;
                }

                if (pixelValues[index] < (double) num2)
                {
                    num2 = pixelValues[index];
                    num4 = index;
                }

                num3 += pixelValues[index];
            }

            var num6 = 1;
            if (num4 > num5)
            {
                var num7 = num4;
                num4 = num5;
                num5 = num7;
                num6 = -1;
            }

            pixelValueAmplitude = (float) ((num1 - (double) num2) / 2.0);
            pixelValueZero = (float) ((num1 + (double) num2) / 2.0);
            var num8 = 0.0f;
            var num9 = 0.0f;
            var num10 = 0.0f;
            var num11 = 0.0f;
            for (var index = num4; index <= num5; ++index)
            {
                var phase = phases[index];
                var pixelValue = pixelValues[index];
                num8 += phase;
                num11 += phase * phase;
                var num7 = (pixelValue - pixelValueZero) / pixelValueAmplitude;
                if (num7 < -1.0)
                    num7 = -1f;
                if (num7 > 1.0)
                    num7 = 1f;
                var num12 = (float) Math.Asin(num7);
                num10 += num12;
                num9 += phase * num12;
            }

            var num13 = (float) ((Math.Abs(num5 - num4) + 1) * (double) num11 - num8 * (double) num8);
            var num14 = (float) (num11 * (double) num10 - num8 * (double) num9) / num13;
            phaseCount = (float) (-(num14 - Math.PI / 2.0) * 16.0 / Math.PI) * num6;
            var num15 = (float) ((Math.Abs(num5 - num4) + 1) * (double) num9 - num8 * (double) num10) / num13;
        }

        public static double PointToLineDist(int x0, int y0, int x1, int y1, int px, int py)
        {
            return Math.Abs((x1 - x0) * (y1 - py) - (x1 - px) * (y1 - y0)) /
                   Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
        }

        public static bool PointToLine(int x0, int y0, int x1, int y1, int px, int py, double tol)
        {
            return PointToLineDist(x0, y0, x1, y1, px, py) < tol;
        }

        public static double PointToPointDist(int x0, int y0, int px, int py)
        {
            return Math.Sqrt((px - x0) * (px - x0) + (py - y0) * (py - y0));
        }

        public static bool PointToPoint(int x0, int y0, int px, int py, double tol)
        {
            return PointToPointDist(x0, y0, px, py) < tol;
        }

        public static ushort CalculateChecksum(IList<ushort> words)
        {
            ushort num = 0;
            foreach (var word in words)
                num += word;
            return num;
        }

        public static ushort CalculateChecksum(IList<byte> bytes)
        {
            return CalculateChecksum(BitByteConverter.BytesToWords(bytes));
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