using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ProjectK.Logging
{
    public class OutputLoggerProvider : ILoggerProvider
    {
        private readonly Action<LogLevel, EventId, string> _logEvent;
        private readonly ConcurrentDictionary<string, OutputLogger> _loggers = new();

        public OutputLoggerProvider(Action<LogLevel, EventId, string> logEvent)
        {
            _logEvent = logEvent;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new OutputLogger(_logEvent));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}