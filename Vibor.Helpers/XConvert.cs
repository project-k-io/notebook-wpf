// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XConvert
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Globalization;

namespace Vibor.Helpers
{
  public class XConvert
  {
    private static readonly NumberFormatInfo excelCsvCurrencyFormat = (NumberFormatInfo) CultureInfo.CurrentCulture.NumberFormat.Clone();
    public const string DateFormat2 = "yyyy-MM-dd";
    public const string DateFormat3 = "dd-MM-yyyy";
    public const int YearMutiplier = 10000;
    public const int MonthMutiplier = 100;
    public const int HourMutiplier = 100;

    static XConvert()
    {
      XConvert.excelCsvCurrencyFormat.CurrencyGroupSeparator = "";
    }

    public static NumberFormatInfo ExcelCsvCurrencyFormat
    {
      get
      {
        return XConvert.excelCsvCurrencyFormat;
      }
    }

    public static int ConvertToIntDate(int year, int mon, int day)
    {
      return year * 10000 + mon * 100 + day;
    }

    public static int ConvertToIntTime(int hour, int sec)
    {
      return hour * 100 + sec;
    }

    public static int ConvertToIntDate(DateTime d)
    {
      return XConvert.ConvertToIntDate(d.Year, d.Month, d.Day);
    }

    public static int ConvertToIntTime(DateTime d)
    {
      return XConvert.ConvertToIntTime(d.Hour, d.Second);
    }

    public static int ConvertFromDateTimeStringToInt(string s)
    {
      DateTime dateTime = DateTime.Parse(s.Trim());
      return XConvert.ConvertToIntDate(dateTime.Year, dateTime.Month, dateTime.Day);
    }

    public static void ExtractYearMonthDay(int date, ref int year, ref int mon, ref int day)
    {
      year = date / 10000;
      mon = (date - XConvert.ConvertToIntDate(year, 0, 0)) / 100;
      day = date - XConvert.ConvertToIntDate(year, mon, 0);
    }

    public static DateTime ConvertToDateTime(int date)
    {
      int year = 0;
      int mon = 0;
      int day = 0;
      XConvert.ExtractYearMonthDay(date, ref year, ref mon, ref day);
      return new DateTime(year, mon, day);
    }

    public static string ConvertToString(int day)
    {
      if (day == 0)
        return string.Empty;
      return XConvert.ConvertToString(XConvert.ConvertToDateTime(day));
    }

    public static string ConvertToString(DateTime date)
    {
      return date.ToString("yyyy-MM-dd");
    }

    public static string ToString(DateTime dt)
    {
      return dt.ToString("yyyy-MM-dd");
    }

    public static string ToString3(DateTime dt)
    {
      return dt.ToString("dd-MM-yyyy");
    }

    public static double ToDouble(string s)
    {
      return XConvert.ToDouble(s, 0.0);
    }

    public static double ToDouble(string s, double o)
    {
      string str = s.Trim();
      if (string.IsNullOrEmpty(str))
        return o;
      if (str[0] == '$')
        str = str.Substring(1);
      return Convert.ToDouble(str);
    }

    public static string ToExcelCurrencyString(double a)
    {
      return a.ToString("C", (IFormatProvider) XConvert.ExcelCsvCurrencyFormat);
    }

    public static int ToInt32(string s)
    {
      return XConvert.ToInt32(s.Trim(), 0);
    }

    public static int ToInt32(string s, int o)
    {
      string str = s.Trim();
      if (string.IsNullOrEmpty(str))
        return o;
      return Convert.ToInt32(str);
    }

    public static double ConvertToFromCurrencyToDouble(string s, double o)
    {
      string s1 = s.Trim();
      if (string.IsNullOrEmpty(s1))
        return o;
      return double.Parse(s1, NumberStyles.Currency);
    }
  }
}
