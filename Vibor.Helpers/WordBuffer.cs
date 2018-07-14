// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.WordBuffer
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;

namespace Vibor.Helpers
{
    public class WordBuffer
    {
        private IList<byte> _bytes;
        private int _index;

        public WordBuffer()
        {
            Bytes = new List<byte>();
        }

        public WordBuffer(IList<byte> bytes)
        {
            Bytes = bytes;
        }

        public string Text
        {
            get => BitByteConverter.BytesToSpaceSeparatedTextHexWords(Bytes);
            set => Bytes = BitByteConverter.SpaceSeparatedTextHexWordsToBytes(value, false);
        }

        public IList<byte> Bytes
        {
            get => _bytes;
            set
            {
                _bytes = value;
                _index = 0;
            }
        }

        public byte[] Read2Bytes()
        {
            return new byte[2] {ReadByte(), ReadByte()};
        }

        public void Write2Bytes(byte[] bytes)
        {
            var b1 = bytes[0];
            var b2 = bytes[1];
            Write(b1);
            Write(b2);
        }

        public short ReadInt16()
        {
            var num = BitByteConverter.ReadInt16(Bytes, _index);
            _index += 2;
            return num;
        }

        public ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(Read2Bytes(), 0);
        }

        public byte ReadByte()
        {
            var num = BitByteConverter.ReadByte(Bytes, _index);
            ++_index;
            return num;
        }

        public void Write(short s)
        {
            Write2Bytes(BitConverter.GetBytes(s));
        }

        public void Write(ushort s)
        {
            Write2Bytes(BitConverter.GetBytes(s));
        }

        public void Write(byte b)
        {
            Bytes.Add(b);
        }

        public void Write(sbyte b)
        {
            Bytes.Add((byte) b);
        }
    }
}