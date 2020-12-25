﻿using Microsoft.Extensions.Logging;
using System;

namespace ProjectK.Logging
{
    public class LoggingEventArgs : EventArgs
    {
        public string State { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }
        public EventId EventId { get; set; }
    }
}