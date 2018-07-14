
using System.Diagnostics;

namespace Vibor.Helpers
{
    public class OutputTraceListener : TraceListener
    {
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
            object data)
        {
        }

        public override void Write(string message)
        {
            Debug.Write(message);
        }

        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}