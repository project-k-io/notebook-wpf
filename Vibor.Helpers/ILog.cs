// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.ILog
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;

namespace Vibor.Helpers
{
  public interface ILog
  {
    void Info(string s);

    void Info(Exception ex);

    void InfoFormat(string format, params object[] values);

    void Warn(string s);

    void Warn(Exception ex);

    void WarnFormat(string format, params object[] values);

    void Error(string s);

    void Error(Exception ex);

    void ErrorFormat(string format, params object[] values);

    void Debug(string state, string s);

    void Debug(string s);

    void Debug(Exception ex);

    void DebugFormat(string format, params object[] values);

    event EventHandler<LoggingEventArgs> LoggingEvent;
  }
}
