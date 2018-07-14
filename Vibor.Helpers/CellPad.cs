// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.CellPad
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

namespace Vibor.Helpers
{
  public class CellPad
  {
    public static string FormatV = "{0,5:C}, {1,8:C}, {2,5}";
    public static string FormatH = "{0}-A, {0}-T, {0}-C";

    public CellPad()
    {
      this.Reset();
    }

    public double Average
    {
      get
      {
        return this.Count > 0 ? this.Total / (double) this.Count : 0.0;
      }
    }

    public int Count { get; private set; }

    public double Total { get; private set; }

    public void Reset()
    {
      this.Total = 0.0;
      this.Count = 0;
    }

    public void Add(double a)
    {
      this.Total += a;
      ++this.Count;
    }

    public void Add(CellPad a)
    {
      this.Total += a.Total;
      this.Count += a.Count;
    }

    public void Add(double total, int count)
    {
      this.Total += total;
      this.Count += count;
    }

    public void Set(CellPad a)
    {
      this.Total = a.Total;
      this.Count = a.Count;
    }

    public void Subtract(double a)
    {
      if (this.Count <= 0)
        return;
      this.Total -= a;
      --this.Count;
    }

    public static string GetHeader(string s)
    {
      return string.Format(CellPad.FormatH, (object) s);
    }

    public override string ToString()
    {
      return string.Format(CellPad.FormatV, (object) this.Average, (object) this.Total, (object) this.Count);
    }
  }
}
