// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.Models.BitByteConverter
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Vibor.Generic.Models
{
  public class BitByteConverter
  {
    private static readonly ILog Logger = LogManager.GetLogger("Converter");

    public static List<byte> SpaceSeparatedTextHexWordsToBytes(string text, bool reverse = false)
    {
      if (string.IsNullOrEmpty(text))
        return (List<byte>) null;
      string[] strArray = text.Split(' ');
      List<byte> bytes = new List<byte>();
      foreach (string textWord in strArray)
      {
        try
        {
          if (!(textWord == string.Empty))
            BitByteConverter.AddHexTextWordAsArrayOfBytes(textWord, bytes, reverse);
        }
        catch (Exception ex)
        {
          BitByteConverter.Logger.Error((object) ex);
        }
      }
      return bytes;
    }

    public static void AddHexTextWordAsArrayOfBytes(string textWord, List<byte> bytes, bool reverse = false)
    {
      byte byteLo;
      byte byteHi;
      BitByteConverter.HexTextWordToLoHiBytes(textWord, out byteLo, out byteHi);
      BitByteConverter.AddLoHiBytesToArrayOfBytes(byteLo, byteHi, bytes, reverse);
    }

    public static void SwapHiLowBytes(IList<byte> bytes)
    {
      int index = 0;
      while (index < bytes.Count - 1)
      {
        byte num = bytes[index + 1];
        bytes[index + 1] = bytes[index];
        bytes[index] = num;
        index += 2;
      }
    }

    public static void HexTextWordToLoHiBytes(string textWord, out byte byteLo, out byte byteHi)
    {
      byteLo = (byte) 0;
      byteHi = (byte) 0;
      short word;
      if (!BitByteConverter.TryParseHexTextWordToShort(textWord, out word))
        return;
      BitByteConverter.ShortToLoHiPairOfBytes(word, out byteLo, out byteHi);
    }

    public static bool TryParseHexTextWordToShort(string textWord, out short word)
    {
      return short.TryParse(textWord, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out word);
    }

    public static bool TryParseHexTextByteToByte(string textByte, out byte byteValue)
    {
      return byte.TryParse(textByte, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out byteValue);
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
      byte[] bytes1 = BitConverter.GetBytes(data);
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
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      int index = 0;
      while (index < bytes.Count)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(" ");
        if (index <= bytes.Count - 1)
        {
          byte num1 = bytes[index];
          if (index + 1 <= bytes.Count - 1)
          {
            byte num2 = bytes[index + 1];
            stringBuilder.Append(num2.ToString("X2"));
            stringBuilder.Append(num1.ToString("X2"));
            index += 2;
          }
          else
            break;
        }
        else
          break;
      }
      return stringBuilder.ToString();
    }

    public static string BytesToSpaceSeparatedTextHexBytes(IList<byte> bytes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      for (int index = 0; index < bytes.Count; ++index)
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
      List<ushort> ushortList = new List<ushort>();
      int index = 0;
      while (index < bytes.Count && index <= bytes.Count - 1)
      {
        byte byteHi = bytes[index];
        if (index + 1 <= bytes.Count - 1)
        {
          ushort num = (ushort) BitByteConverter.LoHiPairOfBytesToShort(bytes[index + 1], byteHi);
          ushortList.Add(num);
          index += 2;
        }
        else
          break;
      }
      return ushortList;
    }

    public static void ShortToBytes(short word, List<byte> bytes)
    {
      byte byteLo;
      byte byteHi;
      BitByteConverter.ShortToLoHiPairOfBytes(word, out byteLo, out byteHi);
      bytes.Add(byteHi);
      bytes.Add(byteLo);
    }

    public static List<byte> ShortToBytes(short word)
    {
      List<byte> bytes = new List<byte>();
      BitByteConverter.ShortToBytes(word, bytes);
      return bytes;
    }

    public static short LoHiPairOfBytesToShort(byte byteLo, byte byteHi)
    {
      return (short) (((int) byteLo << 8) + (int) byteHi);
    }

    public static string LoHiPairOfBytesToHexToString(byte byteLo, byte byteHi)
    {
      return (((int) byteHi << 8) + (int) byteLo).ToString("X4");
    }

    public static string LoHiArrayOfBytesToHexToString(List<byte> twoBytes)
    {
      return Encoding.Unicode.GetString(twoBytes.ToArray());
    }

    public static List<byte> ConvertCommaSeperatedSignedCharToBytes(string numericalsText)
    {
      return BitByteConverter.ConvertCharSeparatedTextHexBytesToBytes(numericalsText, ',');
    }

    public static List<byte> ConvertSpaceSeperatedSignedCharToBytes(string numericalsText)
    {
      return BitByteConverter.ConvertCharSeparatedTextHexBytesToBytes(numericalsText, ' ');
    }

    public static List<byte> ConvertCharSeparatedTextHexBytesToBytes(string numericalsText, char separator = ' ')
    {
      Func<string, byte> parse = (Func<string, byte>) (textByte =>
      {
        byte byteValue = 0;
        BitByteConverter.TryParseHexTextByteToByte(textByte, out byteValue);
        return byteValue;
      });
      return BitByteConverter.ConvertCharSeparatedTextNumbersToNumbers<byte>(numericalsText, parse, separator);
    }

    public static List<short> ConvertCharSeparatedTextShortsToShorts(string numericalsText, char separator = ' ')
    {
      Func<string, short> parse = (Func<string, short>) (text =>
      {
        short result = 0;
        short.TryParse(text, out result);
        return result;
      });
      return BitByteConverter.ConvertCharSeparatedTextNumbersToNumbers<short>(numericalsText, parse, separator);
    }

    public static List<float> ConvertCharSeparatedTextFloatsToFloats(string numericalsText, char separator = ' ')
    {
      Func<string, float> parse = (Func<string, float>) (text =>
      {
        float result = 0.0f;
        float.TryParse(text, out result);
        return result;
      });
      return BitByteConverter.ConvertCharSeparatedTextNumbersToNumbers<float>(numericalsText, parse, separator);
    }

    public static List<T> ConvertCharSeparatedTextNumbersToNumbers<T>(string numericalsText, Func<string, T> parse, char separator = ' ')
    {
      List<T> objList = new List<T>();
      string str1 = numericalsText;
      char[] chArray = new char[1]{ separator };
      foreach (string str2 in str1.Split(chArray))
      {
        try
        {
          T obj = parse(str2);
          objList.Add(obj);
        }
        catch (Exception ex)
        {
          BitByteConverter.Logger.Error((object) string.Format("Invalid format for {0}: {1}", (object) typeof (T), (object) str2));
          BitByteConverter.Logger.Error((object) ex);
          return (List<T>) null;
        }
      }
      return objList;
    }

    public static string ConvertToSpaceSeparatedHexString(IList<byte> bytes, char c = '0', int startIndex = 0)
    {
      StringBuilder sb = new StringBuilder(bytes.Count);
      BitByteConverter.PopulateBySpaceSeparatedHexString(sb, bytes, c, startIndex);
      return sb.ToString();
    }

    public static void PopulateBySpaceSeparatedHexString(StringBuilder sb, IList<byte> bytes, char c = '0', int startIndex = 0)
    {
      try
      {
        for (int index = startIndex; index < bytes.Count; ++index)
        {
          byte num = bytes[index];
          if (num == (byte) 0)
            sb.AppendFormat("{0}{1} ", (object) c, (object) c);
          else
            sb.AppendFormat("{0:X2} ", (object) num);
        }
      }
      catch (Exception ex)
      {
        BitByteConverter.Logger.Error((object) ex);
      }
    }

    public static int FindDoubleSeparatorIndex(int startIndex, List<byte> buffer, byte sep)
    {
      int index = startIndex;
      if (buffer.Count <= startIndex + 3)
        return -1;
      for (bool flag = false; !flag && index < buffer.Count - 3; ++index)
        flag = (int) buffer[index] == (int) sep && (int) buffer[index + 1] == (int) sep && (int) buffer[index + 2] != (int) sep;
      if (index == buffer.Count - 3)
        return -1;
      return index - 1;
    }

    public static byte GetLow4BitValue(byte b)
    {
      return (byte) ((uint) b & 15U);
    }

    public static byte GetHigh4BitValue(byte b)
    {
      return (byte) (((int) b & 240) >> 4);
    }

    public static byte ExtractByteFromBits(byte source, int index, int count)
    {
      BitArray bitArray1 = new BitArray(new byte[1]
      {
        source
      });
      if (count > 8)
        throw new Exception("Invalid logic to use ExtractByteFromBits, count > 8");
      if (count + index > 8)
        throw new Exception("Invalid logic to use ExtractByteFromBits, (count + index) > 8");
      BitArray bitArray2 = new BitArray(count);
      int index1 = 0;
      for (int index2 = index; index2 < count; ++index2)
      {
        bitArray2[index1] = bitArray1[index2];
        ++index1;
      }
      byte[] numArray = new byte[1];
      bitArray2.CopyTo((Array) numArray, 0);
      return numArray[0];
    }

    public static int FindDoubleSeparatorIndex(List<byte> buffer, byte sep)
    {
      return BitByteConverter.FindDoubleSeparatorIndex(0, buffer, sep);
    }

    public static byte GetLowByteA(short w)
    {
      return (byte) ((uint) w & (uint) byte.MaxValue);
    }

    public static byte GetHighByteA(short w)
    {
      return (byte) (((int) w & 65280) >> 8);
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
        BitByteConverter.ReadByte(bytes, index),
        BitByteConverter.ReadByte(bytes, index + 1)
      };
    }

    public static byte ReadByte(IList<byte> bytes, int index)
    {
      return XList.IsValidIndex<byte>((ICollection<byte>) bytes, index) ? bytes[index] : (byte) 0;
    }

    public static short ReadInt16(IList<byte> bytes, int index)
    {
      return BitConverter.ToInt16(BitByteConverter.Read2Bytes(bytes, index), 0);
    }

    public static ushort ReadUInt16(IList<byte> bytes, int index)
    {
      return BitConverter.ToUInt16(BitByteConverter.Read2Bytes(bytes, index), 0);
    }

    public static List<byte> ToLoHiByteList(IList<short> shortList)
    {
      List<byte> bytes = new List<byte>();
      foreach (short word in (IEnumerable<short>) shortList)
        BitByteConverter.ShortToBytes(word, bytes);
      return bytes;
    }

    public static short EvenOddValuesToZeroOneBits(IList<short> values, bool reverse = false)
    {
      short num1 = 0;
      for (byte index = 0; (int) index < values.Count; ++index)
      {
        short num2 = values[(int) index];
        if (num2 != (short) byte.MaxValue)
        {
          bool flag = (int) num2 % 2 != 0;
          if (reverse ? !flag : flag)
            num1 |= (short) (1 << (int) index);
        }
      }
      return num1;
    }

    public static int SetBit(int number, int x)
    {
      return number | 1 << x;
    }

    public static int ClearBit(int number, int x)
    {
      return number & ~(1 << x);
    }

    public static int ToggleBit(int number, int x)
    {
      return number ^ 1 << x;
    }

    public static int CheckBit(int number, int x)
    {
      return number & 1 << x;
    }

    public static void ConvertVal16ToVal12AndVal4(short val16, out short val12, out byte val4)
    {
      val12 = (short) ((int) val16 >> 4);
      val4 = (byte) ((uint) val16 & 15U);
    }

    public static void ConvertVal12AndVal4ToVal16(short val12, byte val4, out short val16)
    {
      val16 = (short) ((int) val12 << 4 | (int) val4);
    }

    public static int GetValue(BitArray bitArray)
    {
      int[] numArray = new int[1];
      bitArray.CopyTo((Array) numArray, 0);
      return numArray[0];
    }

    public static string GetBinrayText(BitArray bitArray)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = bitArray.Count - 1; index >= 0; --index)
      {
        bool flag = bitArray.Get(index);
        stringBuilder.Append(flag ? '1' : '0');
      }
      return stringBuilder.ToString();
    }
  }
}
