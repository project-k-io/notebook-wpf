using System;
using Microsoft.Extensions.Logging;

namespace ProjectK.Logging;

public static class LoggerExtensions
{
    public static void LogError(this ILogger logger, Exception e)
    {
        logger.Log(LogLevel.Error, e.Message);
        var stacks = e.StackTrace.Split("\r\n");
        foreach (var stack in stacks) logger.Log(LogLevel.Error, stack);
    }
}