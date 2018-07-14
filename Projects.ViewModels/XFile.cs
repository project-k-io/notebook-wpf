// Decompiled with JetBrains decompiler
// Type: Projects.ViewModels.XFile
// Assembly: Projects.ViewModels, Version=1.1.8.29121, Culture=neutral, PublicKeyToken=null
// MVID: AA177939-1C69-401F-8524-6C17EE86E3CA
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.ViewModels.dll

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
        string str1 = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string withoutExtension = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path);
        string directoryName = Path.GetDirectoryName(path);
        string str2 = directoryName == null ? "Logs" : Path.Combine(directoryName, "Logs");
        if (!Directory.Exists(str2))
          Directory.CreateDirectory(str2);
        string path2 = string.Format("{0}{1}", (object) string.Format("{0}_{1}", (object) withoutExtension, (object) str1), (object) extension);
        string destFileName = Path.Combine(str2, path2);
        File.Copy(path, destFileName);
      }
      catch (Exception ex)
      {
        Debug.WriteLine((object) ex);
      }
    }

    public static async Task SaveToFileAsync<T>(T model, string filename)
    {
      await Task.Run((Action) (() =>
      {
        try
        {
          StreamWriter text = File.CreateText(filename);
          new XmlSerializer(typeof (T)).Serialize((TextWriter) text, (object) (T) model);
          text.Close();
        }
        catch (Exception ex)
        {
          Debug.WriteLine((object) ex);
        }
      }));
    }

    public static async Task<T> ReadFromFileAsync<T>(string filename) where T : class
    {
      return await Task.Run<T>((Func<T>) (() =>
      {
        try
        {
          XmlSerializer xmlSerializer = new XmlSerializer(typeof (T));
          StreamReader streamReader = File.OpenText(filename);
          object obj = xmlSerializer.Deserialize((TextReader) streamReader);
          streamReader.Close();
          return obj as T;
        }
        catch (Exception ex)
        {
          Debug.WriteLine((object) ex);
          return default (T);
        }
      }));
    }
  }
}
