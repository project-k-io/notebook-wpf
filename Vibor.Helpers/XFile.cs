using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;

namespace ProjectK.Utils
{
    public class XFile
    {
        private static readonly ILogger Logger = LogManager.GetLogger<XFile>();

        public static (string path, bool ok) GetNewLogFileName(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return ("", false);

                var suffix = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                var directoryName = Path.GetDirectoryName(path);
                var logs = string.IsNullOrWhiteSpace(directoryName) ? "Logs" : Path.Combine(directoryName, "Logs");

                if (!Directory.Exists(logs))
                    Directory.CreateDirectory(logs);

                logs = string.IsNullOrWhiteSpace(fileNameWithoutExtension) ? logs : Path.Combine(logs, fileNameWithoutExtension);
                if (!Directory.Exists(logs))
                    Directory.CreateDirectory(logs);

                var fileName = $"{suffix}{extension}";
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


        // write asynchronously to a file
        public static async Task SaveJsonToFileAsync<T>(T model, string path)
        {
        }

        public static async Task SaveToFileAsync<T>(T model, string path)
        {
            Logger.LogDebug($"SaveToFileAsync: {path}");
            try
            {
                var extension = Path.GetExtension(path).ToUpper();
                switch (extension)
                {
                    case ".XML":
                        {
                            await using var sr = File.Create(path);
                            new XmlSerializer(typeof(T)).Serialize(sr, model);
                        }
                        break;
                    case ".JSON":
                        {
                            await using var fs = File.Create(path);
                            var option = new JsonSerializerOptions {WriteIndented = true};
                            await JsonSerializer.SerializeAsync<T>(fs, model, option);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
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
                            await using var fs = File.OpenRead(path);
                            var data = new XmlSerializer(typeof(T)).Deserialize(fs);
                            return data as T;
                        }
                    case ".JSON":
                    {
                        await using var fs = File.OpenRead(path);
                        var data = await JsonSerializer.DeserializeAsync<T>(fs);
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