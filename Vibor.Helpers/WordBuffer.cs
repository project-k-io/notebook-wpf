// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.Models.WordBuffer
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;

namespace Vibor.Generic.Models
{
  public class WordBuffer
  {
    private IList<byte> _bytes;
    private int _index;

    public WordBuffer()
    {
      this.Bytes = (IList<byte>) new List<byte>();
    }

    public WordBuffer(IList<byte> bytes)
    {
      this.Bytes = bytes;
    }

    public string Text
    {
      get
      {
        return BitByteConverter.BytesToSpaceSeparatedTextHexWords(this.Bytes);
      }
      set
      {
        this.Bytes = (IList<byte>) BitByteConverter.SpaceSeparatedTextHexWordsToBytes(value, false);
      }
    }

    public IList<byte> Bytes
    {
      get
      {
        return this._bytes;
      }
      set
      {
        this._bytes = value;
        this._index = 0;
      }
    }

    public byte[] Read2Bytes()
    {
      return new byte[2]{ this.ReadByte(), this.ReadByte() };
    }

    public void Write2Bytes(byte[] bytes)
    {
      byte b1 = bytes[0];
      byte b2 = bytes[1];
      this.Write(b1);
      this.Write(b2);
    }

    public short ReadInt16()
    {
      short num = BitByteConverter.ReadInt16(this.Bytes, this._index);
      this._index += 2;
      return num;
    }

    public ushort ReadUInt16()
    {
      return BitConverter.ToUInt16(this.Read2Bytes(), 0);
    }

    public byte ReadByte()
    {
      byte num = BitByteConverter.ReadByte(this.Bytes, this._index);
      ++this._index;
      return num;
    }

    public void Write(short s)
    {
      this.Write2Bytes(BitConverter.GetBytes(s));
    }

    public void Write(ushort s)
    {
      this.Write2Bytes(BitConverter.GetBytes(s));
    }

    public void Write(byte b)
    {
      this.Bytes.Add(b);
    }

    public void Write(sbyte b)
    {
      this.Bytes.Add((byte) b);
    }
  }
}
