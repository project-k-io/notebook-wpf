using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Vibor.Helpers
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