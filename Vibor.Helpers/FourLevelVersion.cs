// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.FourLevelVersion
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;
using Vibor.Helpers;
using Vibor.Logging;

namespace Vibor.Helpers
{
  public class FourLevelVersion
  {
    private readonly ILog _logger = LogManager.GetLogger(nameof (FourLevelVersion));

    public bool IgnoreProtocolVersion { get; set; }

    public short Major { get; set; }

    public short Minor { get; set; }

    public short Protocol { get; set; }

    public short Build { get; set; }

    public bool IsValid
    {
      get
      {
        return this.Major != (short) 0;
      }
    }

    public static bool operator <(FourLevelVersion a, FourLevelVersion b)
    {
      if ((int) a.Major != (int) b.Major)
        return (int) a.Major < (int) b.Major;
      if ((int) a.Minor != (int) b.Minor)
        return (int) a.Minor < (int) b.Minor;
      if (a.IgnoreProtocolVersion)
        return false;
      return (int) a.Protocol < (int) b.Protocol;
    }

    public static bool operator >(FourLevelVersion a, FourLevelVersion b)
    {
      return b < a;
    }

    public static bool operator ==(FourLevelVersion a, FourLevelVersion b)
    {
      if (object.ReferenceEquals( a,  b))
        return true;
      if ( a == null ||  b == null)
        return false;
      return (int) a.Major == (int) b.Major && (int) a.Minor == (int) b.Minor && (a.IgnoreProtocolVersion || (int) a.Protocol == (int) b.Protocol);
    }

    public static bool operator !=(FourLevelVersion a, FourLevelVersion b)
    {
      return !(a == b);
    }

    public override bool Equals(object obj)
    {
      return this == obj as FourLevelVersion;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      string str = this.IgnoreProtocolVersion ? string.Format("{0}.{1}",  this.Major,  this.Minor) : string.Format("{0}.{1}.{2}",  this.Major,  this.Minor,  this.Protocol);
      return this.Build == (short) 0 || this.IgnoreProtocolVersion ? str : str + "." + this.Build.ToString();
    }

    public void Parse(string text)
    {
      try
      {
        if (string.IsNullOrEmpty(text))
          return;
        string[] strArray = text.Split('.');
        if (!XList.IsValidIndex<string>((ICollection<string>) strArray, 0))
          return;
        short result1 = 0;
        if (!short.TryParse(strArray[0], out result1))
          return;
        this.Major = result1;
        if (!XList.IsValidIndex<string>((ICollection<string>) strArray, 1))
          return;
        short result2 = 0;
        if (!short.TryParse(strArray[1], out result2))
          return;
        this.Minor = result2;
        if (this.IgnoreProtocolVersion || !XList.IsValidIndex<string>((ICollection<string>) strArray, 2))
          return;
        short result3 = 0;
        if (!short.TryParse(strArray[2], out result3))
          return;
        this.Protocol = result3;
        if (strArray.Length > 3 && short.TryParse(strArray[3], out result3))
          this.Build = result3;
      }
      catch (Exception ex)
      {
        this._logger.Error( ex);
      }
    }
  }
}
