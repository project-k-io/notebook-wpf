// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.OutputTraceListener
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System.Diagnostics;

namespace Vibor.Helpers
{
  public class OutputTraceListener : TraceListener
  {
    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
    {
    }

    public override void Write(string message)
    {
      Debug.Write(message);
    }

    public override void WriteLine(string message)
    {
      Debug.WriteLine(message);
    }
  }
}
