using System;
using System.Collections.Generic;

namespace Vibor.Helpers
{
    internal class Matrix<T>
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