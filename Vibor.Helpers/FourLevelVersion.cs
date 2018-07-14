using System;
using Vibor.Logging;

namespace Vibor.Helpers
{
    public class FourLevelVersion
    {
        private readonly ILog _logger = LogManager.GetLogger(nameof(FourLevelVersion));

        public bool IgnoreProtocolVersion { get; set; }

        public short Major { get; set; }

        public short Minor { get; set; }

        public short Protocol { get; set; }

        public short Build { get; set; }

        public bool IsValid => Major != 0;

        public static bool operator <(FourLevelVersion a, FourLevelVersion b)
        {
            if (a.Major != b.Major)
                return a.Major < b.Major;
            if (a.Minor != b.Minor)
                return a.Minor < b.Minor;
            if (a.IgnoreProtocolVersion)
                return false;
            return a.Protocol < b.Protocol;
        }

        public static bool operator >(FourLevelVersion a, FourLevelVersion b)
        {
            return b < a;
        }

        public static bool operator ==(FourLevelVersion a, FourLevelVersion b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null)
                return false;
            return a.Major == b.Major && a.Minor == b.Minor && (a.IgnoreProtocolVersion || a.Protocol == b.Protocol);
        }

        public static bool operator !=(FourLevelVersion a, FourLevelVersion b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == obj as FourLevelVersion;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var str = IgnoreProtocolVersion
                ? string.Format("{0}.{1}", Major, Minor)
                : string.Format("{0}.{1}.{2}", Major, Minor, Protocol);
            return Build == (short) 0 || IgnoreProtocolVersion ? str : str + "." + Build;
        }

        public void Parse(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                var strArray = text.Split('.');
                if (!XList.IsValidIndex(strArray, 0))
                    return;
                short result1 = 0;
                if (!short.TryParse(strArray[0], out result1))
                    return;
                Major = result1;
                if (!XList.IsValidIndex(strArray, 1))
                    return;
                short result2 = 0;
                if (!short.TryParse(strArray[1], out result2))
                    return;
                Minor = result2;
                if (IgnoreProtocolVersion || !XList.IsValidIndex(strArray, 2))
                    return;
                short result3 = 0;
                if (!short.TryParse(strArray[2], out result3))
                    return;
                Protocol = result3;
                if (strArray.Length > 3 && short.TryParse(strArray[3], out result3))
                    Build = result3;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}