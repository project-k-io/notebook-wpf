using Microsoft.Extensions.Logging;
using System;

namespace Vibor.Logging
{
    public class LoggingEventArgs : EventArgs
    {
        public string State { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }
    }
}