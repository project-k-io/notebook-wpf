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
            var dateTime = DateTime.Now.AddSeconds(PauseSeconds);
            do
            {
                DoEvents(o);
            } while (dateTime > DateTime.Now && !myCancel);
        }

        public static void DoEvents(DispatcherObject o)
        {
            o.Dispatcher.Invoke(DispatcherPriority.Background, (Action) (() => { }));
        }
    }
}