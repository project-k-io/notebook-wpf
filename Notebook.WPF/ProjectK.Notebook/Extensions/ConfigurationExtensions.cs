using System;
using System.Collections.Specialized;
using System.Configuration;

namespace ProjectK.Notebook.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetEnumValue<T>(this NameValueCollection collection, string key, T defaultValue) where T : struct => Enum.TryParse(collection[key], out T value) ? value : defaultValue;
        public static double GetDouble(this NameValueCollection collection, string key, double defaultValue) => double.TryParse(collection[key], out var value) ? value : defaultValue;
        public static int GetInt(this NameValueCollection collection, string key, int defaultValue) => int.TryParse(collection[key], out var value) ? value : defaultValue;
        public static Guid GetGuid(this NameValueCollection collection, string key, Guid defaultValue) => Guid.TryParse(collection[key], out var value) ? value : defaultValue;
        public static string GetString(this NameValueCollection collection, string key, string defaultValue) => collection[key] ?? defaultValue;
        public static bool GetBool(this NameValueCollection collection, string key, bool defaultValue) => bool.TryParse(collection[key], out var value) ? value : defaultValue;

        public static void SetValue(this KeyValueConfigurationCollection settings, string key, string value)
        {
            if (settings[key] == null)
                settings.Add(key, value);
            else
                settings[key].Value = value;
        }
    }
    public static class ConfigurationExtensions2
    {
        public static T GetEnumValue<T>(this string key, T defaultValue) where T : struct => Enum.TryParse(key, out T value) ? value : defaultValue;
        public static double GetDouble(this string key, double defaultValue) => double.TryParse(key, out var value) ? value : defaultValue;
        public static int GetInt(this string key, int defaultValue) => int.TryParse(key, out var value) ? value : defaultValue;
        public static Guid GetGuid(this string key, Guid defaultValue) => Guid.TryParse(key, out var value) ? value : defaultValue;
        public static string GetString(this string key, string defaultValue) => key ?? defaultValue;
        public static bool GetBool(this string key, bool defaultValue) => bool.TryParse(key, out var value) ? value : defaultValue;

        public static void SetValue(this string key, string value)
        {
            // if (key == null)
                // settings.Add(key, value);
            //else
                // settings[key].Value = value;
        }
    }
}
