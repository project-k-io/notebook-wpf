using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ProjectK.Logging
{
    public class LogManager
    {
        private static IServiceProvider _provider;

        public LogManager(IServiceProvider provider)
        {
            _provider = provider;
        }

        public static ILogger GetLogger<T>()
        {
            return _provider?.GetService<ILogger<T>>();
        }

        public static ILogger GetLogger()
        {
            return _provider?.GetService<ILogger<LogManager>>();
        }
    }
}