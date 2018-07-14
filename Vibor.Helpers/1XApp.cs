// Decompiled with JetBrains decompiler
// Type: Target
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

public class Target
{
  public string Name { get; set; }

  public string Url { get; set; }

  public override string ToString()
  {
    return this.Name + " : " + this.Url;
  }
}
