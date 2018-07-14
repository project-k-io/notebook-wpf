// Decompiled with JetBrains decompiler
// Type: Projects.Views.ExcelCsvView
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Projects.Views
{
  public partial class ExcelCsvView : UserControl, IComponentConnector
  {
    internal Button buttonCopyFromExelCsvFormat;
    private bool _contentLoaded;

    public ExcelCsvView()
    {
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Projects.Views;component/excelcsvview.xaml", UriKind.Relative));
    }

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.buttonCopyFromExelCsvFormat = (Button) target;
      else
        this._contentLoaded = true;
    }
  }
}
