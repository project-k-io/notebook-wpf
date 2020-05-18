using System.Configuration;

namespace ProjectK.Notebook.Extensions
{
    public static class KeyValueConfigurationCollectionExtensions
    {
        public static void SetValue(this KeyValueConfigurationCollection settings, string key, string value)
        {
            if (settings[key] == null)
                settings.Add(key, value);
            else
                settings[key].Value = value;
        }
    }
}
