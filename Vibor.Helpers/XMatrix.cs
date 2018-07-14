using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vibor.Logging;

namespace Vibor.Helpers
{
    public class XMatrix
    {
        private static readonly ILog Logger = LogManager.GetLogger("StartUpFlowViewModel");

        public static void CheckSize<T>(List<List<T>> m, int colCount, int rowCount, T zero = default(T))
        {
            if (IsSameDimensions(m, colCount, rowCount))
                return;
            Resize(m, colCount, rowCount, default(T));
        }

        public static void Resize<T>(List<List<T>> m, int colCount, int rowCount, T zero = default(T))
        {
            Clear(m);
            for (var index1 = 0; index1 < rowCount; ++index1)
            {
                var objList = new List<T>();
                for (var index2 = 0; index2 < colCount; ++index2)
                    objList.Add(zero);
                m.Add(objList);
            }
        }

        public static void Init<T>(List<List<T>> matrixA, List<List<T>> matrixB, T zero = default(T))
        {
            matrixA.AddRange(matrixB.Select(rowB => rowB.Select(value => zero).ToList()));
        }

        public static List<List<T>> CopyZero<T>(List<List<T>> a)
        {
            var matrixA = new List<List<T>>();
            Init(matrixA, a, default(T));
            return matrixA;
        }

        public static List<List<T>> Copy<T>(List<List<T>> matrixA)
        {
            var objListList = new List<List<T>>();
            foreach (var objList1 in matrixA)
            {
                var objList2 = new List<T>();
                objList2.AddRange(objList1);
                objListList.Add(objList2);
            }

            return objListList;
        }

        public static List<List<T>> CopyListToMatrix<T>(List<T> list, List<List<T>> templateMatrix)
        {
            if (XList.IsNullOrEmpty(list))
                return null;
            var objListList = new List<List<T>>();
            var num = 0;
            foreach (var objList1 in templateMatrix)
            {
                var objList2 = new List<T>();
                foreach (var obj1 in objList1)
                {
                    if (num >= list.Count)
                        return null;
                    var obj2 = list[num++];
                    objList2.Add(obj2);
                }

                objListList.Add(objList2);
            }

            return objListList;
        }

        public static List<List<T>> Copy<T>(List<T> list, int colCount, int rowCount)
        {
            if (XList.IsNullOrEmpty(list) || colCount * rowCount != list.Count)
                return null;
            var objListList = new List<List<T>>();
            var num = 0;
            for (var index1 = 0; index1 < rowCount; ++index1)
            {
                var objList = new List<T>();
                for (var index2 = 0; index2 < colCount; ++index2)
                {
                    var obj = list[num++];
                    objList.Add(obj);
                }

                objListList.Add(objList);
            }

            return objListList;
        }

        public static List<List<T1>> Copy<T1, T2>(List<List<T2>> matrix, T1 value)
        {
            return matrix.Select(rowA => rowA.Select(v => value).ToList()).ToList();
        }

        public static List<List<T2>> MakeCopy<T1, T2>(List<List<T1>> m, Func<T1, T2> cast)
        {
            return m.Select(r => r.Select(cast).ToList()).ToList();
        }

        public static List<List<short>> MakeCopy(List<List<int>> m)
        {
            return MakeCopy(m, a => (short) a);
        }

        public static List<List<short>> MakeCopy(List<List<float>> m)
        {
            return MakeCopy(m, a => (short) a);
        }

        public static void Reset<T>(List<List<T>> m, T t)
        {
            for (var index1 = 0; index1 < m.Count; ++index1)
            {
                var objList = m[index1];
                for (var index2 = 0; index2 < objList.Count; ++index2)
                    objList[index2] = t;
            }
        }

        public static bool IsSameDimensions<T1, T2>(List<List<T1>> a, List<List<T2>> b)
        {
            return IsSameDimensions(a, b.Count > 0 ? b[0].Count : 0, b.Count);
        }

        public static bool IsSameDimensions<T>(List<List<T>> m, int colCount, int rowCount)
        {
            return m != null && m.Count == rowCount && m.Count >= 1 && m[0].Count == colCount;
        }

        public static bool IsEqual<T>(List<List<T>> a, List<List<T>> b, T delta, Func<T, T, T, bool> equals)
            where T : IComparable
        {
            if (!IsSameDimensions(a, b))
                return false;
            for (var index1 = 0; index1 < a.Count; ++index1)
            {
                var objList1 = a[index1];
                var objList2 = b[index1];
                for (var index2 = 0; index2 < objList1.Count; ++index2)
                    if (!equals(objList1[index2], objList2[index2], delta))
                        return false;
            }

            return true;
        }

        public static bool IsEqual<T>(List<List<T>> matrxiA, List<List<T>> matrixB) where T : IComparable
        {
            return IsEqual(matrxiA, matrixB, default(T), (a, b, d) => a.Equals(b));
        }

        public static bool IsEqual(List<List<double>> matrixA, List<List<double>> matrixB, double delta)
        {
            return IsEqual(matrixA, matrixB, delta, IsEqual);
        }

        public static bool IsEqual(double a, double b, double delta)
        {
            return Math.Abs(a - b) <= delta;
        }

        public static void Clear<T>(List<List<T>> m)
        {
            if (XList.IsNullOrEmpty(m))
                return;
            foreach (var objList in m)
                objList.Clear();
            m.Clear();
        }

        public static void SetValues<T>(List<List<T>> a, List<List<T>> b)
        {
            if (XList.IsNullOrEmpty(a))
                return;
            for (var index1 = 0; index1 < a.Count; ++index1)
            {
                var objList1 = a[index1];
                var objList2 = b[index1];
                for (var index2 = 0; index2 < objList1.Count; ++index2)
                    objList1[index2] = objList2[index2];
            }
        }

        public static string GetText<T>(List<List<T>> m, string formatGood, string formatBad, Func<T, bool> isGood)
        {
            return GetText(m, a => isGood(a) ? formatGood : formatBad, ',', true);
        }

        public static string GetText<T>(List<List<T>> m, string format = "{0,5}", char separator = ',',
            bool bAppendLine = true)
        {
            return GetText(m, a => format, separator, bAppendLine);
        }

        public static string GetText<T>(List<List<T>> m, Func<T, string> format, char separator = ',',
            bool bAppendLine = true)
        {
            if (XList.IsNullOrEmpty(m))
                return "";
            var separator1 = separator.ToString();
            var sb = new StringBuilder();
            foreach (var list in m)
            {
                XList2.AddSeparatedText(sb, list, format, separator1);
                if (bAppendLine)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        public static void PopulateMatrix<T>(List<List<T>> matrix, string text, Func<string, T> convert,
            char separator = ',')
        {
            if (string.IsNullOrEmpty(text))
                return;
            using (var stringReader = new StringReader(text))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    var objList = XList2.PopulateArray(line, convert, separator);
                    if (!XList.IsNullOrEmpty(objList))
                        matrix.Add(objList);
                }
            }
        }

        public static List<List<T>> GetMatrix<T>(string text, Func<string, T> convert, char separator = ',')
        {
            var matrix = new List<List<T>>();
            PopulateMatrix(matrix, text, convert, separator);
            return matrix;
        }

        public static List<List<int>> GetIntMatrix(string text, char separator = ',')
        {
            return GetMatrix(text, XConverter.ConvertToInt, separator);
        }

        public static List<List<double>> GetDoubleMatrix(string text, char separator = ',')
        {
            return GetMatrix(text, XConverter.ConvertToDouble, separator);
        }

        public static List<List<int>> GetMatrixHex(string text)
        {
            var convert = (Func<string, int>) (s =>
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
            return GetMatrix(text, convert, ',');
        }

        public static void AddMatrix<T>(List<List<T>> a, List<List<T>> b, Func<T, T, T> add)
        {
            AddMatrix<T, T>(a, b, add);
        }

        public static void AddMatrix<T1, T2>(List<List<T1>> a, List<List<T2>> b, Func<T1, T2, T1> add)
        {
            try
            {
                if (a.Count != b.Count)
                    Logger.Warn("Row Counts are not equal");
                var num1 = Math.Min(a.Count, b.Count);
                for (var index1 = 0; index1 < num1; ++index1)
                {
                    var objList1 = a[index1];
                    var objList2 = b[index1];
                    if (objList1.Count != objList2.Count)
                        Logger.Warn("Column Counts are not equal");
                    var num2 = Math.Min(objList1.Count, objList2.Count);
                    for (var index2 = 0; index2 < num2; ++index2)
                        objList1[index2] = add(objList1[index2], objList2[index2]);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static List<List<double>> CreateAverageMatrix(List<List<List<short>>> matrixSamples)
        {
            var averageMatrix = Copy(matrixSamples[0], 0.0);
            Avearge(averageMatrix, matrixSamples);
            return averageMatrix;
        }

        public static void Avearge(List<List<double>> averageMatrix, ICollection<List<List<short>>> matrixSamples)
        {
            Avearge(averageMatrix, matrixSamples, (a, b) => a + (double) b, (a, b) => a / (double) b, 0.0);
        }

        public static void Avearge(List<List<int>> averageMatrix, ICollection<List<List<short>>> matrixSamples)
        {
            Avearge(averageMatrix, matrixSamples, (a, b) => a + (int) b, (a, b) => a / b, 0);
        }

        public static void Avearge<T1, T2>(List<List<T1>> averageMatrix, ICollection<List<List<T2>>> matrixSamples,
            Func<T1, T2, T1> add, Func<T1, int, T1> divide, T1 zero = default(T1))
        {
            Reset(averageMatrix, zero);
            foreach (var matrixSample in matrixSamples)
                AddMatrix(averageMatrix, matrixSample, add);
            var count = matrixSamples.Count;
            foreach (var objList in averageMatrix)
                for (var index = 0; index < objList.Count; ++index)
                    objList[index] = divide(objList[index], count);
        }

        public static short FindAverageAbs(List<List<short>> m)
        {
            var num1 = 0;
            var num2 = 0;
            foreach (var shortList in m)
            foreach (var num3 in shortList)
            {
                num1 += Math.Abs(num3);
                ++num2;
            }

            return (short) (num1 / (double) num2);
        }

        public static double FindAverage<T1, T2>(List<List<T1>> m, Func<T1, T2, T2> add, Func<T2, int, double> div,
            T2 zero)
        {
            var obj1 = zero;
            var num = 0;
            foreach (var objList in m)
            foreach (var obj2 in objList)
            {
                obj1 = add(obj2, obj1);
                ++num;
            }

            return div(obj1, num);
        }

        public static short FindAverage(List<List<short>> m)
        {
            return (short) FindAverage(m, (a, b) => (float) a + b, (a, b) => (double) a / (double) b, 0.0f);
        }

        public static short FindAverage(List<List<int>> m)
        {
            return (short) FindAverage(m, (a, b) => (float) a + b, (a, b) => (double) a / (double) b, 0.0f);
        }

        public static double FindAverage(List<List<double>> m)
        {
            return FindAverage(m, (a, b) => a + b, (a, b) => a / (double) b, 0.0);
        }

        public static void Percentage(List<List<int>> matrixA, List<List<int>> matrixB,
            List<List<double>> percentageMatrix)
        {
            Percentage(matrixA, matrixB, percentageMatrix, XMath2.GetPercentage);
        }

        private static void Percentage<T1, T2>(List<List<T1>> matrixA, List<List<T1>> matrixB,
            List<List<T2>> percentageMatrix, Func<T1, T1, T2> getPercentage)
        {
            for (var index1 = 0; index1 < matrixA.Count; ++index1)
            {
                var objList1 = matrixA[index1];
                var objList2 = matrixB[index1];
                var objList3 = percentageMatrix[index1];
                for (var index2 = 0; index2 < objList1.Count; ++index2)
                {
                    var obj1 = objList2[index2];
                    var obj2 = objList1[index2];
                    objList3[index2] = getPercentage(obj2, obj1);
                }
            }
        }

        public static List<List<short>> ConvertSenseMatrixToNormal(List<List<short>> data)
        {
            if (XList.IsNullOrEmpty(data))
                return null;
            var count1 = data[0].Count;
            var count2 = data.Count;
            var m = new List<List<short>>();
            CheckSize(m, count1, count2, (short) 0);
            var num1 = count1 / 2;
            var num2 = count1 - num1;
            var fromReversedMatrix = GetShortArrayFromReversedMatrix(data);
            var index1 = 0;
            for (var index2 = 0; index2 < count2; ++index2)
            {
                var index3 = 1;
                var shortList = m[index2];
                for (var index4 = 0; index4 < num1; ++index4)
                {
                    shortList[index3] = (short) (fromReversedMatrix[index1] * -1);
                    ++index1;
                    index3 += 2;
                }
            }

            for (var index2 = 0; index2 < count2; ++index2)
            {
                var index3 = 0;
                var shortList = m[index2];
                for (var index4 = 0; index4 < num2; ++index4)
                {
                    shortList[index3] = (short) (fromReversedMatrix[index1] * -1);
                    ++index1;
                    index3 += 2;
                }
            }

            ReversedMatrixColumns(m);
            return m;
        }

        public static void FillSenseMatrixFromByteArray(byte[] image, List<List<short>> matrix, int colCount,
            int rowCount)
        {
            var shortList1 = new List<short>();
            var startIndex = 0;
            while (startIndex < image.Length)
            {
                shortList1.Add(BitConverter.ToInt16(image, startIndex));
                startIndex += 2;
            }

            var num1 = colCount / 2;
            var num2 = colCount - num1;
            var index1 = 0;
            for (var index2 = 0; index2 < rowCount; ++index2)
            {
                var index3 = 1;
                var shortList2 = matrix[index2];
                for (var index4 = 0; index4 < num1; ++index4)
                {
                    shortList2[index3] = (short) (shortList1[index1] * -1);
                    ++index1;
                    index3 += 2;
                }
            }

            for (var index2 = 0; index2 < rowCount; ++index2)
            {
                var index3 = 0;
                var shortList2 = matrix[index2];
                for (var index4 = 0; index4 < num2; ++index4)
                {
                    shortList2[index3] = (short) (shortList1[index1] * -1);
                    ++index1;
                    index3 += 2;
                }
            }
        }

        public static List<List<short>> BuildDriveMatrix(List<List<short>> matrixA, List<int> sequence)
        {
            var shortListList = CopyZero(matrixA);
            for (var index1 = 0; index1 < sequence.Count; ++index1)
                if (sequence[index1] != byte.MaxValue)
                {
                    var index2 = sequence[index1];
                    var index3 = index1;
                    var shortList1 = matrixA[index3];
                    var shortList2 = shortListList[index2];
                    for (var index4 = 0; index4 < shortList1.Count; ++index4)
                        shortList2[index4] = shortList1[index4];
                }

            return shortListList;
        }

        public static void FillReversedMatrixFromByteArray(byte[] image, List<List<short>> m, int colCount,
            int rowCount)
        {
            var startIndex = 0;
            for (var index1 = 0; index1 < rowCount; ++index1)
            {
                var shortList = m[index1];
                for (var index2 = colCount - 1; index2 >= 0; --index2)
                {
                    var int16 = BitConverter.ToInt16(image, startIndex);
                    shortList[index2] = int16;
                    startIndex += 2;
                }
            }
        }

        public static void FillMatrixFromByteArray(byte[] image, List<List<short>> m, int colCount, int rowCount)
        {
            var startIndex = 0;
            for (var index1 = 0; index1 < rowCount; ++index1)
            {
                var shortList = m[index1];
                for (var index2 = 0; index2 < colCount; ++index2)
                {
                    var int16 = BitConverter.ToInt16(image, startIndex);
                    shortList[index2] = int16;
                    startIndex += 2;
                }
            }
        }

        public static List<short> GetShortArrayFromReversedMatrix(List<List<short>> m)
        {
            var shortList1 = new List<short>();
            var count = m.Count;
            for (var index1 = 0; index1 < count; ++index1)
            {
                var shortList2 = m[index1];
                for (var index2 = shortList2.Count - 1; index2 >= 0; --index2)
                    shortList1.Add(shortList2[index2]);
            }

            return shortList1;
        }

        public static void ReversedMatrixColumns(List<List<short>> m)
        {
            foreach (var shortList in m)
                shortList.Reverse();
        }

        public static List<List<double>> CreateStandardDeviationMatrix(List<List<double>> averageMatrix,
            List<List<List<short>>> sampleMatrixes)
        {
            var doubleListList = Copy(averageMatrix, 0.0);
            foreach (var sampleMatrix in sampleMatrixes)
            {
                var num1 = Math.Min(sampleMatrix.Count, averageMatrix.Count);
                for (var index1 = 0; index1 < num1; ++index1)
                {
                    var doubleList1 = doubleListList[index1];
                    var doubleList2 = averageMatrix[index1];
                    var shortList = sampleMatrix[index1];
                    var val2 = Math.Min(doubleList1.Count, averageMatrix.Count);
                    var num2 = Math.Min(shortList.Count, val2);
                    for (var index2 = 0; index2 < num2; ++index2)
                    {
                        var num3 = doubleList2[index2];
                        var num4 = shortList[index2] - num3;
                        var num5 = num4 * num4;
                        List<double> doubleList3;
                        int index3;
                        (doubleList3 = doubleList1)[index3 = index2] = doubleList3[index3] + num5;
                    }
                }
            }

            var num = sampleMatrixes.Count - 1;
            foreach (var doubleList in doubleListList)
            {
                var count = doubleList.Count;
                for (var index = 0; index < count; ++index)
                {
                    var d = doubleList[index] / num;
                    doubleList[index] = Math.Sqrt(d);
                }
            }

            return doubleListList;
        }

        public static void SaveMatrixToCsvFile(List<List<short>> matrix, string filename)
        {
            SaveMatrixToCsvFile(matrix, filename, c => c.ToString());
        }

        public static void SaveMatrixToCsvFile(List<List<short>> matrix, TextWriter sw)
        {
            SaveMatrixToCsvFile(matrix, sw, c => c.ToString());
        }

        public static void SaveMatrixToCsvFile<T>(List<string> columNames, List<List<T>> matrix, string filename)
        {
            SaveMatrixToCsvFile(matrix, filename, c => columNames[c]);
        }

        public static void SaveMatrixToCsvFile<T>(List<List<T>> matrix, string filename,
            GetNameByIndexDelegate getNameByIndex)
        {
            using (var streamWriter = new StreamWriter(filename))
            {
                SaveMatrixToCsvFile(matrix, streamWriter, getNameByIndex);
            }
        }

        public static void SaveMatrixToCsvFile<T>(List<List<T>> matrix, TextWriter sw,
            GetNameByIndexDelegate getNameByIndex)
        {
            if (matrix == null || matrix.Count == 0)
                return;
            var newLine = Environment.NewLine;
            for (var c = 0; c < matrix[0].Count; ++c)
                sw.Write(getNameByIndex(c) + ",");
            sw.Write(newLine);
            foreach (var objList in matrix)
            {
                foreach (var obj in objList)
                {
                    var str = obj + ",";
                    sw.Write(str);
                }

                sw.Write(newLine);
            }
        }

        public static string ConvertMatrixToCsvString<T>(List<List<T>> matrix, bool isMatrix = false)
        {
            if (XList.IsNullOrEmpty(matrix))
                return "";
            var count1 = matrix[0].Count;
            var count2 = matrix.Count;
            var stringBuilder = new StringBuilder();
            for (var index1 = 0; index1 < count2; ++index1)
            {
                for (var index2 = 0; index2 < count1; ++index2)
                {
                    stringBuilder.Append(matrix[index1][index2]);
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
            if (XList.IsNullOrEmpty(matrix))
                return false;
            rowCount = matrix.Count;
            var objList = matrix[0];
            if (XList.IsNullOrEmpty(objList))
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
            if (!FindRowAndColCount(matrixes[0], out rowCount1, out colCount1))
                return false;
            foreach (var matrix in matrixes)
                if (!FindRowAndColCount(matrix, out rowCount, out colCount) || rowCount1 != rowCount ||
                    colCount1 != colCount)
                    return false;
            return true;
        }

        public static int GetSize<T>(List<List<T>> matrix)
        {
            int rowCount;
            int colCount;
            if (!FindRowAndColCount(matrix, out rowCount, out colCount))
                return 0;
            return rowCount * colCount;
        }

        public static T FindValueAndLocation<T>(List<List<T>> a, T minValue, Func<T, T, bool> compare,
            out int maxRowIndex, out int maxColIndex)
        {
            var obj1 = minValue;
            maxColIndex = -1;
            maxRowIndex = -1;
            for (var index1 = 0; index1 < a.Count; ++index1)
            {
                var objList = a[index1];
                for (var index2 = 0; index2 < objList.Count; ++index2)
                {
                    var obj2 = objList[index2];
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
            return FindValueAndLocation(a, minValue, compare, out maxRowIndex, out maxColIndex);
        }

        public static double FindMaxValue(List<List<double>> m)
        {
            int maxRowIndex;
            int maxColIndex;
            return FindValueAndLocation(m, double.MinValue, (a, b) => a > b, out maxRowIndex, out maxColIndex);
        }

        public static double FindMaxValue(List<List<short>> m)
        {
            int maxRowIndex;
            int maxColIndex;
            return FindValueAndLocation(m, short.MinValue, (a, b) => (int) a > (int) b, out maxRowIndex,
                out maxColIndex);
        }

        public static void FindSignal<T>(List<List<List<T>>> matrixes, T minValue, Func<T, T, bool> more,
            out int maxRowIndex, out int maxColIndex)
        {
            maxRowIndex = -1;
            maxColIndex = -1;
            try
            {
                if (XList.IsNullOrEmpty(matrixes))
                    return;
                var intListList = Copy(matrixes[0], 0);
                var num1 = 0;
                foreach (var matrix in matrixes)
                {
                    int maxRowIndex1;
                    int maxColIndex1;
                    FindValueAndLocation(matrix, minValue, more, out maxRowIndex1, out maxColIndex1);
                    var num2 = intListList[maxRowIndex1][maxColIndex1] + 1;
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
                Logger.Error(ex);
            }
        }

        private static List<List<T>> MakeMatrix<T>(IList<List<List<T>>> matrixes, Func<T, T, bool> compare)
        {
            if (XList.IsNullOrEmpty(matrixes))
                return null;
            var objListList = Copy(matrixes[0], default(T));
            foreach (var matrix in matrixes)
            {
                var count1 = matrixes.Count;
                for (var index1 = 0; index1 < count1; ++index1)
                {
                    var objList1 = matrix[index1];
                    var objList2 = objListList[index1];
                    var count2 = objList1.Count;
                    for (var index2 = 0; index2 < count2; ++index2)
                        if (compare(objList1[index2], objList2[index2]))
                            objList2[index2] = objList1[index2];
                }
            }

            return objListList;
        }

        public static List<List<T>> CreateDeltaMatrix<T>(IList<List<List<T>>> matrixes, T minValue, T maxValue,
            Func<T, T, bool> compare, Func<T, T, T> sub)
        {
            if (XList.IsNullOrEmpty(matrixes))
                return null;
            var objListList1 = Copy(matrixes[0], minValue);
            var objListList2 = Copy(matrixes[0], maxValue);
            var val2_1 = Math.Min(objListList1.Count, objListList1.Count);
            foreach (var matrix in matrixes)
            {
                var num1 = Math.Min(matrix.Count, val2_1);
                for (var index1 = 0; index1 < num1; ++index1)
                {
                    var objList1 = matrix[index1];
                    var objList2 = objListList1[index1];
                    var objList3 = objListList2[index1];
                    var val2_2 = Math.Min(objList3.Count, objList2.Count);
                    var num2 = Math.Min(objList1.Count, val2_2);
                    for (var index2 = 0; index2 < num2; ++index2)
                    {
                        if (compare(objList1[index2], objList2[index2]))
                            objList2[index2] = objList1[index2];
                        if (!compare(objList1[index2], objList3[index2]))
                            objList3[index2] = objList1[index2];
                    }
                }
            }

            var objListList3 = Copy(matrixes[0], default(T));
            var num3 = Math.Min(objListList3.Count, val2_1);
            for (var index1 = 0; index1 < num3; ++index1)
            {
                var objList1 = objListList1[index1];
                var objList2 = objListList2[index1];
                var objList3 = objListList3[index1];
                var val2_2 = Math.Min(objList2.Count, objList1.Count);
                var num1 = Math.Min(objList3.Count, val2_2);
                for (var index2 = 0; index2 < num1; ++index2)
                    objList3[index2] = sub(objList1[index2], objList2[index2]);
            }

            return objListList3;
        }

        public static List<List<short>> CreateDeltaMatrix(IList<List<List<short>>> matrixes)
        {
            return CreateDeltaMatrix(matrixes, short.MinValue, short.MaxValue, (a, b) => (int) a > (int) b,
                (a, b) => (short) ((int) a - (int) b));
        }
    }
}