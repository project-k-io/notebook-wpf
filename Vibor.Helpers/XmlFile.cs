// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XmlFile
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Vibor.Logging;

namespace Vibor.Helpers
{
    public class XmlFile
    {
        private static readonly ILog log = LogManager.GetLogger("XmlFile");

        public static void Serialize(string fileName, XmlWriterDeleage action)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            try
            {
                XFile.CreateFileDirectory(fileName);
                using (var w = XmlWriter.Create(fileName, settings))
                {
                    action(w);
                    w.Flush();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        public static void Deserialize(string fileName, XmlReadedDeleage action)
        {
            if (!File.Exists(fileName)) return;

            using (var r = XmlReader.Create(fileName, new XmlReaderSettings {IgnoreWhitespace = true}))
            {
                try
                {
                    while (r.Read()) action(r);
                }
                catch (XmlException ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void Write(XmlWriter w, string name, object value)
        {
            w.WriteStartElement(name);
            w.WriteString(value.ToString());
            w.WriteEndElement();
        }

        public static void SaveToXmlFile<T>(T t, string filename)
        {
            XFile.CreateFileDirectory(filename);
            SaveToXmlStream(File.CreateText(filename), t);
        }

        public static string SaveToXmlString<T>(T t)
        {
            var sb = new StringBuilder();
            SaveToXmlStream(new StringWriter(sb), t);
            return sb.ToString();
        }

        public static T ReadFromXmlFile<T>(string filename) where T : class
        {
            if (!File.Exists(filename)) return default(T);

            return ReadFromXmlStream<T>(File.OpenText(filename));
        }

        public static T ReadFromXmlString<T>(string xmlText) where T : class
        {
            return ReadFromXmlStream<T>(new StringReader(xmlText));
        }

        public static T ReadFromXmlStream<T>(TextReader textReader) where T : class
        {
            try
            {
                var xmlTextReader = new XmlTextReader(textReader);
                var obj = XmlSerializer.FromTypes(new Type[1] {typeof(T)})[0].Deserialize(xmlTextReader);
                xmlTextReader.Close();
                return obj as T;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return default(T);
        }

        public static void SaveToXmlStream<T>(TextWriter textWriter, T model)
        {
            try
            {
                var xmlWriter = XmlWriter.Create(textWriter,
                    new XmlWriterSettings {Indent = true, NewLineHandling = NewLineHandling.None});
                XmlSerializer.FromTypes(new Type[1]
                {
                    typeof(T)
                })[0].Serialize(xmlWriter, model);
                xmlWriter.Close();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static string AndNewSuffixAndExt(string fileName, string suffix, string ext)
        {
            var path = fileName;
            var withoutExtension = Path.GetFileNameWithoutExtension(path);
            var directoryName = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directoryName)) return string.Empty;

            return Path.Combine(directoryName, withoutExtension + "-" + suffix + "." + ext);
        }

        public static string MakeUnique(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
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
                log.Error(ex);
            }

            return flag;
        }
    }
}