using System;
using Microsoft.Extensions.Logging;

namespace ProjectK.Logging
{
    public class OutputLogger : ILogger
    {
        private readonly Action<LogLevel, EventId, string> _logEvent;

        public OutputLogger(Action<LogLevel, EventId, string> logEvent)
        {
            _logEvent = logEvent;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logEvent?.Invoke(logLevel, eventId, formatter(state, exception));
        }
    }
}