// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Misc.Helper
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.Windows.Threading;

namespace Vibor.View.Helpers.Misc
{
    public class Helper
    {
        public static void SafePause(DispatcherObject o)
        {
            SafePause(o, 1.0, false);
        }

        public static void SafePause(DispatcherObject o, double PauseSeconds)
        {
            SafePause(o, PauseSeconds, false);
        }

        public static void SafePause(DispatcherObject o, double PauseSeconds, bool myCancel)
        {
            DateTime dateTime = DateTime.Now.AddSeconds(PauseSeconds);
            do
            {
                DoEvents(o);
            } while (dateTime > DateTime.Now && !myCancel);
        }

        public static void DoEvents(DispatcherObject o)
        {
            o.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() => { }));
        }
    }
}