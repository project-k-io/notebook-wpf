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
            using (var streamWriter = new StreamWriter(fileName))
            {
                streamWriter.Write(text);
            }
        }

        public static void SaveFile(string fileName, byte[] data)
        {
            CreateFileDirectory(fileName);
            using (var binaryWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                binaryWriter.Write(data);
            }
        }

        private static string GetPrefix(int n)
        {
            return string.Format("{0}", n.ToString("D4"));
        }

        public static string GetNextSubFolder(string path)
        {
            CreateDirectory(path);
            var directories = Directory.GetDirectories(path);
            var n = 0;
            if (!XList.IsNullOrEmpty(directories))
                n = Convert.ToInt32(Path.GetFileName(directories[directories.Length - 1])) + 1;
            var prefix = GetPrefix(n);
            return Path.Combine(path, prefix);
        }

        public static string GetLastSubFolder(string path)
        {
            var directories = Directory.GetDirectories(path);
            if (XList.IsNullOrEmpty(directories))
                return string.Empty;
            return directories.LastOrDefault();
        }

        public static bool ReadFromBinaryFile(string fileName, BinaryReaderDelegate action)
        {
            if (!File.Exists(fileName))
                return false;
            using (var br = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                return action(br);
            }
        }

        public static void WriteToBinaryFile(string fileName, BinaryWriterDelegate action)
        {
            CreateFileDirectory(fileName);
            using (var bw = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                action(bw);
            }
        }

        public static bool ReadFromTextFile(string fileName, LinesReaderDelegate action, char separator = ',')
        {
            if (!File.Exists(fileName))
                return false;
            var lines = File.ReadAllLines(fileName);
            if (XList.IsNullOrEmpty(lines))
                return false;
            return action(separator, lines);
        }

        public static void WriteToTextFile(string fileName, LinesWriterDelegate action, char separator = ',')
        {
            var lines = new List<string>();
            action(separator, lines);
            WriteAllLines(fileName, lines.ToArray());
        }

        public static List<string> GetNamesFromFileNames(string path)
        {
            var names = new List<string>();
            GetNamesFromFileNames(path, names);
            return names;
        }

        public static bool GetNamesFromFileNames(string path, List<string> names)
        {
            foreach (var file in Directory.GetFiles(path))
                names.Add(Path.GetFileNameWithoutExtension(file));
            return true;
        }

        public static void WriteAllLines(string path, IEnumerable<string> contents)
        {
            CreateFileDirectory(path);
            File.WriteAllLines(path, contents);
        }

        public static void CreateFileDirectory(string fileName)
        {
            CreateDirectory(Path.GetDirectoryName(fileName));
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
                var fileStream1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
                if (file2.Length <= 0)
                    throw new ArgumentException();
                var fileStream2 = new FileStream(file2, FileMode.Open, FileAccess.Read);
                var binaryReader1 = new BinaryReader(fileStream1);
                var buffer1 = new byte[1024];
                var binaryReader2 = new BinaryReader(fileStream2);
                var buffer2 = new byte[1024];
                var flag = true;
                int num1;
                do
                {
                    num1 = binaryReader1.Read(buffer1, 0, 1024);
                    if (num1 > 0)
                    {
                        var num2 = binaryReader2.Read(buffer2, 0, 1024);
                        if (num1 != num2)
                        {
                            flag = false;
                            break;
                        }

                        for (var index = 0; index < num1; ++index)
                            if (buffer1[index] != buffer2[index])
                            {
                                flag = false;
                                break;
                            }

                        if (!flag)
                            break;
                    }
                } while (num1 > 0);

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
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                numArray = await ReadBytesAsync(stream);
            }

            return numArray;
        }

        public static async Task<byte[]> ReadBytesAsync(FileStream stream)
        {
            var bytes = new byte[stream.Length];
            var num = await stream.ReadAsync(bytes, 0, (int) stream.Length);
            return bytes;
        }
    }
}