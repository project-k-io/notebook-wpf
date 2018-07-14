// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.Calendar
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;

namespace Vibor.Helpers
{
  public class Calendar
  {
    public static Calendar Instance = new Calendar();
    private readonly SortedList<int, SortedList<int, SortedList<int, string>>> _hoidayDays = new SortedList<int, SortedList<int, SortedList<int, string>>>();

    private Calendar()
    {
      this.PopulateCalendar();
    }

    public SortedList<int, SortedList<int, SortedList<int, string>>> HoidayDays
    {
      get
      {
        return this._hoidayDays;
      }
    }

    private void Add(string sdate, string name, string misc)
    {
      DateTime dateTime = DateTime.Parse(sdate);
      if (!this._hoidayDays.ContainsKey(dateTime.Year))
        this._hoidayDays.Add(dateTime.Year, new SortedList<int, SortedList<int, string>>());
      SortedList<int, SortedList<int, string>> hoidayDay = this.HoidayDays[dateTime.Year];
      if (!hoidayDay.ContainsKey(dateTime.Month))
        hoidayDay.Add(dateTime.Month, new SortedList<int, string>());
      SortedList<int, string> sortedList = hoidayDay[dateTime.Month];
      if (sortedList.ContainsKey(dateTime.Day))
        return;
      sortedList.Add(dateTime.Day, name);
    }

    private void PopulateCalendar()
    {
      this.Add("01/01/2010", "New Year's Day", "");
      this.Add("01/18/2010", "Martin Luther King Jr. Day", "");
      this.Add("02/15/2010", "President's Day", "");
      this.Add("04/02/2010", "Good Friday", "");
      this.Add("05/31/2010", "Memorial Day", "");
      this.Add("07/05/2010", "Independence Day", "");
      this.Add("09/06/2010", "Labor Day", " ");
      this.Add("11/25/2010", "Thanksgiving Day", "");
      this.Add("11/26/2010", "Day after Thanksgiving\t Early", "close 1:00 p.m.");
      this.Add("12/24/2010", "Christmas", "");
      this.Add("01/01/2009", "New Year's Day", "");
      this.Add("01/19/2009", "Martin Luther King Jr. Day", "");
      this.Add("02/16/2009", "President's Day", "");
      this.Add("04/10/2009", "Good Friday", "");
      this.Add("05/25/2009", "Memorial Day", "");
      this.Add("07/03/2009", "Independence Day", "");
      this.Add("09/07/2009", "Labor Day", "");
      this.Add("11/26/2009", "Thanksgiving Day", "");
      this.Add("11/27/2009", "Day after Thanksgiving Early", "close 1:00 p.m.");
      this.Add("12/25/2009", "Christmas", "");
      this.Add("01/01/2008", "New Year's Day", "");
      this.Add("01/21/2008", "Martin Luther King Jr. Day", "");
      this.Add("02/18/2008", "President's Day", "");
      this.Add("03/21/2008", "Good Friday", "");
      this.Add("05/26/2008", "Memorial Day", "");
      this.Add("07/04/2008", "Independence Day\t", "");
      this.Add("09/01/2008", "Labor Day", "");
      this.Add("11/27/2008", "Thanksgiving Day", "");
      this.Add("11/28/2008", "Day after Thanksgiving\t Early", "close 1:00 p.m.");
      this.Add("12/25/2008", "Christmas", "");
      this.Add("01/01/2007", "New Year's Day", "");
      this.Add("01/15/2007", "Martin Luther King Jr. Day", "");
      this.Add("02/19/2007", "President's Day", "");
      this.Add("04/06/2007", "Good Friday", "");
      this.Add("05/28/2007", "Memorial Day", "");
      this.Add("07/04/2007", "Independence Day", "");
      this.Add("09/03/2007", "Labor Day", "");
      this.Add("11/22/2007", "Thanksgiving Day", "");
      this.Add("11/23/2007", "Day after Thanksgiving Early", "close 1:00 p.m.");
      this.Add("12/25/2007", "Christmas", "");
      this.Add("01/02/2006", "New Year's Day", "");
      this.Add("01/16/2006", "Martin Luther King Jr. Day", "");
      this.Add("02/20/2006", "President's Day", "");
      this.Add("04/14/2006", "Good Friday", "");
      this.Add("05/29/2006", "Memorial Day", "");
      this.Add("07/04/2006", "Independence Day", "");
      this.Add("09/04/2006", "Labor Day", "");
      this.Add("11/23/2006", "Thanksgiving Day", "");
      this.Add("11/24/2006", "Day after Thanksgiving Early", "close 1:00 p.m.");
      this.Add("12/25/2006", "Christmas", "");
      this.Add("01/01/2005", "New Year's Day", "");
      this.Add("01/17/2005", "Martin Luther King Jr. Day", "");
      this.Add("02/21/2005", "President's Day", "");
      this.Add("03/25/2005", "Good Friday", "");
      this.Add("05/30/2005", "Memorial Day", "");
      this.Add("07/04/2005", "Independence Day", "");
      this.Add("09/05/2005", "Labor Day", "");
      this.Add("11/24/2005", "Thanksgiving Day", "");
      this.Add("11/25/2005", "Day after Thanksgiving Early", "close 1:00 p.m.");
      this.Add("12/26/2005", "Christmas", "");
    }

    public bool IsHolidayDay(DateTime date)
    {
      if (!this._hoidayDays.ContainsKey(date.Year))
        return false;
      SortedList<int, SortedList<int, string>> hoidayDay = this.HoidayDays[date.Year];
      return hoidayDay.ContainsKey(date.Month) && hoidayDay[date.Month].ContainsKey(date.Day);
    }
  }
}
