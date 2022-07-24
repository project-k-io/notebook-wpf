using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using ProjectK.Extensions.Logging;

namespace ProjectK.ToolKit.Extensions;

public static class FileExtensions
{
    private static readonly ILogger Logger = LogManager.GetLogger();

    public static (string path, bool ok) GetNewFileName(string path, string folderName, string suffix, string newExtension = "")
    {
        try
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);
            var directoryName = Path.GetDirectoryName(path);
            var subFolder = string.IsNullOrWhiteSpace(directoryName) ? folderName : Path.Combine(directoryName, folderName);

            if (!string.IsNullOrEmpty(subFolder))
            {
                if (!Directory.Exists(subFolder))
                    Directory.CreateDirectory(subFolder);
            }

            subFolder = string.IsNullOrWhiteSpace(subFolder) ? subFolder : Path.Combine(subFolder, fileNameWithoutExtension);

            if (!string.IsNullOrEmpty(subFolder))
            {
                if (!Directory.Exists(subFolder))
                    Directory.CreateDirectory(subFolder);
            }

            if (!string.IsNullOrEmpty(newExtension))
                extension = newExtension;

            var fileName = $"{suffix}{extension}";
            var destFileName = string.IsNullOrEmpty(subFolder) ? fileName : Path.Combine(subFolder, fileName);
            return (destFileName, true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            return ("", false);
        }
    }

    public static (string path, bool ok) GetNewLogFileName(this string path)
    {
        var suffix = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        return GetNewFileName(path, "Logs", suffix);
    }


    public static void SaveFileToLog(this string path)
    {
        try
        {
            var (s, ok) = GetNewLogFileName(path);
            if (!ok)
                return;

            File.Copy(path, s);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }


    public static async Task<string> GetJsonAsync<T>(this T model)
    {
        await using var stream = new MemoryStream();
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        await JsonSerializer.SerializeAsync(stream, model, options);
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }


    public static async Task SaveToFileAsync<T>(this string path, T model)
    {
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
                        await using var stream = File.Create(path);
                        var options = new JsonSerializerOptions
                        {
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                            WriteIndented = true
                        };
                        await JsonSerializer.SerializeAsync(stream, model, options);
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }


    public static async Task<T> ReadFromFileAsync<T>(this string path) where T : class
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
            Logger.LogError(ex);
            return default;
        }
    }

    public static string MakeUnique(this string path)
    {
        var directoryName = GetDirectoryName(path);
        var withoutExtension = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path);
        var num = 1;
        while (true)
            if (File.Exists(path))
            {
                if (directoryName != null)
                    path = Path.Combine(directoryName, withoutExtension + " " + num + extension);
                ++num;
            }
            else
            {
                break;
            }

        return path;
    }


    public static string GetDirectoryName(this string path)
    {
        return string.IsNullOrEmpty(path) ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(path);
    }
}