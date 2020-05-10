using System;
using Microsoft.Extensions.Logging;

namespace ProjectK.Utils
{
    public class LoggingEventArgs : EventArgs
    {
        public string State { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }
    }
}