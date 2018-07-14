// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.LogManager
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

namespace Vibor.Logging
{
    public class LogManager
    {
        private static readonly Logger logger = new Logger();

        public static ILog GetLogger()
        {
            return logger;
        }

        public static ILog GetLogger(string name)
        {
            return logger;
        }
    }
}