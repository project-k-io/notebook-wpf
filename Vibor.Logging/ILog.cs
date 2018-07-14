using System;

namespace Vibor.Logging
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