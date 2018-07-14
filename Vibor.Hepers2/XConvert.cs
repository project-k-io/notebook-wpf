using System;
using System.Globalization;

namespace Vibor.Helpers
{
    class XConvert
    {
        public const string DateFormat2 = "yyyy-MM-dd";
        public const string DateFormat3 = "dd-MM-yyyy";
        public const int YearMutiplier = 10000;
        public const int MonthMutiplier = 100;
        public const int HourMutiplier = 100;

        static XConvert()
        {
            ExcelCsvCurrencyFormat.CurrencyGroupSeparator = "";
        }

        public static NumberFormatInfo ExcelCsvCurrencyFormat { get; } =
            (NumberFormatInfo) CultureInfo.CurrentCulture.NumberFormat.Clone();

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
            return ConvertToIntDate(d.Year, d.Month, d.Day);
        }

        public static int ConvertToIntTime(DateTime d)
        {
            return ConvertToIntTime(d.Hour, d.Second);
        }

        public static int ConvertFromDateTimeStringToInt(string s)
        {
            var dateTime = DateTime.Parse(s.Trim());
            return ConvertToIntDate(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static void ExtractYearMonthDay(int date, ref int year, ref int mon, ref int day)
        {
            year = date / 10000;
            mon = (date - ConvertToIntDate(year, 0, 0)) / 100;
            day = date - ConvertToIntDate(year, mon, 0);
        }

        public static DateTime ConvertToDateTime(int date)
        {
            var year = 0;
            var mon = 0;
            var day = 0;
            ExtractYearMonthDay(date, ref year, ref mon, ref day);
            return new DateTime(year, mon, day);
        }

        public static string ConvertToString(int day)
        {
            if (day == 0)
                return string.Empty;
            return ConvertToString(ConvertToDateTime(day));
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
            return ToDouble(s, 0.0);
        }

        public static double ToDouble(string s, double o)
        {
            var str = s.Trim();
            if (string.IsNullOrEmpty(str))
                return o;
            if (str[0] == '$')
                str = str.Substring(1);
            return Convert.ToDouble(str);
        }

        public static string ToExcelCurrencyString(double a)
        {
            return a.ToString("C", ExcelCsvCurrencyFormat);
        }

        public static int ToInt32(string s)
        {
            return ToInt32(s.Trim(), 0);
        }

        public static int ToInt32(string s, int o)
        {
            var str = s.Trim();
            if (string.IsNullOrEmpty(str))
                return o;
            return Convert.ToInt32(str);
        }

        public static double ConvertToFromCurrencyToDouble(string s, double o)
        {
            var s1 = s.Trim();
            if (string.IsNullOrEmpty(s1))
                return o;
            return double.Parse(s1, NumberStyles.Currency);
        }
    }
}