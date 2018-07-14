// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.LoggingEventArgs
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;

namespace Vibor.Logging
{
    public class LoggingEventArgs : EventArgs
    {
        public string State { get; set; }

        public string Message { get; set; }

        public Level Level { get; set; }
    }
}