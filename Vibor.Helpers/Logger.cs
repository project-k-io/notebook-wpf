// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.Logger
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;

namespace Vibor.Helpers
{
  public class Logger : ILog
  {
    public event EventHandler<LoggingEventArgs> LoggingEvent = null;

    public void Info(string s)
    {
      this.Print(Level.Info, s, "");
    }

    public void Info(Exception ex)
    {
      this.Info(ex.Message);
    }

    public void InfoFormat(string format, params object[] values)
    {
      this.Info(string.Format(format, values));
    }

    public void Warn(string s)
    {
      this.Print(Level.Warn, s, "");
    }

    public void Warn(Exception ex)
    {
      this.Warn(ex.Message);
    }

    public void WarnFormat(string format, params object[] values)
    {
      this.Warn(string.Format(format, values));
    }

    public void Error(string s)
    {
      this.Print(Level.Error, s, "");
    }

    public void Error(Exception ex)
    {
      this.Error(ex.Message);
    }

    public void ErrorFormat(string format, params object[] values)
    {
      this.Error(string.Format(format, values));
    }

    public void Debug(string state, string s)
    {
      this.Print(Level.Debug, s, state);
    }

    public void Debug(string s)
    {
      this.Print(Level.Debug, s, "");
    }

    public void Debug(Exception ex)
    {
      this.Debug(ex.Message);
    }

    public void DebugFormat(string format, params object[] values)
    {
      this.Debug(string.Format(format, values));
    }

    private void Print(Level level, string message, string state = "")
    {
      Console.WriteLine(message);
      if (this.LoggingEvent == null)
        return;
      this.LoggingEvent((object) this, new LoggingEventArgs()
      {
        Level = level,
        Message = message,
        State = state
      });
    }
  }
}
