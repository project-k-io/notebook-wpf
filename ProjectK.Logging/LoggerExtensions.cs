using System;
using Microsoft.Extensions.Logging;

namespace ProjectK.Logging
{
    public static class LoggerExtensions
    {
        public static void LogError(this ILogger logger, Exception e)
        {
            logger.Log(LogLevel.Error, e.Message);
        }
    }
}