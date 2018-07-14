// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XMath
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

namespace Vibor.Helpers
{
    public class XMath
    {
        public static double Percentage(double oldValue, double newValue)
        {
            return oldValue != 0.0 ? (newValue - oldValue) / oldValue * 100.0 : 0.0;
        }
    }
}