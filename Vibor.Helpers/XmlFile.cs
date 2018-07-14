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
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            try
            {
                XFile.CreateFileDirectory(fileName);
                using (XmlWriter w = XmlWriter.Create(fileName, settings))
                {
                    action(w);
                    w.Flush();
                }
            }
            catch (Exception ex)
            {
                XmlFile.log.Error(ex.Message);
            }
        }

        public static void Deserialize(string fileName, XmlReadedDeleage action)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            using (XmlReader r = XmlReader.Create(fileName, new XmlReaderSettings() { IgnoreWhitespace = true }))
            {
                try
                {
                    while (r.Read())
                    {
                        action(r);
                    }
                }
                catch (XmlException ex)
                {
                    XmlFile.log.Error((Exception)ex);
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
            XmlFile.SaveToXmlStream<T>((TextWriter)File.CreateText(filename), t);
        }

        public static string SaveToXmlString<T>(T t)
        {
            StringBuilder sb = new StringBuilder();
            XmlFile.SaveToXmlStream<T>((TextWriter)new StringWriter(sb), t);
            return sb.ToString();
        }

        public static T ReadFromXmlFile<T>(string filename) where T : class
        {
            if (!File.Exists(filename))
            {
                return default(T);
            }

            return XmlFile.ReadFromXmlStream<T>((TextReader)File.OpenText(filename));
        }

        public static T ReadFromXmlString<T>(string xmlText) where T : class
        {
            return XmlFile.ReadFromXmlStream<T>((TextReader)new StringReader(xmlText));
        }

        public static T ReadFromXmlStream<T>(TextReader textReader) where T : class
        {
            try
            {
                XmlTextReader xmlTextReader = new XmlTextReader(textReader);
                object obj = XmlSerializer.FromTypes(new Type[1] { typeof(T) })[0].Deserialize((XmlReader)xmlTextReader);
                xmlTextReader.Close();
                return obj as T;
            }
            catch (Exception ex)
            {
                XmlFile.log.Error(ex);
            }
            return default(T);
        }

        public static void SaveToXmlStream<T>(TextWriter textWriter, T model)
        {
            try
            {
                XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings() { Indent = true, NewLineHandling = NewLineHandling.None });
                XmlSerializer.FromTypes(new Type[1]
                {
          typeof (T)
                })[0].Serialize(xmlWriter, model);
                xmlWriter.Close();
            }
            catch (Exception ex)
            {
                XmlFile.log.Error(ex);
            }
        }

        public static string AndNewSuffixAndExt(string fileName, string suffix, string ext)
        {
            string path = fileName;
            string withoutExtension = Path.GetFileNameWithoutExtension(path);
            string directoryName = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directoryName))
            {
                return string.Empty;
            }

            return Path.Combine(directoryName, withoutExtension + "-" + suffix + "." + ext);
        }

        public static string MakeUnique(string path)
        {
            string directoryName = Path.GetDirectoryName(path);
            string withoutExtension = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            int num = 1;
            while (true)
            {
                if (File.Exists(path))
                {
                    if (directoryName != null)
                    {
                        path = Path.Combine(directoryName, withoutExtension + " " + num + extension);
                    }

                    ++num;
                }
                else
                {
                    break;
                }
            }
            return path;
        }

        public static bool WriteByteArrayToFile(byte[] buff, string fileName)
        {
            bool flag = false;
            try
            {
                BinaryWriter binaryWriter = new BinaryWriter((Stream)new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite));
                binaryWriter.Write(buff);
                binaryWriter.Close();
                flag = true;
            }
            catch (Exception ex)
            {
                XmlFile.log.Error(ex);
            }
            return flag;
        }
    }
}
