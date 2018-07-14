// Decompiled with JetBrains decompiler
// Type: XamlGeneratedNamespace.GeneratedInternalTypeHelper
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;

namespace XamlGeneratedNamespace
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DebuggerNonUserCode]
  public sealed class GeneratedInternalTypeHelper : InternalTypeHelper
  {
    protected override object CreateInstance(Type type, CultureInfo culture)
    {
      return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, (object[]) null, culture);
    }

    protected override object GetPropertyValue(PropertyInfo propertyInfo, object target, CultureInfo culture)
    {
      return propertyInfo.GetValue(target, BindingFlags.Default, (Binder) null, (object[]) null, culture);
    }

    protected override void SetPropertyValue(PropertyInfo propertyInfo, object target, object value, CultureInfo culture)
    {
      propertyInfo.SetValue(target, value, BindingFlags.Default, (Binder) null, (object[]) null, culture);
    }

    protected override Delegate CreateDelegate(Type delegateType, object target, string handler)
    {
      return (Delegate) target.GetType().InvokeMember("_CreateDelegate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, (Binder) null, target, new object[2]{ (object) delegateType, (object) handler }, (CultureInfo) null);
    }

    protected override void AddEventHandler(EventInfo eventInfo, object target, Delegate handler)
    {
      eventInfo.AddEventHandler(target, handler);
    }
  }
}
