using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectK.Logging;

namespace ProjectK.Utils
{
    public class XFile
    {
        private static readonly ILogger Logger = LogManager.GetLogger<XFile>();

        public static void SaveOldFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return;

                var str1 = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                var withoutExtension = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                var directoryName = Path.GetDirectoryName(path);
                var str2 = directoryName == null ? "Logs" : Path.Combine(directoryName, "Logs");
                if (!Directory.Exists(str2))
                    Directory.CreateDirectory(str2);

                var path2 = $"{string.Format("{0}_{1}", withoutExtension, str1)}{extension}";
                var destFileName = Path.Combine(str2, path2);
                File.Copy(path, destFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static async Task SaveToFileAsync<T>(T model, string path)
        {
            await Task.Run(() =>
            {
                try
                {
                    var extension = Path.GetExtension(path).ToUpper();
                    switch (extension)
                    {
                        case ".XML":
                        {
                            using var sr = File.CreateText(path);
                            new XmlSerializer(typeof(T)).Serialize(sr, model);
                            break;
                        }
                        case ".JSON":
                        {
                            using var sr = File.CreateText(path);
                            new JsonSerializer().Serialize(sr, model);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });
        }


        public static async Task<T> ReadFromFileAsync<T>(string path) where T : class
        {
            try
            {
                var extension = Path.GetExtension(path).ToUpper();
                switch (extension)
                {
                    case ".XML":
                    {
                        var xmlSerializer = new XmlSerializer(typeof(T));
                        using var sr = File.OpenText(path);
                        var data = xmlSerializer.Deserialize(sr);
                        return data as T;
                    }
                    case ".JSON":
                    {
                        var text = await File.ReadAllTextAsync(path);
                        var data = JsonConvert.DeserializeObject<T>(text);
                        return data;
                    }
                    default:
                        return default;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return default;
            }
        }

    }
}