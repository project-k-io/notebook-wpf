// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.Models.XFile2
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Vibor.Generic.Models
{
  public class XFile2
  {
    private static readonly ILog Logger = LogManager.GetLogger("CommandsViewModel");

    public static void SaveToXmlFile<T>(T t, string filename)
    {
      XFile2.SaveToXmlStream<T>((TextWriter) File.CreateText(filename), t);
    }

    public static string SaveToXmlString<T>(T t)
    {
      StringBuilder sb = new StringBuilder();
      if ((object) t != null)
        XFile2.SaveToXmlStream<T>((TextWriter) new StringWriter(sb), t);
      return sb.ToString();
    }

    public static T ReadFromXmlFile<T>(string filename) where T : class, new()
    {
      if (!File.Exists(filename))
        return Activator.CreateInstance<T>();
      return XFile2.ReadFromXmlStream<T>((TextReader) File.OpenText(filename));
    }

    public static T ReadFromXmlString<T>(string xmlText) where T : class
    {
      return XFile2.ReadFromXmlStream<T>((TextReader) new StringReader(xmlText));
    }

    private static T ReadFromXmlStream<T>(TextReader textReader) where T : class
    {
      try
      {
        XmlTextReader xmlTextReader = new XmlTextReader(textReader);
        object obj = XmlSerializer.FromTypes(new Type[1]
        {
          typeof (T)
        })[0].Deserialize((XmlReader) xmlTextReader);
        xmlTextReader.Close();
        textReader.Close();
        return obj as T;
      }
      catch (Exception ex)
      {
        XFile2.Logger.Error((object) ex);
      }
      return default (T);
    }

    public static void SaveToXmlStream<T>(TextWriter textWriter, T model)
    {
      try
      {
        XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings()
        {
          Indent = true,
          NewLineHandling = NewLineHandling.None
        });
        XmlSerializer.FromTypes(new Type[1]
        {
          typeof (T)
        })[0].Serialize(xmlWriter, (object) model);
        xmlWriter.Close();
        textWriter.Close();
      }
      catch (Exception ex)
      {
        XFile2.Logger.Error((object) ex);
      }
    }

    public static bool WriteByteArrayToFile(byte[] buff, string fileName)
    {
      bool flag = false;
      try
      {
        BinaryWriter binaryWriter = new BinaryWriter((Stream) new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite));
        binaryWriter.Write(buff);
        binaryWriter.Close();
        flag = true;
      }
      catch (Exception ex)
      {
        XFile2.Logger.Error((object) ex);
      }
      return flag;
    }

    public static string AndSuffix(string fileName, string suffix)
    {
      return XFile2.AndSuffixOrPrefixAndExt(fileName, suffix, Path.GetExtension(fileName), true);
    }

    public static string AndPrefix(string fileName, string prefix)
    {
      return XFile2.AndSuffixOrPrefixAndExt(fileName, prefix, Path.GetExtension(fileName), false);
    }

    public static string AndSuffixOrPrefixAndExt(string filePath, string fix, string ext, bool isSuffix = true)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(filePath);
      return Path.Combine(XFile2.GetDirectoryName(filePath), (isSuffix ? withoutExtension + fix : fix + withoutExtension) + ext);
    }

    public static string MakeUnique(string path)
    {
      string directoryName = XFile2.GetDirectoryName(path);
      string withoutExtension = Path.GetFileNameWithoutExtension(path);
      string extension = Path.GetExtension(path);
      int num = 1;
      while (true)
      {
        if (File.Exists(path))
        {
          if (directoryName != null)
            path = Path.Combine(directoryName, withoutExtension + " " + (object) num + extension);
          ++num;
        }
        else
          break;
      }
      return path;
    }

    public static string GetFileNameFromDateAndTime(DateTime d)
    {
      return string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", (object) d);
    }

    public static string GetFileNameFromTime(DateTime d)
    {
      return string.Format("{0:hh-mm-ss-tt}", (object) d);
    }

    public static string GetFileNameFromDate(DateTime d)
    {
      return string.Format("{0:yyyy-MM-dd}", (object) d);
    }

    public static string GetDirectoryName(string path)
    {
      return string.IsNullOrEmpty(path) ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(path);
    }

    public static bool MoveFileToFolder(string oldFilePath, string folderName)
    {
      try
      {
        string fileName = Path.GetFileName(oldFilePath);
        if (string.IsNullOrEmpty(fileName))
          return false;
        if (!Directory.Exists(folderName))
          Directory.CreateDirectory(folderName);
        string str = Path.Combine(folderName, fileName);
        File.Copy(oldFilePath, str);
        if (File.Exists(str))
          File.Delete(oldFilePath);
        return true;
      }
      catch (Exception ex)
      {
        XFile2.Logger.Error((object) ex);
        return false;
      }
    }

    public static string GetDateTimeSubFolder(string path, string folder)
    {
      return XFile2.GetDateTimeSubFolder(DateTime.Now, path, folder);
    }

    public static string GetDateTimeSubFolder(DateTime date, string path, string folder)
    {
      string str = Path.Combine(path, folder);
      if (!Directory.Exists(str))
        Directory.CreateDirectory(str);
      string nameFromDateAndTime = XFile2.GetFileNameFromDateAndTime(date);
      string path1 = Path.Combine(str, nameFromDateAndTime);
      if (!Directory.Exists(path1))
        Directory.CreateDirectory(path1);
      return path1;
    }

    public static List<string> GetLines(string path)
    {
      List<string> stringList = new List<string>();
      using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream))
        {
          while (streamReader.Peek() >= 0)
            stringList.Add(streamReader.ReadLine());
        }
      }
      return stringList;
    }

    public static IList<string> OpenCsvText(string text, ILog logger)
    {
      if (string.IsNullOrEmpty(text))
        return (IList<string>) null;
      try
      {
        return (IList<string>) Regex.Split(text, "\r\n");
      }
      catch (Exception ex)
      {
        logger.Error((object) ex);
        return (IList<string>) null;
      }
    }
  }
}
