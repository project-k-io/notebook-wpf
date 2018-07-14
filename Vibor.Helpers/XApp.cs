// Decompiled with JetBrains decompiler
// Type: XApp
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

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