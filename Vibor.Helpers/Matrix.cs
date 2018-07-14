// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.Matrix`1
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;
using Vibor.Helpers;

namespace Vibor.Helpers
{
  public class Matrix<T>
  {
    private List<List<T>> _data = new List<List<T>>();

    public Matrix()
    {
    }

    public Matrix(List<List<T>> data)
    {
      this._data = XMatrix.Copy<T>(data);
    }

    public Matrix(Matrix<T> rs)
      : this(rs.Data)
    {
    }

    public bool IsEmpty
    {
      get
      {
        return this._data.Count == 0;
      }
    }

    public bool HasData
    {
      get
      {
        return !this.IsEmpty;
      }
    }

    public List<List<T>> Data
    {
      get
      {
        return this._data;
      }
      set
      {
        this._data = value;
      }
    }

    public List<T> this[int row]
    {
      get
      {
        return this._data[row];
      }
    }

    public int RowCount
    {
      get
      {
        return this._data.Count;
      }
    }

    public int ColCount
    {
      get
      {
        if (this.RowCount > 0)
          return this._data[0].Count;
        return 0;
      }
    }

    public override string ToString()
    {
      if (this.IsEmpty)
        return "empty";
      return string.Format("has data, rows: {0}  cols:{1}.",  this.RowCount,  this.ColCount);
    }

    public T FindValueAndLoaction(T minValue, Func<T, T, bool> compare, out int maxRowIndex, out int maxColIndex)
    {
      return XMatrix.FindValueAndLocation<T>(this.Data, minValue, compare, out maxRowIndex, out maxColIndex);
    }

    public T FindValue(T minValue, Func<T, T, bool> compare)
    {
      return XMatrix.FindValue<T>(this.Data, minValue, compare);
    }
  }
}
