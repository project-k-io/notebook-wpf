namespace Vibor.Helpers
{
    public class XMath
    {
        public static double Percentage(double oldValue, double newValue)
        {
            return oldValue != 0.0 ? (newValue - oldValue) / oldValue * 100.0 : 0.0;
        }
    }
}