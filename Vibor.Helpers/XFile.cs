// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XFile
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vibor.Helpers
{
  public sealed class XFile
  {
    private XFile()
    {
    }

    public static void SaveFile(string fileName, string text)
    {
      using (StreamWriter streamWriter = new StreamWriter(fileName))
        streamWriter.Write(text);
    }

    public static void SaveFile(string fileName, byte[] data)
    {
      XFile.CreateFileDirectory(fileName);
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) File.Open(fileName, FileMode.Create)))
        binaryWriter.Write(data);
    }

    private static string GetPrefix(int n)
    {
      return string.Format("{0}", (object) n.ToString("D4"));
    }

    public static string GetNextSubFolder(string path)
    {
      XFile.CreateDirectory(path);
      string[] directories = Directory.GetDirectories(path);
      int n = 0;
      if (!XList.IsNullOrEmpty<string>((ICollection<string>) directories))
        n = Convert.ToInt32(Path.GetFileName(directories[directories.Length - 1])) + 1;
      string prefix = XFile.GetPrefix(n);
      return Path.Combine(path, prefix);
    }

    public static string GetLastSubFolder(string path)
    {
      string[] directories = Directory.GetDirectories(path);
      if (XList.IsNullOrEmpty<string>((ICollection<string>) directories))
        return string.Empty;
      return ((IEnumerable<string>) directories).LastOrDefault<string>();
    }

    public static bool ReadFromBinaryFile(string fileName, BinaryReaderDelegate action)
    {
      if (!File.Exists(fileName))
        return false;
      using (BinaryReader br = new BinaryReader((Stream) File.Open(fileName, FileMode.Open)))
        return action(br);
    }

    public static void WriteToBinaryFile(string fileName, BinaryWriterDelegate action)
    {
      XFile.CreateFileDirectory(fileName);
      using (BinaryWriter bw = new BinaryWriter((Stream) File.Open(fileName, FileMode.Create)))
        action(bw);
    }

    public static bool ReadFromTextFile(string fileName, LinesReaderDelegate action, char separator = ',')
    {
      if (!File.Exists(fileName))
        return false;
      string[] lines = File.ReadAllLines(fileName);
      if (XList.IsNullOrEmpty<string>((ICollection<string>) lines))
        return false;
      return action(separator, lines);
    }

    public static void WriteToTextFile(string fileName, LinesWriterDelegate action, char separator = ',')
    {
      List<string> lines = new List<string>();
      action(separator, lines);
      XFile.WriteAllLines(fileName, (IEnumerable<string>) lines.ToArray());
    }

    public static List<string> GetNamesFromFileNames(string path)
    {
      List<string> names = new List<string>();
      XFile.GetNamesFromFileNames(path, names);
      return names;
    }

    public static bool GetNamesFromFileNames(string path, List<string> names)
    {
      foreach (string file in Directory.GetFiles(path))
        names.Add(Path.GetFileNameWithoutExtension(file));
      return true;
    }

    public static void WriteAllLines(string path, IEnumerable<string> contents)
    {
      XFile.CreateFileDirectory(path);
      File.WriteAllLines(path, contents);
    }

    public static void CreateFileDirectory(string fileName)
    {
      XFile.CreateDirectory(Path.GetDirectoryName(fileName));
    }

    public static void CreateDirectory(string path)
    {
      if (string.IsNullOrEmpty(path) || Directory.Exists(path))
        return;
      Directory.CreateDirectory(path);
    }

    public static bool BinaryCompare(string file1, string file2)
    {
      try
      {
        if (file1.Length <= 0)
          throw new ArgumentException();
        FileStream fileStream1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
        if (file2.Length <= 0)
          throw new ArgumentException();
        FileStream fileStream2 = new FileStream(file2, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader1 = new BinaryReader((Stream) fileStream1);
        byte[] buffer1 = new byte[1024];
        BinaryReader binaryReader2 = new BinaryReader((Stream) fileStream2);
        byte[] buffer2 = new byte[1024];
        bool flag = true;
        int num1;
        do
        {
          num1 = binaryReader1.Read(buffer1, 0, 1024);
          if (num1 > 0)
          {
            int num2 = binaryReader2.Read(buffer2, 0, 1024);
            if (num1 != num2)
            {
              flag = false;
              break;
            }
            for (int index = 0; index < num1; ++index)
            {
              if ((int) buffer1[index] != (int) buffer2[index])
              {
                flag = false;
                break;
              }
            }
            if (!flag)
              break;
          }
        }
        while (num1 > 0);
        fileStream1.Close();
        fileStream2.Close();
        return flag;
      }
      catch
      {
        Console.WriteLine("Cannot find or read file to dump.");
        return false;
      }
    }

    public static async Task<byte[]> ReadBytesAsync(string filePath)
    {
      byte[] numArray;
      using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
        numArray = await XFile.ReadBytesAsync(stream);
      return numArray;
    }

    public static async Task<byte[]> ReadBytesAsync(FileStream stream)
    {
      byte[] bytes = new byte[stream.Length];
      int num = await stream.ReadAsync(bytes, 0, (int) stream.Length);
      return bytes;
    }
  }
}
