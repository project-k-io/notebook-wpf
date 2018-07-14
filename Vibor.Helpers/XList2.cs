﻿// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XList2
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vibor.Helpers
{
  public class XList2
  {
    public static bool IsValidIndex2(IList a, int ii)
    {
      if (a == null)
        return false;
      return ii >= 0 && ii < a.Count;
    }

    public static bool IsNullOrEmpty2(IList a)
    {
      return !XList2.IsValidIndex2(a, 0);
    }

    public static bool IsValidIndex<T>(ICollection<T> a, int ii)
    {
      if (a == null)
        return false;
      return ii >= 0 && ii < a.Count;
    }

    public static bool IsNullOrEmpty<T>(ICollection<T> a)
    {
      return !XList2.IsValidIndex<T>(a, 0);
    }

    public static bool Equal(IList<short> a, IList<short> b)
    {
      return XList2.Equal<short>(a, b, (Func<short, short, bool>) ((a1, b1) => (int) a1 == (int) b1));
    }

    public static bool Equal(IList<byte> a, IList<byte> b)
    {
      return XList2.Equal<byte>(a, b, (Func<byte, byte, bool>) ((a1, b1) => (int) a1 == (int) b1));
    }

    public static bool Equal<T>(IList<T> a, IList<T> b, Func<T, T, bool> equal)
    {
      if (a == null || b == null || a.Count != b.Count)
        return false;
      for (int index = 0; index < a.Count; ++index)
      {
        if (!equal(a[index], b[index]))
          return false;
      }
      return true;
    }

    public static void AddSeparatedText<T>(StringBuilder sb, List<T> list, Func<T, string> format, string separator)
    {
      foreach (T obj in list)
      {
        string format1 = format(obj);
        sb.AppendFormat(format1,  obj);
        sb.AppendFormat(separator);
      }
    }

    public static void AddCommaSeparatedText(StringBuilder sb, List<string> list, string separator = ",")
    {
      XList2.AddSeparatedText<string>(sb, list, (Func<string, string>) (s => s), separator);
    }

    public static string GetCommaSepartedString(List<string> r)
    {
      StringBuilder sb = new StringBuilder();
      XList2.AddCommaSeparatedText(sb, r, ",");
      return sb.ToString();
    }

    public static TKey FindBestKey<TKey, TVal>(Dictionary<TKey, TVal> aveageValues, TVal startValue, Func<TVal, TVal, bool> comp)
    {
      TVal val = startValue;
      TKey key = default (TKey);
      foreach (KeyValuePair<TKey, TVal> aveageValue in aveageValues)
      {
        if (comp(aveageValue.Value, val))
        {
          val = aveageValue.Value;
          key = aveageValue.Key;
        }
      }
      return key;
    }

    public static TKey FindMaximKey<TKey>(Dictionary<TKey, short> aveageValues)
    {
      return XList2.FindBestKey<TKey, short>(aveageValues, short.MinValue, (Func<short, short, bool>) ((a, b) => (int) a >= (int) b));
    }

    public static TKey FindMinumKey<TKey>(Dictionary<TKey, short> aveageValues)
    {
      return XList2.FindBestKey<TKey, short>(aveageValues, short.MaxValue, (Func<short, short, bool>) ((a, b) => (int) a <= (int) b));
    }

    public static bool FindInRangeKey<TKey, TVal>(Dictionary<TKey, TVal> aveageValues, TVal minValue, TVal maxValue, Func<TVal, TVal, bool> less, ref TKey key)
    {
      key = default (TKey);
      foreach (KeyValuePair<TKey, TVal> aveageValue in aveageValues)
      {
        TVal val = aveageValue.Value;
        if (less(minValue, val) && less(val, maxValue))
        {
          key = aveageValue.Key;
          return true;
        }
      }
      return false;
    }

    public static bool FindInRangeKey<TKey>(Dictionary<TKey, short> aveageValues, short minValue, short maxValue, ref TKey key)
    {
      return XList2.FindInRangeKey<TKey, short>(aveageValues, minValue, maxValue, (Func<short, short, bool>) ((a, b) => (int) a <= (int) b), ref key);
    }

    public static List<T> PopulateArray<T>(string line, Func<string, T> convert, char separator = ',')
    {
      char[] chArray = new char[1]{ separator };
      line = line.Trim();
      if (string.IsNullOrEmpty(line))
        return (List<T>) null;
      if (line[line.Length - 1] == ',')
        line = line.Remove(line.Length - 1);
      return ((IEnumerable<string>) line.Split(chArray)).Select<string, T>(convert).ToList<T>();
    }

    public static List<double> PopulateArray(string line, char separator = ',')
    {
      return XList2.PopulateArray<double>(line, new Func<string, double>(XConverter.ConvertToDouble), separator);
    }

    public static byte[] AllocateBytes(int size, byte zero)
    {
      byte[] numArray = new byte[size];
      for (int index = 0; index < size; ++index)
        numArray[index] = zero;
      return numArray;
    }

    public static byte[] StrToByteArray(string str)
    {
      return new UTF8Encoding().GetBytes(str);
    }

    public static string ByteArrayToStr(byte[] dBytes)
    {
      return new UTF8Encoding().GetString(dBytes);
    }

    public static string ByteArrayToHexStr(byte[] bytes)
    {
      return BitConverter.ToString(bytes).Replace("-", "");
    }
  }
}
