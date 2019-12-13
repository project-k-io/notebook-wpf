namespace Vibor.Helpers
{
    internal class TwoLevelVersion : FourLevelVersion
    {
        public TwoLevelVersion()
        {
            IgnoreProtocolVersion = true;
        }
    }
}