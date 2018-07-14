﻿// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.ComparisonComparer`1
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;

namespace Vibor.Helpers
{
  public class ComparisonComparer<T> : IComparer<T>
  {
    private readonly Comparison<T> _comparison;

    public ComparisonComparer(Comparison<T> comparison)
    {
      this._comparison = comparison;
    }

    public int Compare(T x, T y)
    {
      return this._comparison(x, y);
    }
  }
}
