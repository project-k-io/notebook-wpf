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

        public static (string path, bool ok)  GetNewLogFileName(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return ("", false);

                var suffix = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                var withoutExtension = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                var directoryName = Path.GetDirectoryName(path);
                var logs = string.IsNullOrWhiteSpace(directoryName) ? "Logs" : Path.Combine(directoryName, "Logs");

                if (!Directory.Exists(logs))
                    Directory.CreateDirectory(logs);

                var fileName = $"{withoutExtension}_{suffix}{extension}";
                var destFileName = Path.Combine(logs, fileName);
                return (destFileName, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ("", false);
            }
        }


        public static void SaveFileToLog(string path)
        {
            try
            {
                var r = GetNewLogFileName(path);
                if (!r.ok)
                    return;

                File.Copy(path, r.path);
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