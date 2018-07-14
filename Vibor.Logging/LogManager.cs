namespace Vibor.Logging
{
    public class LogManager
    {
        private static readonly Logger Logger = new Logger();

        public static ILog GetLogger()
        {
            return Logger;
        }

        public static ILog GetLogger(string name)
        {
            return Logger;
        }
    }
}