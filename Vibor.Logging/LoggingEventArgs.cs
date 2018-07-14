using System;

namespace Vibor.Logging
{
    public class LoggingEventArgs : EventArgs
    {
        public string State { get; set; }

        public string Message { get; set; }

        public Level Level { get; set; }
    }
}