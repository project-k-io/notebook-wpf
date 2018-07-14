// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XConverter
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

namespace Vibor.Helpers
{
  public class XConverter
  {
    public static int ConvertToInt(string s)
    {
      int result;
      int.TryParse(s, out result);
      return result;
    }

    public static double ConvertToDouble(string s)
    {
      double result;
      double.TryParse(s, out result);
      return result;
    }
  }
}
