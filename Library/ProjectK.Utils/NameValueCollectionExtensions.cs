using System;
using System.Collections.Specialized;

namespace ProjectK.Utils
{
    public static class NameValueCollectionExtensions
    {
        public static T GetEnumValue<T>(this NameValueCollection collection, string key, T defaultValue) where T : struct
        {
            return Enum.TryParse(collection[key], out T value) ? value : defaultValue;
        }
        public static double GetDouble(this NameValueCollection collection,  string key, double defaultValue)
        {
            return double.TryParse(collection[key], out var value) ? value : defaultValue;
        }

        public static int GetInt(this NameValueCollection collection, string key, int defaultValue)
        {
            return int.TryParse(collection[key], out var value) ? value : defaultValue;
        }

        public static Guid GetGuid(this NameValueCollection collection, string key, Guid defaultValue)
        {
            return Guid.TryParse(collection[key], out var value) ? value : defaultValue;
        }

        public static string GetString(this NameValueCollection collection, string key, string defaultValue)
        {
            return collection[key] ?? defaultValue;
        }
    }
}
