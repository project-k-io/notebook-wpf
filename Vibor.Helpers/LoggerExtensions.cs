using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Vibor.Helpers
{
    public static class LoggerExtensions
    {
        public static void LogError(this ILogger logger, Exception e)
        {
            logger.Log(LogLevel.Error, e.Message);
        }
    }
}
