namespace Vibor.Helpers
{
    public class CellPad
    {
        public static string FormatV = "{0,5:C}, {1,8:C}, {2,5}";
        public static string FormatH = "{0}-A, {0}-T, {0}-C";

        public CellPad()
        {
            Reset();
        }

        public double Average => Count > 0 ? Total / Count : 0.0;

        public int Count { get; private set; }

        public double Total { get; private set; }

        public void Reset()
        {
            Total = 0.0;
            Count = 0;
        }

        public void Add(double a)
        {
            Total += a;
            ++Count;
        }

        public void Add(CellPad a)
        {
            Total += a.Total;
            Count += a.Count;
        }

        public void Add(double total, int count)
        {
            Total += total;
            Count += count;
        }

        public void Set(CellPad a)
        {
            Total = a.Total;
            Count = a.Count;
        }

        public void Subtract(double a)
        {
            if (Count <= 0)
                return;
            Total -= a;
            --Count;
        }

        public static string GetHeader(string s)
        {
            return string.Format(FormatH, s);
        }

        public override string ToString()
        {
            return string.Format(FormatV, Average, Total, Count);
        }
    }
}