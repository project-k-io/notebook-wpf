using System.IO;
using System.Reflection;

namespace Vibor.Helpers
{
    public class XApp
    {
        public static string AppName
        {
            get
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    return nameof(AppName);
                return Path.GetFileName(entryAssembly.GetName().CodeBase);
            }
        }
    }
}