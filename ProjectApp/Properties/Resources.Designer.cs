// Decompiled with JetBrains decompiler
// Type: ProjectApp.Properties.Resources
// Assembly: ProjectApp, Version=1.1.8.29131, Culture=neutral, PublicKeyToken=null
// MVID: A7331AD2-AF8A-4A84-BF9D-60C36001D1E0
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\ProjectApp.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ProjectApp.Properties
{
  [DebuggerNonUserCode]
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) ProjectApp.Properties.Resources.resourceMan, (object) null))
          ProjectApp.Properties.Resources.resourceMan = new ResourceManager("ProjectApp.Properties.Resources", typeof (ProjectApp.Properties.Resources).Assembly);
        return ProjectApp.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return ProjectApp.Properties.Resources.resourceCulture;
      }
      set
      {
        ProjectApp.Properties.Resources.resourceCulture = value;
      }
    }
  }
}
