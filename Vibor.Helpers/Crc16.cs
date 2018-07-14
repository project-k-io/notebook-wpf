// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.Crc16
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Vibor.Helpers
{
  public class Crc16
  {
    private readonly ushort[] _table = new ushort[256];
    private const ushort P16 = 40961;

    public Crc16()
    {
      this.InitTable();
    }

    private void InitTable()
    {
      for (int index1 = 0; index1 < 256; ++index1)
      {
        ushort num1 = 0;
        ushort num2 = (ushort) index1;
        for (int index2 = 0; index2 < 8; ++index2)
        {
          if ((((int) num1 ^ (int) num2) & 1) > 0)
            num1 = (ushort) ((int) num1 >> 1 ^ 40961);
          else
            num1 >>= 1;
          num2 >>= 1;
        }
        this._table[index1] = num1;
      }
    }

    public ushort Calculate(ushort crc, char c)
    {
      ushort num1 = (ushort) ((uint) byte.MaxValue & (uint) c);
      ushort num2 = (ushort) ((uint) crc ^ (uint) num1);
      crc = (ushort) ((uint) crc >> 8 ^ (uint) this._table[(int) num2 & (int) byte.MaxValue]);
      return crc;
    }

    public ushort Calculate(char[] chars)
    {
      return ((IEnumerable<char>) chars).Aggregate<char, ushort>((ushort) 0, new Func<ushort, char, ushort>(this.Calculate));
    }

    public ushort Calculate(ushort crc, byte b)
    {
      byte num = (byte) ((uint) crc ^ (uint) b);
      crc = (ushort) ((uint) crc >> 8 ^ (uint) this._table[(int) num]);
      return crc;
    }

    public ushort Calculate(IList<byte> bytes)
    {
      return bytes.Aggregate<byte, ushort>((ushort) 0, new Func<ushort, byte, ushort>(this.Calculate));
    }

    public ushort CalculateFp41(IList<byte> bytes)
    {
      return bytes.Aggregate<byte, ushort>((ushort) 45057, new Func<ushort, byte, ushort>(this.Calculate));
    }
  }
}
