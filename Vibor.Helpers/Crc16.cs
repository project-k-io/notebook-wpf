using System.Collections.Generic;
using System.Linq;

namespace Vibor.Helpers
{
    public class Crc16
    {
        private const ushort P16 = 40961;
        private readonly ushort[] _table = new ushort[256];

        public Crc16()
        {
            InitTable();
        }

        private void InitTable()
        {
            for (var index1 = 0; index1 < 256; ++index1)
            {
                ushort num1 = 0;
                var num2 = (ushort) index1;
                for (var index2 = 0; index2 < 8; ++index2)
                {
                    if (((num1 ^ num2) & 1) > 0)
                        num1 = (ushort) ((num1 >> 1) ^ 40961);
                    else
                        num1 >>= 1;
                    num2 >>= 1;
                }

                _table[index1] = num1;
            }
        }

        public ushort Calculate(ushort crc, char c)
        {
            var num1 = (ushort) (byte.MaxValue & (uint) c);
            var num2 = (ushort) (crc ^ (uint) num1);
            crc = (ushort) (((uint) crc >> 8) ^ _table[num2 & byte.MaxValue]);
            return crc;
        }

        public ushort Calculate(char[] chars)
        {
            return chars.Aggregate((ushort) 0, Calculate);
        }

        public ushort Calculate(ushort crc, byte b)
        {
            var num = (byte) (crc ^ (uint) b);
            crc = (ushort) (((uint) crc >> 8) ^ _table[num]);
            return crc;
        }

        public ushort Calculate(IList<byte> bytes)
        {
            return bytes.Aggregate((ushort) 0, Calculate);
        }

        public ushort CalculateFp41(IList<byte> bytes)
        {
            return bytes.Aggregate((ushort) 45057, Calculate);
        }
    }
}