using System;
using NVC = System.Collections.Specialized.NameValueCollection;
namespace ProjectK.Utils.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static T GetEnumValue<T>(this NVC c, string key, T defaultValue) where T : struct => Enum.TryParse(c[key], out T value) ? value : defaultValue;
        public static double GetDouble(this NVC c,  string key, double defaultValue) => double.TryParse(c[key], out var value) ? value : defaultValue;
        public static int GetInt(this NVC c, string key, int defaultValue) => int.TryParse(c[key], out var value) ? value : defaultValue;
        public static Guid GetGuid(this NVC c, string key, Guid defaultValue) => Guid.TryParse(c[key], out var value) ? value : defaultValue;
        public static string GetString(this NVC c, string key, string defaultValue) => c[key] ?? defaultValue;
        public static bool GetBool(this NVC c, string key, bool defaultValue) => bool.TryParse(c[key], out var value) ? value : defaultValue;
    }
}
