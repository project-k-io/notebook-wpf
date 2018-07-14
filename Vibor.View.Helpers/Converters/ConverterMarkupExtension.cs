// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Converters.ConverterMarkupExtension`1
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.Windows.Markup;

namespace Vibor.View.Helpers.Converters
{
  public class ConverterMarkupExtension<T> : MarkupExtension where T : class, new()
  {
    private static T extensionConverter;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if ((object) ConverterMarkupExtension<T>.extensionConverter == null)
        ConverterMarkupExtension<T>.extensionConverter = Activator.CreateInstance<T>();
      return (object) ConverterMarkupExtension<T>.extensionConverter;
    }
  }
}
