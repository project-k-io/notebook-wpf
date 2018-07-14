// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.Matrix`1
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;

namespace Vibor.Helpers
{
    public class Matrix<T>
    {
        public Matrix()
        {
        }

        public Matrix(List<List<T>> data)
        {
            Data = XMatrix.Copy(data);
        }

        public Matrix(Matrix<T> rs)
            : this(rs.Data)
        {
        }

        public bool IsEmpty => Data.Count == 0;

        public bool HasData => !IsEmpty;

        public List<List<T>> Data { get; set; } = new List<List<T>>();

        public List<T> this[int row] => Data[row];

        public int RowCount => Data.Count;

        public int ColCount
        {
            get
            {
                if (RowCount > 0)
                    return Data[0].Count;
                return 0;
            }
        }

        public override string ToString()
        {
            if (IsEmpty)
                return "empty";
            return string.Format("has data, rows: {0}  cols:{1}.", RowCount, ColCount);
        }

        public T FindValueAndLoaction(T minValue, Func<T, T, bool> compare, out int maxRowIndex, out int maxColIndex)
        {
            return XMatrix.FindValueAndLocation(Data, minValue, compare, out maxRowIndex, out maxColIndex);
        }

        public T FindValue(T minValue, Func<T, T, bool> compare)
        {
            return XMatrix.FindValue(Data, minValue, compare);
        }
    }
}