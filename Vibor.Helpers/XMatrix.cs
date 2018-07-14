// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.Models.XMatrix
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vibor.Generic.Models
{
  public class XMatrix
  {
    private static readonly ILog Logger = LogManager.GetLogger("StartUpFlowViewModel");

    public static void CheckSize<T>(List<List<T>> m, int colCount, int rowCount, T zero = null)
    {
      if (XMatrix.IsSameDimensions<T>(m, colCount, rowCount))
        return;
      XMatrix.Resize<T>(m, colCount, rowCount, default (T));
    }

    public static void Resize<T>(List<List<T>> m, int colCount, int rowCount, T zero = null)
    {
      XMatrix.Clear<T>(m);
      for (int index1 = 0; index1 < rowCount; ++index1)
      {
        List<T> objList = new List<T>();
        for (int index2 = 0; index2 < colCount; ++index2)
          objList.Add(zero);
        m.Add(objList);
      }
    }

    public static void Init<T>(List<List<T>> matrixA, List<List<T>> matrixB, T zero = null)
    {
      matrixA.AddRange(matrixB.Select<List<T>, List<T>>((Func<List<T>, List<T>>) (rowB => rowB.Select<T, T>((Func<T, T>) (value => zero)).ToList<T>())));
    }

    public static List<List<T>> CopyZero<T>(List<List<T>> a)
    {
      List<List<T>> matrixA = new List<List<T>>();
      XMatrix.Init<T>(matrixA, a, default (T));
      return matrixA;
    }

    public static List<List<T>> Copy<T>(List<List<T>> matrixA)
    {
      List<List<T>> objListList = new List<List<T>>();
      foreach (List<T> objList1 in matrixA)
      {
        List<T> objList2 = new List<T>();
        objList2.AddRange((IEnumerable<T>) objList1);
        objListList.Add(objList2);
      }
      return objListList;
    }

    public static List<List<T>> CopyListToMatrix<T>(List<T> list, List<List<T>> templateMatrix)
    {
      if (XList.IsNullOrEmpty<T>((ICollection<T>) list))
        return (List<List<T>>) null;
      List<List<T>> objListList = new List<List<T>>();
      int num = 0;
      foreach (List<T> objList1 in templateMatrix)
      {
        List<T> objList2 = new List<T>();
        foreach (T obj1 in objList1)
        {
          if (num >= list.Count)
            return (List<List<T>>) null;
          T obj2 = list[num++];
          objList2.Add(obj2);
        }
        objListList.Add(objList2);
      }
      return objListList;
    }

    public static List<List<T>> Copy<T>(List<T> list, int colCount, int rowCount)
    {
      if (XList.IsNullOrEmpty<T>((ICollection<T>) list) || colCount * rowCount != list.Count)
        return (List<List<T>>) null;
      List<List<T>> objListList = new List<List<T>>();
      int num = 0;
      for (int index1 = 0; index1 < rowCount; ++index1)
      {
        List<T> objList = new List<T>();
        for (int index2 = 0; index2 < colCount; ++index2)
        {
          T obj = list[num++];
          objList.Add(obj);
        }
        objListList.Add(objList);
      }
      return objListList;
    }

    public static List<List<T1>> Copy<T1, T2>(List<List<T2>> matrix, T1 value)
    {
      return matrix.Select<List<T2>, List<T1>>((Func<List<T2>, List<T1>>) (rowA => rowA.Select<T2, T1>((Func<T2, T1>) (v => value)).ToList<T1>())).ToList<List<T1>>();
    }

    public static List<List<T2>> MakeCopy<T1, T2>(List<List<T1>> m, Func<T1, T2> cast)
    {
      return m.Select<List<T1>, List<T2>>((Func<List<T1>, List<T2>>) (r => r.Select<T1, T2>(cast).ToList<T2>())).ToList<List<T2>>();
    }

    public static List<List<short>> MakeCopy(List<List<int>> m)
    {
      return XMatrix.MakeCopy<int, short>(m, (Func<int, short>) (a => (short) a));
    }

    public static List<List<short>> MakeCopy(List<List<float>> m)
    {
      return XMatrix.MakeCopy<float, short>(m, (Func<float, short>) (a => (short) a));
    }

    public static void Reset<T>(List<List<T>> m, T t)
    {
      for (int index1 = 0; index1 < m.Count; ++index1)
      {
        List<T> objList = m[index1];
        for (int index2 = 0; index2 < objList.Count; ++index2)
          objList[index2] = t;
      }
    }

    public static bool IsSameDimensions<T1, T2>(List<List<T1>> a, List<List<T2>> b)
    {
      return XMatrix.IsSameDimensions<T1>(a, b.Count > 0 ? b[0].Count : 0, b.Count);
    }

    public static bool IsSameDimensions<T>(List<List<T>> m, int colCount, int rowCount)
    {
      return m != null && m.Count == rowCount && (m.Count >= 1 && m[0].Count == colCount);
    }

    public static bool IsEqual<T>(List<List<T>> a, List<List<T>> b, T delta, Func<T, T, T, bool> equals) where T : IComparable
    {
      if (!XMatrix.IsSameDimensions<T, T>(a, b))
        return false;
      for (int index1 = 0; index1 < a.Count; ++index1)
      {
        List<T> objList1 = a[index1];
        List<T> objList2 = b[index1];
        for (int index2 = 0; index2 < objList1.Count; ++index2)
        {
          if (!equals(objList1[index2], objList2[index2], delta))
            return false;
        }
      }
      return true;
    }

    public static bool IsEqual<T>(List<List<T>> matrxiA, List<List<T>> matrixB) where T : IComparable
    {
      return XMatrix.IsEqual<T>(matrxiA, matrixB, default (T), (Func<T, T, T, bool>) ((a, b, d) => a.Equals((object) b)));
    }

    public static bool IsEqual(List<List<double>> matrixA, List<List<double>> matrixB, double delta)
    {
      return XMatrix.IsEqual<double>(matrixA, matrixB, delta, new Func<double, double, double, bool>(XMatrix.IsEqual));
    }

    public static bool IsEqual(double a, double b, double delta)
    {
      return Math.Abs(a - b) <= delta;
    }

    public static void Clear<T>(List<List<T>> m)
    {
      if (XList.IsNullOrEmpty<List<T>>((ICollection<List<T>>) m))
        return;
      foreach (List<T> objList in m)
        objList.Clear();
      m.Clear();
    }

    public static void SetValues<T>(List<List<T>> a, List<List<T>> b)
    {
      if (XList.IsNullOrEmpty<List<T>>((ICollection<List<T>>) a))
        return;
      for (int index1 = 0; index1 < a.Count; ++index1)
      {
        List<T> objList1 = a[index1];
        List<T> objList2 = b[index1];
        for (int index2 = 0; index2 < objList1.Count; ++index2)
          objList1[index2] = objList2[index2];
      }
    }

    public static string GetText<T>(List<List<T>> m, string formatGood, string formatBad, Func<T, bool> isGood)
    {
      return XMatrix.GetText<T>(m, (Func<T, string>) (a => isGood(a) ? formatGood : formatBad), ',', true);
    }

    public static string GetText<T>(List<List<T>> m, string format = "{0,5}", char separator = ',', bool bAppendLine = true)
    {
      return XMatrix.GetText<T>(m, (Func<T, string>) (a => format), separator, bAppendLine);
    }

    public static string GetText<T>(List<List<T>> m, Func<T, string> format, char separator = ',', bool bAppendLine = true)
    {
      if (XList.IsNullOrEmpty<List<T>>((ICollection<List<T>>) m))
        return "";
      string separator1 = separator.ToString();
      StringBuilder sb = new StringBuilder();
      foreach (List<T> list in m)
      {
        XList.AddSeparatedText<T>(sb, list, format, separator1);
        if (bAppendLine)
          sb.AppendLine();
      }
      return sb.ToString();
    }

    public static void PopulateMatrix<T>(List<List<T>> matrix, string text, Func<string, T> convert, char separator = ',')
    {
      if (string.IsNullOrEmpty(text))
        return;
      using (StringReader stringReader = new StringReader(text))
      {
        string line;
        while ((line = stringReader.ReadLine()) != null)
        {
          List<T> objList = XList.PopulateArray<T>(line, convert, separator);
          if (!XList.IsNullOrEmpty<T>((ICollection<T>) objList))
            matrix.Add(objList);
        }
      }
    }

    public static List<List<T>> GetMatrix<T>(string text, Func<string, T> convert, char separator = ',')
    {
      List<List<T>> matrix = new List<List<T>>();
      XMatrix.PopulateMatrix<T>(matrix, text, convert, separator);
      return matrix;
    }

    public static List<List<int>> GetIntMatrix(string text, char separator = ',')
    {
      return XMatrix.GetMatrix<int>(text, new Func<string, int>(XConverter.ConvertToInt), separator);
    }

    public static List<List<double>> GetDoubleMatrix(string text, char separator = ',')
    {
      return XMatrix.GetMatrix<double>(text, new Func<string, double>(XConverter.ConvertToDouble), separator);
    }

    public static List<List<int>> GetMatrixHex(string text)
    {
      Func<string, int> convert = (Func<string, int>) (s =>
      {
        int num;
        try
        {
          num = Convert.ToInt32(s, 16);
        }
        catch
        {
          num = 0;
        }
        return num;
      });
      return XMatrix.GetMatrix<int>(text, convert, ',');
    }

    public static void AddMatrix<T>(List<List<T>> a, List<List<T>> b, Func<T, T, T> add)
    {
      XMatrix.AddMatrix<T, T>(a, b, add);
    }

    public static void AddMatrix<T1, T2>(List<List<T1>> a, List<List<T2>> b, Func<T1, T2, T1> add)
    {
      try
      {
        if (a.Count != b.Count)
          XMatrix.Logger.Warn((object) "Row Counts are not equal");
        int num1 = Math.Min(a.Count, b.Count);
        for (int index1 = 0; index1 < num1; ++index1)
        {
          List<T1> objList1 = a[index1];
          List<T2> objList2 = b[index1];
          if (objList1.Count != objList2.Count)
            XMatrix.Logger.Warn((object) "Column Counts are not equal");
          int num2 = Math.Min(objList1.Count, objList2.Count);
          for (int index2 = 0; index2 < num2; ++index2)
            objList1[index2] = add(objList1[index2], objList2[index2]);
        }
      }
      catch (Exception ex)
      {
        XMatrix.Logger.Error((object) ex);
        throw;
      }
    }

    public static List<List<double>> CreateAverageMatrix(List<List<List<short>>> matrixSamples)
    {
      List<List<double>> averageMatrix = XMatrix.Copy<double, short>(matrixSamples[0], 0.0);
      XMatrix.Avearge(averageMatrix, (ICollection<List<List<short>>>) matrixSamples);
      return averageMatrix;
    }

    public static void Avearge(List<List<double>> averageMatrix, ICollection<List<List<short>>> matrixSamples)
    {
      XMatrix.Avearge<double, short>(averageMatrix, matrixSamples, (Func<double, short, double>) ((a, b) => a + (double) b), (Func<double, int, double>) ((a, b) => a / (double) b), 0.0);
    }

    public static void Avearge(List<List<int>> averageMatrix, ICollection<List<List<short>>> matrixSamples)
    {
      XMatrix.Avearge<int, short>(averageMatrix, matrixSamples, (Func<int, short, int>) ((a, b) => a + (int) b), (Func<int, int, int>) ((a, b) => a / b), 0);
    }

    public static void Avearge<T1, T2>(List<List<T1>> averageMatrix, ICollection<List<List<T2>>> matrixSamples, Func<T1, T2, T1> add, Func<T1, int, T1> divide, T1 zero = null)
    {
      XMatrix.Reset<T1>(averageMatrix, zero);
      foreach (List<List<T2>> matrixSample in (IEnumerable<List<List<T2>>>) matrixSamples)
        XMatrix.AddMatrix<T1, T2>(averageMatrix, matrixSample, add);
      int count = matrixSamples.Count;
      foreach (List<T1> objList in averageMatrix)
      {
        for (int index = 0; index < objList.Count; ++index)
          objList[index] = divide(objList[index], count);
      }
    }

    public static short FindAverageAbs(List<List<short>> m)
    {
      int num1 = 0;
      int num2 = 0;
      foreach (List<short> shortList in m)
      {
        foreach (short num3 in shortList)
        {
          num1 += (int) Math.Abs(num3);
          ++num2;
        }
      }
      return (short) ((double) num1 / (double) num2);
    }

    public static double FindAverage<T1, T2>(List<List<T1>> m, Func<T1, T2, T2> add, Func<T2, int, double> div, T2 zero)
    {
      T2 obj1 = zero;
      int num = 0;
      foreach (List<T1> objList in m)
      {
        foreach (T1 obj2 in objList)
        {
          obj1 = add(obj2, obj1);
          ++num;
        }
      }
      return div(obj1, num);
    }

    public static short FindAverage(List<List<short>> m)
    {
      return (short) XMatrix.FindAverage<short, float>(m, (Func<short, float, float>) ((a, b) => (float) a + b), (Func<float, int, double>) ((a, b) => (double) a / (double) b), 0.0f);
    }

    public static short FindAverage(List<List<int>> m)
    {
      return (short) XMatrix.FindAverage<int, float>(m, (Func<int, float, float>) ((a, b) => (float) a + b), (Func<float, int, double>) ((a, b) => (double) a / (double) b), 0.0f);
    }

    public static double FindAverage(List<List<double>> m)
    {
      return XMatrix.FindAverage<double, double>(m, (Func<double, double, double>) ((a, b) => a + b), (Func<double, int, double>) ((a, b) => a / (double) b), 0.0);
    }

    public static void Percentage(List<List<int>> matrixA, List<List<int>> matrixB, List<List<double>> percentageMatrix)
    {
      XMatrix.Percentage<int, double>(matrixA, matrixB, percentageMatrix, new Func<int, int, double>(XMath.GetPercentage));
    }

    private static void Percentage<T1, T2>(List<List<T1>> matrixA, List<List<T1>> matrixB, List<List<T2>> percentageMatrix, Func<T1, T1, T2> getPercentage)
    {
      for (int index1 = 0; index1 < matrixA.Count; ++index1)
      {
        List<T1> objList1 = matrixA[index1];
        List<T1> objList2 = matrixB[index1];
        List<T2> objList3 = percentageMatrix[index1];
        for (int index2 = 0; index2 < objList1.Count; ++index2)
        {
          T1 obj1 = objList2[index2];
          T1 obj2 = objList1[index2];
          objList3[index2] = getPercentage(obj2, obj1);
        }
      }
    }

    public static List<List<short>> ConvertSenseMatrixToNormal(List<List<short>> data)
    {
      if (XList.IsNullOrEmpty<List<short>>((ICollection<List<short>>) data))
        return (List<List<short>>) null;
      int count1 = data[0].Count;
      int count2 = data.Count;
      List<List<short>> m = new List<List<short>>();
      XMatrix.CheckSize<short>(m, count1, count2, (short) 0);
      int num1 = count1 / 2;
      int num2 = count1 - num1;
      List<short> fromReversedMatrix = XMatrix.GetShortArrayFromReversedMatrix(data);
      int index1 = 0;
      for (int index2 = 0; index2 < count2; ++index2)
      {
        int index3 = 1;
        List<short> shortList = m[index2];
        for (int index4 = 0; index4 < num1; ++index4)
        {
          shortList[index3] = (short) ((int) fromReversedMatrix[index1] * -1);
          ++index1;
          index3 += 2;
        }
      }
      for (int index2 = 0; index2 < count2; ++index2)
      {
        int index3 = 0;
        List<short> shortList = m[index2];
        for (int index4 = 0; index4 < num2; ++index4)
        {
          shortList[index3] = (short) ((int) fromReversedMatrix[index1] * -1);
          ++index1;
          index3 += 2;
        }
      }
      XMatrix.ReversedMatrixColumns(m);
      return m;
    }

    public static void FillSenseMatrixFromByteArray(byte[] image, List<List<short>> matrix, int colCount, int rowCount)
    {
      List<short> shortList1 = new List<short>();
      int startIndex = 0;
      while (startIndex < image.Length)
      {
        shortList1.Add(BitConverter.ToInt16(image, startIndex));
        startIndex += 2;
      }
      int num1 = colCount / 2;
      int num2 = colCount - num1;
      int index1 = 0;
      for (int index2 = 0; index2 < rowCount; ++index2)
      {
        int index3 = 1;
        List<short> shortList2 = matrix[index2];
        for (int index4 = 0; index4 < num1; ++index4)
        {
          shortList2[index3] = (short) ((int) shortList1[index1] * -1);
          ++index1;
          index3 += 2;
        }
      }
      for (int index2 = 0; index2 < rowCount; ++index2)
      {
        int index3 = 0;
        List<short> shortList2 = matrix[index2];
        for (int index4 = 0; index4 < num2; ++index4)
        {
          shortList2[index3] = (short) ((int) shortList1[index1] * -1);
          ++index1;
          index3 += 2;
        }
      }
    }

    public static List<List<short>> BuildDriveMatrix(List<List<short>> matrixA, List<int> sequence)
    {
      List<List<short>> shortListList = XMatrix.CopyZero<short>(matrixA);
      for (int index1 = 0; index1 < sequence.Count; ++index1)
      {
        if (sequence[index1] != (int) byte.MaxValue)
        {
          int index2 = sequence[index1];
          int index3 = index1;
          List<short> shortList1 = matrixA[index3];
          List<short> shortList2 = shortListList[index2];
          for (int index4 = 0; index4 < shortList1.Count; ++index4)
            shortList2[index4] = shortList1[index4];
        }
      }
      return shortListList;
    }

    public static void FillReversedMatrixFromByteArray(byte[] image, List<List<short>> m, int colCount, int rowCount)
    {
      int startIndex = 0;
      for (int index1 = 0; index1 < rowCount; ++index1)
      {
        List<short> shortList = m[index1];
        for (int index2 = colCount - 1; index2 >= 0; --index2)
        {
          short int16 = BitConverter.ToInt16(image, startIndex);
          shortList[index2] = int16;
          startIndex += 2;
        }
      }
    }

    public static void FillMatrixFromByteArray(byte[] image, List<List<short>> m, int colCount, int rowCount)
    {
      int startIndex = 0;
      for (int index1 = 0; index1 < rowCount; ++index1)
      {
        List<short> shortList = m[index1];
        for (int index2 = 0; index2 < colCount; ++index2)
        {
          short int16 = BitConverter.ToInt16(image, startIndex);
          shortList[index2] = int16;
          startIndex += 2;
        }
      }
    }

    public static List<short> GetShortArrayFromReversedMatrix(List<List<short>> m)
    {
      List<short> shortList1 = new List<short>();
      int count = m.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        List<short> shortList2 = m[index1];
        for (int index2 = shortList2.Count - 1; index2 >= 0; --index2)
          shortList1.Add(shortList2[index2]);
      }
      return shortList1;
    }

    public static void ReversedMatrixColumns(List<List<short>> m)
    {
      foreach (List<short> shortList in m)
        shortList.Reverse();
    }

    public static List<List<double>> CreateStandardDeviationMatrix(List<List<double>> averageMatrix, List<List<List<short>>> sampleMatrixes)
    {
      List<List<double>> doubleListList = XMatrix.Copy<double, double>(averageMatrix, 0.0);
      foreach (List<List<short>> sampleMatrix in sampleMatrixes)
      {
        int num1 = Math.Min(sampleMatrix.Count, averageMatrix.Count);
        for (int index1 = 0; index1 < num1; ++index1)
        {
          List<double> doubleList1 = doubleListList[index1];
          List<double> doubleList2 = averageMatrix[index1];
          List<short> shortList = sampleMatrix[index1];
          int val2 = Math.Min(doubleList1.Count, averageMatrix.Count);
          int num2 = Math.Min(shortList.Count, val2);
          for (int index2 = 0; index2 < num2; ++index2)
          {
            double num3 = doubleList2[index2];
            double num4 = (double) shortList[index2] - num3;
            double num5 = num4 * num4;
            List<double> doubleList3;
            int index3;
            (doubleList3 = doubleList1)[index3 = index2] = doubleList3[index3] + num5;
          }
        }
      }
      int num = sampleMatrixes.Count - 1;
      foreach (List<double> doubleList in doubleListList)
      {
        int count = doubleList.Count;
        for (int index = 0; index < count; ++index)
        {
          double d = doubleList[index] / (double) num;
          doubleList[index] = Math.Sqrt(d);
        }
      }
      return doubleListList;
    }

    public static void SaveMatrixToCsvFile(List<List<short>> matrix, string filename)
    {
      XMatrix.SaveMatrixToCsvFile<short>(matrix, filename, (GetNameByIndexDelegate) (c => c.ToString()));
    }

    public static void SaveMatrixToCsvFile(List<List<short>> matrix, TextWriter sw)
    {
      XMatrix.SaveMatrixToCsvFile<short>(matrix, sw, (GetNameByIndexDelegate) (c => c.ToString()));
    }

    public static void SaveMatrixToCsvFile<T>(List<string> columNames, List<List<T>> matrix, string filename)
    {
      XMatrix.SaveMatrixToCsvFile<T>(matrix, filename, (GetNameByIndexDelegate) (c => columNames[c]));
    }

    public static void SaveMatrixToCsvFile<T>(List<List<T>> matrix, string filename, GetNameByIndexDelegate getNameByIndex)
    {
      using (StreamWriter streamWriter = new StreamWriter(filename))
        XMatrix.SaveMatrixToCsvFile<T>(matrix, (TextWriter) streamWriter, getNameByIndex);
    }

    public static void SaveMatrixToCsvFile<T>(List<List<T>> matrix, TextWriter sw, GetNameByIndexDelegate getNameByIndex)
    {
      if (matrix == null || matrix.Count == 0)
        return;
      string newLine = Environment.NewLine;
      for (int c = 0; c < matrix[0].Count; ++c)
        sw.Write(getNameByIndex(c) + ",");
      sw.Write(newLine);
      foreach (List<T> objList in matrix)
      {
        foreach (T obj in objList)
        {
          string str = obj.ToString() + ",";
          sw.Write(str);
        }
        sw.Write(newLine);
      }
    }

    public static string ConvertMatrixToCsvString<T>(List<List<T>> matrix, bool isMatrix = false)
    {
      if (XList.IsNullOrEmpty<List<T>>((ICollection<List<T>>) matrix))
        return "";
      int count1 = matrix[0].Count;
      int count2 = matrix.Count;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index1 = 0; index1 < count2; ++index1)
      {
        for (int index2 = 0; index2 < count1; ++index2)
        {
          stringBuilder.Append((object) matrix[index1][index2]);
          if (index2 != count1 - 1 || index1 != count2 - 1)
            stringBuilder.Append(", ");
        }
        if (isMatrix)
          stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }

    public static bool FindRowAndColCount<T>(List<List<T>> matrix, out int rowCount, out int colCount)
    {
      rowCount = 0;
      colCount = 0;
      if (XList.IsNullOrEmpty<List<T>>((ICollection<List<T>>) matrix))
        return false;
      rowCount = matrix.Count;
      List<T> objList = matrix[0];
      if (XList.IsNullOrEmpty<T>((ICollection<T>) objList))
        return false;
      colCount = objList.Count;
      return true;
    }

    public static bool FindRowAndColCount<T>(List<List<List<T>>> matrixes, out int rowCount, out int colCount)
    {
      rowCount = 0;
      colCount = 0;
      int rowCount1;
      int colCount1;
      if (!XMatrix.FindRowAndColCount<T>(matrixes[0], out rowCount1, out colCount1))
        return false;
      foreach (List<List<T>> matrix in matrixes)
      {
        if (!XMatrix.FindRowAndColCount<T>(matrix, out rowCount, out colCount) || rowCount1 != rowCount || colCount1 != colCount)
          return false;
      }
      return true;
    }

    public static int GetSize<T>(List<List<T>> matrix)
    {
      int rowCount;
      int colCount;
      if (!XMatrix.FindRowAndColCount<T>(matrix, out rowCount, out colCount))
        return 0;
      return rowCount * colCount;
    }

    public static T FindValueAndLocation<T>(List<List<T>> a, T minValue, Func<T, T, bool> compare, out int maxRowIndex, out int maxColIndex)
    {
      T obj1 = minValue;
      maxColIndex = -1;
      maxRowIndex = -1;
      for (int index1 = 0; index1 < a.Count; ++index1)
      {
        List<T> objList = a[index1];
        for (int index2 = 0; index2 < objList.Count; ++index2)
        {
          T obj2 = objList[index2];
          if (compare(obj2, obj1))
          {
            obj1 = obj2;
            maxRowIndex = index1;
            maxColIndex = index2;
          }
        }
      }
      return obj1;
    }

    public static T FindValue<T>(List<List<T>> a, T minValue, Func<T, T, bool> compare)
    {
      int maxRowIndex;
      int maxColIndex;
      return XMatrix.FindValueAndLocation<T>(a, minValue, compare, out maxRowIndex, out maxColIndex);
    }

    public static double FindMaxValue(List<List<double>> m)
    {
      int maxRowIndex;
      int maxColIndex;
      return XMatrix.FindValueAndLocation<double>(m, double.MinValue, (Func<double, double, bool>) ((a, b) => a > b), out maxRowIndex, out maxColIndex);
    }

    public static double FindMaxValue(List<List<short>> m)
    {
      int maxRowIndex;
      int maxColIndex;
      return (double) XMatrix.FindValueAndLocation<short>(m, short.MinValue, (Func<short, short, bool>) ((a, b) => (int) a > (int) b), out maxRowIndex, out maxColIndex);
    }

    public static void FindSignal<T>(List<List<List<T>>> matrixes, T minValue, Func<T, T, bool> more, out int maxRowIndex, out int maxColIndex)
    {
      maxRowIndex = -1;
      maxColIndex = -1;
      try
      {
        if (XList.IsNullOrEmpty<List<List<T>>>((ICollection<List<List<T>>>) matrixes))
          return;
        List<List<int>> intListList = XMatrix.Copy<int, T>(matrixes[0], 0);
        int num1 = 0;
        foreach (List<List<T>> matrix in matrixes)
        {
          int maxRowIndex1;
          int maxColIndex1;
          XMatrix.FindValueAndLocation<T>(matrix, minValue, more, out maxRowIndex1, out maxColIndex1);
          int num2 = intListList[maxRowIndex1][maxColIndex1] + 1;
          intListList[maxRowIndex1][maxColIndex1] = num2;
          if (num2 > num1)
          {
            num1 = num2;
            maxRowIndex = maxRowIndex1;
            maxColIndex = maxColIndex1;
          }
        }
      }
      catch (Exception ex)
      {
        XMatrix.Logger.Error((object) ex);
      }
    }

    private static List<List<T>> MakeMatrix<T>(IList<List<List<T>>> matrixes, Func<T, T, bool> compare)
    {
      if (XList.IsNullOrEmpty<List<List<T>>>((ICollection<List<List<T>>>) matrixes))
        return (List<List<T>>) null;
      List<List<T>> objListList = XMatrix.Copy<T, T>(matrixes[0], default (T));
      foreach (List<List<T>> matrix in (IEnumerable<List<List<T>>>) matrixes)
      {
        int count1 = matrixes.Count;
        for (int index1 = 0; index1 < count1; ++index1)
        {
          List<T> objList1 = matrix[index1];
          List<T> objList2 = objListList[index1];
          int count2 = objList1.Count;
          for (int index2 = 0; index2 < count2; ++index2)
          {
            if (compare(objList1[index2], objList2[index2]))
              objList2[index2] = objList1[index2];
          }
        }
      }
      return objListList;
    }

    public static List<List<T>> CreateDeltaMatrix<T>(IList<List<List<T>>> matrixes, T minValue, T maxValue, Func<T, T, bool> compare, Func<T, T, T> sub)
    {
      if (XList.IsNullOrEmpty<List<List<T>>>((ICollection<List<List<T>>>) matrixes))
        return (List<List<T>>) null;
      List<List<T>> objListList1 = XMatrix.Copy<T, T>(matrixes[0], minValue);
      List<List<T>> objListList2 = XMatrix.Copy<T, T>(matrixes[0], maxValue);
      int val2_1 = Math.Min(objListList1.Count, objListList1.Count);
      foreach (List<List<T>> matrix in (IEnumerable<List<List<T>>>) matrixes)
      {
        int num1 = Math.Min(matrix.Count, val2_1);
        for (int index1 = 0; index1 < num1; ++index1)
        {
          List<T> objList1 = matrix[index1];
          List<T> objList2 = objListList1[index1];
          List<T> objList3 = objListList2[index1];
          int val2_2 = Math.Min(objList3.Count, objList2.Count);
          int num2 = Math.Min(objList1.Count, val2_2);
          for (int index2 = 0; index2 < num2; ++index2)
          {
            if (compare(objList1[index2], objList2[index2]))
              objList2[index2] = objList1[index2];
            if (!compare(objList1[index2], objList3[index2]))
              objList3[index2] = objList1[index2];
          }
        }
      }
      List<List<T>> objListList3 = XMatrix.Copy<T, T>(matrixes[0], default (T));
      int num3 = Math.Min(objListList3.Count, val2_1);
      for (int index1 = 0; index1 < num3; ++index1)
      {
        List<T> objList1 = objListList1[index1];
        List<T> objList2 = objListList2[index1];
        List<T> objList3 = objListList3[index1];
        int val2_2 = Math.Min(objList2.Count, objList1.Count);
        int num1 = Math.Min(objList3.Count, val2_2);
        for (int index2 = 0; index2 < num1; ++index2)
          objList3[index2] = sub(objList1[index2], objList2[index2]);
      }
      return objListList3;
    }

    public static List<List<short>> CreateDeltaMatrix(IList<List<List<short>>> matrixes)
    {
      return XMatrix.CreateDeltaMatrix<short>(matrixes, short.MinValue, short.MaxValue, (Func<short, short, bool>) ((a, b) => (int) a > (int) b), (Func<short, short, short>) ((a, b) => (short) ((int) a - (int) b)));
    }
  }
}
