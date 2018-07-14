// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.ViewModels.OutputRecordViewModel
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using System;
using Vibor.Helpers;

namespace Vibor.Generic.ViewModels
{
  public class OutputRecordViewModel
  {
    private const string Format = "{0}\t{1}\t{2}\t{3}\t{4}";

    public int ID { get; set; }

    public Level Type { get; set; }

    public DateTime Date { get; set; }

    public string State { get; set; }

    public string Message { get; set; }

    public static string Header
    {
      get
      {
        return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", (object) "ID", (object) "Type", (object) "Date", (object) "State", (object) "Message");
      }
    }

    public override string ToString()
    {
      return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", (object) this.ID, (object) this.Type, (object) this.Date, (object) this.State, (object) this.Message);
    }
  }
}
