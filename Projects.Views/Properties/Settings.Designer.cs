// Decompiled with JetBrains decompiler
// Type: Projects.Views.Properties.Settings
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Projects.Views.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        Settings defaultInstance = Settings.defaultInstance;
        return defaultInstance;
      }
    }

    [DefaultSettingValue("")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public string LastProjectFileName
    {
      get
      {
        return (string) this[nameof (LastProjectFileName)];
      }
      set
      {
        this[nameof (LastProjectFileName)] = (object) value;
      }
    }
  }
}
