using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Projects.ViewModels
{
    public class XFile
    {
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
                var path2 = string.Format("{0}{1}", string.Format("{0}_{1}", withoutExtension, str1), extension);
                var destFileName = Path.Combine(str2, path2);
                File.Copy(path, destFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static async Task SaveToFileAsync<T>(T model, string filename)
        {
            await Task.Run(() =>
            {
                try
                {
                    var text = File.CreateText(filename);
                    new XmlSerializer(typeof(T)).Serialize(text, model);
                    text.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });
        }

        public static async Task<T> ReadFromFileAsync<T>(string filename) where T : class
        {
            return await Task.Run(() =>
            {
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    var streamReader = File.OpenText(filename);
                    var obj = xmlSerializer.Deserialize(streamReader);
                    streamReader.Close();
                    return obj as T;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return default(T);
                }
            });
        }
    }
}