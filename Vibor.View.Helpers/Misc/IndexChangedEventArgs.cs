using System;

namespace Vibor.View.Helpers.Misc
{
    public class IndexChangedEventArgs : EventArgs
    {
        public int Index { get; set; } = -1;
    }
}