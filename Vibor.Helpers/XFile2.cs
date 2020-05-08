using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Vibor.Helpers
{
    public class XFile2
    {
        private static readonly ILogger Logger = LogManager.GetLogger<XFile2>();

        public static void SaveToXmlFile<T>(T t, string filename)
        {
            SaveToXmlStream(File.CreateText(filename), t);
        }

        public static string SaveToXmlString<T>(T t)
        {
            var sb = new StringBuilder();
            if (t != null)
                SaveToXmlStream(new StringWriter(sb), t);
            return sb.ToString();
        }

        public static T ReadFromXmlFile<T>(string filename) where T : class, new()
        {
            if (!File.Exists(filename))
                return Activator.CreateInstance<T>();
            return ReadFromXmlStream<T>(File.OpenText(filename));
        }

        public static T ReadFromXmlString<T>(string xmlText) where T : class
        {
            return ReadFromXmlStream<T>(new StringReader(xmlText));
        }

        private static T ReadFromXmlStream<T>(TextReader textReader) where T : class
        {
            try
            {
                var xmlTextReader = new XmlTextReader(textReader);
                var obj = XmlSerializer.FromTypes(new Type[1]
                {
                    typeof(T)
                })[0].Deserialize(xmlTextReader);
                xmlTextReader.Close();
                textReader.Close();
                return obj as T;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return default;
        }

        public static void SaveToXmlStream<T>(TextWriter textWriter, T model)
        {
            try
            {
                var xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings
                {
                    Indent = true,
                    NewLineHandling = NewLineHandling.None
                });
                XmlSerializer.FromTypes(new Type[1]
                {
                    typeof(T)
                })[0].Serialize(xmlWriter, model);
                xmlWriter.Close();
                textWriter.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public static bool WriteByteArrayToFile(byte[] buff, string fileName)
        {
            var flag = false;
            try
            {
                var binaryWriter = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite));
                binaryWriter.Write(buff);
                binaryWriter.Close();
                flag = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return flag;
        }

        public static string AndSuffix(string fileName, string suffix)
        {
            return AndSuffixOrPrefixAndExt(fileName, suffix, Path.GetExtension(fileName));
        }

        public static string AndPrefix(string fileName, string prefix)
        {
            return AndSuffixOrPrefixAndExt(fileName, prefix, Path.GetExtension(fileName), false);
        }

        public static string AndSuffixOrPrefixAndExt(string filePath, string fix, string ext, bool isSuffix = true)
        {
            var withoutExtension = Path.GetFileNameWithoutExtension(filePath);
            return Path.Combine(GetDirectoryName(filePath),
                (isSuffix ? withoutExtension + fix : fix + withoutExtension) + ext);
        }

        public static string MakeUnique(string path)
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

        public static string GetFileNameFromDateAndTime(DateTime d)
        {
            return $"{d:yyyy-MM-dd_hh-mm-ss-tt}";
        }

        public static string GetFileNameFromTime(DateTime d)
        {
            return $"{d:hh-mm-ss-tt}";
        }

        public static string GetFileNameFromDate(DateTime d)
        {
            return $"{d:yyyy-MM-dd}";
        }

        public static string GetDirectoryName(string path)
        {
            return string.IsNullOrEmpty(path) ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(path);
        }

        public static bool MoveFileToFolder(string oldFilePath, string folderName)
        {
            try
            {
                var fileName = Path.GetFileName(oldFilePath);
                if (string.IsNullOrEmpty(fileName))
                    return false;
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);
                var str = Path.Combine(folderName, fileName);
                File.Copy(oldFilePath, str);
                if (File.Exists(str))
                    File.Delete(oldFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        public static string GetDateTimeSubFolder(string path, string folder)
        {
            return GetDateTimeSubFolder(DateTime.Now, path, folder);
        }

        public static string GetDateTimeSubFolder(DateTime date, string path, string folder)
        {
            var str = Path.Combine(path, folder);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            var nameFromDateAndTime = GetFileNameFromDateAndTime(date);
            var path1 = Path.Combine(str, nameFromDateAndTime);
            if (!Directory.Exists(path1))
                Directory.CreateDirectory(path1);
            return path1;
        }

        public static List<string> GetLines(string path)
        {
            var stringList = new List<string>();
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    while (streamReader.Peek() >= 0)
                        stringList.Add(streamReader.ReadLine());
                }
            }

            return stringList;
        }

        public static IList<string> OpenCsvText(string text, ILogger logger)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            try
            {
                return Regex.Split(text, "\r\n");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }
    }
}