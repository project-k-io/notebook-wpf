using System;

namespace Vibor.Logging
{
    public class Logger : ILog
    {
        public event EventHandler<LoggingEventArgs> LoggingEvent;

        public void Info(string s)
        {
            Print(Level.Info, s);
        }

        public void Info(Exception ex)
        {
            Info(ex.Message);
        }

        public void InfoFormat(string format, params object[] values)
        {
            Info(string.Format(format, values));
        }

        public void Warn(string s)
        {
            Print(Level.Warn, s);
        }

        public void Warn(Exception ex)
        {
            Warn(ex.Message);
        }

        public void WarnFormat(string format, params object[] values)
        {
            Warn(string.Format(format, values));
        }

        public void Error(string s)
        {
            Print(Level.Error, s);
        }

        public void Error(Exception ex)
        {
            Error(ex.Message);
        }

        public void ErrorFormat(string format, params object[] values)
        {
            Error(string.Format(format, values));
        }

        public void Debug(string state, string s)
        {
            Print(Level.Debug, s, state);
        }

        public void Debug(string s)
        {
            Print(Level.Debug, s);
        }

        public void Debug(Exception ex)
        {
            Debug(ex.Message);
        }

        public void DebugFormat(string format, params object[] values)
        {
            Debug(string.Format(format, values));
        }

        private void Print(Level level, string message, string state = "")
        {
            Console.WriteLine(message);
            if (LoggingEvent == null)
                return;
            LoggingEvent(this, new LoggingEventArgs
            {
                Level = level,
                Message = message,
                State = state
            });
        }
    }
}