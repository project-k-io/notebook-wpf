namespace Vibor.Helpers
{
    internal class XConverter
    {
        public static int ConvertToInt(string s)
        {
            int result;
            int.TryParse(s, out result);
            return result;
        }

        public static double ConvertToDouble(string s)
        {
            double result;
            double.TryParse(s, out result);
            return result;
        }
    }
}