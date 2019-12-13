using Microsoft.Extensions.Logging;

namespace Vibor.Helpers
{
    public class LogManager
    {
        private static ILogger Logger { get; set; }

        public static ILogger GetLogger()
        {
            return Logger;
        }

        public static ILogger GetLogger(string name)
        {
            return Logger;
        }
    }
}