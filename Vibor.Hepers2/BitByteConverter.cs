using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Vibor.Helpers
{
    internal class BitByteConverter
    {
        private static readonly ILogger Logger = LogManager.GetLogger<BitByteConverter>();

        public static List<byte> SpaceSeparatedTextHexWordsToBytes(string text, bool reverse = false)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var strArray = text.Split(' ');
            var bytes = new List<byte>();
            foreach (var textWord in strArray)
                try
                {
                    if (!(textWord == string.Empty)) AddHexTextWordAsArrayOfBytes(textWord, bytes, reverse);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }

            return bytes;
        }

        public static void AddHexTextWordAsArrayOfBytes(string textWord, List<byte> bytes, bool reverse = false)
        {
            byte byteLo;
            byte byteHi;
            HexTextWordToLoHiBytes(textWord, out byteLo, out byteHi);
            AddLoHiBytesToArrayOfBytes(byteLo, byteHi, bytes, reverse);
        }

        public static void SwapHiLowBytes(IList<byte> bytes)
        {
            var index = 0;
            while (index < bytes.Count - 1)
            {
                var num = bytes[index + 1];
                bytes[index + 1] = bytes[index];
                bytes[index] = num;
                index += 2;
            }
        }

        public static void HexTextWordToLoHiBytes(string textWord, out byte byteLo, out byte byteHi)
        {
            byteLo = 0;
            byteHi = 0;
            short word;
            if (!TryParseHexTextWordToShort(textWord, out word)) return;

            ShortToLoHiPairOfBytes(word, out byteLo, out byteHi);
        }

        public static bool TryParseHexTextWordToShort(string textWord, out short word)
        {
            return short.TryParse(textWord, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out word);
        }

        public static bool TryParseHexTextByteToByte(string textByte, out byte byteValue)
        {
            return byte.TryParse(textByte, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byteValue);
        }

        public static void ShortToLoHiPairOfBytes(short word, out byte byteLo, out byte byteHi)
        {
            byteLo = (byte) word;
            byteHi = (byte) ((uint) word >> 8);
        }

        public static void AddLoHiBytesToArrayOfBytes(byte byteLo, byte byteHi, List<byte> bytes, bool reverse = false)
        {
            if (reverse)
            {
                bytes.Add(byteHi);
                bytes.Add(byteLo);
            }
            else
            {
                bytes.Add(byteLo);
                bytes.Add(byteHi);
            }
        }

        public static void AddWordToArrayOfBytes(ushort data, List<byte> bytes, bool reverse = false)
        {
            var bytes1 = BitConverter.GetBytes(data);
            if (reverse)
            {
                bytes.Add(bytes1[1]);
                bytes.Add(bytes1[0]);
            }
            else
            {
                bytes.Add(bytes1[0]);
                bytes.Add(bytes1[1]);
            }
        }

        public static string BytesToSpaceSeparatedTextHexWords(IList<byte> bytes)
        {
            var stringBuilder = new StringBuilder();
            var flag = true;
            var index = 0;
            while (index < bytes.Count)
            {
                if (flag)
                    flag = false;
                else
                    stringBuilder.Append(" ");

                if (index <= bytes.Count - 1)
                {
                    var num1 = bytes[index];
                    if (index + 1 <= bytes.Count - 1)
                    {
                        var num2 = bytes[index + 1];
                        stringBuilder.Append(num2.ToString("X2"));
                        stringBuilder.Append(num1.ToString("X2"));
                        index += 2;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return stringBuilder.ToString();
        }

        public static string BytesToSpaceSeparatedTextHexBytes(IList<byte> bytes)
        {
            var stringBuilder = new StringBuilder();
            var flag = true;
            for (var index = 0; index < bytes.Count; ++index)
            {
                if (flag)
                    flag = false;
                else
                    stringBuilder.Append(" ");

                stringBuilder.Append(bytes[index].ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        public static List<ushort> BytesToWords(IList<byte> bytes)
        {
            var ushortList = new List<ushort>();
            var index = 0;
            while (index < bytes.Count && index <= bytes.Count - 1)
            {
                var byteHi = bytes[index];
                if (index + 1 <= bytes.Count - 1)
                {
                    var num = (ushort) LoHiPairOfBytesToShort(bytes[index + 1], byteHi);
                    ushortList.Add(num);
                    index += 2;
                }
                else
                {
                    break;
                }
            }

            return ushortList;
        }

        public static void ShortToBytes(short word, List<byte> bytes)
        {
            byte byteLo;
            byte byteHi;
            ShortToLoHiPairOfBytes(word, out byteLo, out byteHi);
            bytes.Add(byteHi);
            bytes.Add(byteLo);
        }

        public static List<byte> ShortToBytes(short word)
        {
            var bytes = new List<byte>();
            ShortToBytes(word, bytes);
            return bytes;
        }

        public static short LoHiPairOfBytesToShort(byte byteLo, byte byteHi)
        {
            return (short) ((byteLo << 8) + byteHi);
        }

        public static string LoHiPairOfBytesToHexToString(byte byteLo, byte byteHi)
        {
            return ((byteHi << 8) + byteLo).ToString("X4");
        }

        public static string LoHiArrayOfBytesToHexToString(List<byte> twoBytes)
        {
            return Encoding.Unicode.GetString(twoBytes.ToArray());
        }

        public static List<byte> ConvertCommaSeperatedSignedCharToBytes(string numericalsText)
        {
            return ConvertCharSeparatedTextHexBytesToBytes(numericalsText, ',');
        }

        public static List<byte> ConvertSpaceSeperatedSignedCharToBytes(string numericalsText)
        {
            return ConvertCharSeparatedTextHexBytesToBytes(numericalsText);
        }

        public static List<byte> ConvertCharSeparatedTextHexBytesToBytes(string numericalsText, char separator = ' ')
        {
            Func<string, byte> parse = textByte =>
            {
                byte byteValue = 0;
                TryParseHexTextByteToByte(textByte, out byteValue);
                return byteValue;
            };
            return ConvertCharSeparatedTextNumbersToNumbers(numericalsText, parse, separator);
        }

        public static List<short> ConvertCharSeparatedTextShortsToShorts(string numericalsText, char separator = ' ')
        {
            Func<string, short> parse = text =>
            {
                short result = 0;
                short.TryParse(text, out result);
                return result;
            };
            return ConvertCharSeparatedTextNumbersToNumbers(numericalsText, parse, separator);
        }

        public static List<float> ConvertCharSeparatedTextFloatsToFloats(string numericalsText, char separator = ' ')
        {
            Func<string, float> parse = text =>
            {
                var result = 0.0f;
                float.TryParse(text, out result);
                return result;
            };
            return ConvertCharSeparatedTextNumbersToNumbers(numericalsText, parse, separator);
        }

        public static List<T> ConvertCharSeparatedTextNumbersToNumbers<T>(string numericalsText, Func<string, T> parse,
            char separator = ' ')
        {
            var objList = new List<T>();
            var str1 = numericalsText;
            var chArray = new char[1] {separator};
            foreach (var str2 in str1.Split(chArray))
                try
                {
                    var obj = parse(str2);
                    objList.Add(obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(string.Format("Invalid format for {0}: {1}", typeof(T), str2));
                    Logger.LogError(ex);
                    return null;
                }

            return objList;
        }

        public static string ConvertToSpaceSeparatedHexString(IList<byte> bytes, char c = '0', int startIndex = 0)
        {
            var sb = new StringBuilder(bytes.Count);
            PopulateBySpaceSeparatedHexString(sb, bytes, c, startIndex);
            return sb.ToString();
        }

        public static void PopulateBySpaceSeparatedHexString(StringBuilder sb, IList<byte> bytes, char c = '0',
            int startIndex = 0)
        {
            try
            {
                for (var index = startIndex; index < bytes.Count; ++index)
                {
                    var num = bytes[index];
                    if (num == 0)
                        sb.AppendFormat("{0}{1} ", c, c);
                    else
                        sb.AppendFormat("{0:X2} ", num);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public static int FindDoubleSeparatorIndex(int startIndex, List<byte> buffer, byte sep)
        {
            var index = startIndex;
            if (buffer.Count <= startIndex + 3) return -1;

            for (var flag = false; !flag && index < buffer.Count - 3; ++index)
                flag = buffer[index] == sep && buffer[index + 1] == sep && buffer[index + 2] != sep;

            if (index == buffer.Count - 3) return -1;

            return index - 1;
        }

        public static byte GetLow4BitValue(byte b)
        {
            return (byte) (b & 15U);
        }

        public static byte GetHigh4BitValue(byte b)
        {
            return (byte) ((b & 240) >> 4);
        }

        public static byte ExtractByteFromBits(byte source, int index, int count)
        {
            var bitArray1 = new BitArray(new byte[1]
            {
                source
            });
            if (count > 8) throw new Exception("Invalid logic to use ExtractByteFromBits, count > 8");

            if (count + index > 8) throw new Exception("Invalid logic to use ExtractByteFromBits, (count + index) > 8");

            var bitArray2 = new BitArray(count);
            var index1 = 0;
            for (var index2 = index; index2 < count; ++index2)
            {
                bitArray2[index1] = bitArray1[index2];
                ++index1;
            }

            var numArray = new byte[1];
            bitArray2.CopyTo(numArray, 0);
            return numArray[0];
        }

        public static int FindDoubleSeparatorIndex(List<byte> buffer, byte sep)
        {
            return FindDoubleSeparatorIndex(0, buffer, sep);
        }

        public static byte GetLowByteA(short w)
        {
            return (byte) ((uint) w & byte.MaxValue);
        }

        public static byte GetHighByteA(short w)
        {
            return (byte) ((w & 65280) >> 8);
        }

        public static byte GetLowByteB(short w)
        {
            return (byte) w;
        }

        public static byte GetHighByteB(short w)
        {
            return (byte) ((uint) w >> 8);
        }

        public static byte[] Read2Bytes(IList<byte> bytes, int index)
        {
            return new byte[2]
            {
                ReadByte(bytes, index),
                ReadByte(bytes, index + 1)
            };
        }

        public static byte ReadByte(IList<byte> bytes, int index)
        {
            return XList.IsValidIndex(bytes, index) ? bytes[index] : (byte) 0;
        }

        public static short ReadInt16(IList<byte> bytes, int index)
        {
            return BitConverter.ToInt16(Read2Bytes(bytes, index), 0);
        }

        public static ushort ReadUInt16(IList<byte> bytes, int index)
        {
            return BitConverter.ToUInt16(Read2Bytes(bytes, index), 0);
        }

        public static List<byte> ToLoHiByteList(IList<short> shortList)
        {
            var bytes = new List<byte>();
            foreach (var word in shortList) ShortToBytes(word, bytes);

            return bytes;
        }

        public static short EvenOddValuesToZeroOneBits(IList<short> values, bool reverse = false)
        {
            short num1 = 0;
            for (byte index = 0; (int) index < values.Count; ++index)
            {
                var num2 = values[index];
                if (num2 != byte.MaxValue)
                {
                    var flag = num2 % 2 != 0;
                    if (reverse ? !flag : flag) num1 |= (short) (1 << index);
                }
            }

            return num1;
        }

        public static int SetBit(int number, int x)
        {
            return number | (1 << x);
        }

        public static int ClearBit(int number, int x)
        {
            return number & ~(1 << x);
        }

        public static int ToggleBit(int number, int x)
        {
            return number ^ (1 << x);
        }

        public static int CheckBit(int number, int x)
        {
            return number & (1 << x);
        }

        public static void ConvertVal16ToVal12AndVal4(short val16, out short val12, out byte val4)
        {
            val12 = (short) (val16 >> 4);
            val4 = (byte) ((uint) val16 & 15U);
        }

        public static void ConvertVal12AndVal4ToVal16(short val12, byte val4, out short val16)
        {
            val16 = (short) ((val12 << 4) | val4);
        }

        public static int GetValue(BitArray bitArray)
        {
            var numArray = new int[1];
            bitArray.CopyTo(numArray, 0);
            return numArray[0];
        }

        public static string GetBinrayText(BitArray bitArray)
        {
            var stringBuilder = new StringBuilder();
            for (var index = bitArray.Count - 1; index >= 0; --index)
            {
                var flag = bitArray.Get(index);
                stringBuilder.Append(flag ? '1' : '0');
            }

            return stringBuilder.ToString();
        }
    }
}