using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProjectK.Utils
{
    public class LogManager
    {
        public static ServiceProvider Provider { get; set; }

        public static ILogger GetLogger<T>()
        {
            return Provider?.GetService<ILogger<T>>();
        }
    }
}