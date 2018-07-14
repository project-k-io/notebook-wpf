// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.OutputView
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Vibor.Generic.ViewModels;
using Vibor.Helpers;

namespace Vibor.View.Helpers
{
  public partial class OutputView : UserControl, IComponentConnector
  {
    private static readonly ILog Log = XLogger.GetLogger();
    private readonly OutputViewModel _model = new OutputViewModel();
    internal ListView ListViewMessages;
    private bool _contentLoaded;

    public OutputView()
    {
      this.InitializeComponent();
      this.Init();
    }

    private void Init()
    {
      this.DataContext = (object) this._model;
      this.ListViewMessages.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Copy, new ExecutedRoutedEventHandler(OutputView.CopyCmdExecuted), new CanExecuteRoutedEventHandler(OutputView.CopyCmdCanExecute)));
      this._model.Init();
      OutputView.Log.LoggingEvent += (EventHandler<LoggingEventArgs>) ((s, e) => this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) (() => this.ListViewMessages.ScrollIntoView((object) this._model.AddNewRecord(e)))));
    }

    private static void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
    {
      ListView originalSource = e.OriginalSource as ListView;
      if (originalSource == null)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (object selectedItem in (IEnumerable) originalSource.SelectedItems)
        stringBuilder.AppendLine(selectedItem.ToString());
      Clipboard.SetText(stringBuilder.ToString());
    }

    private static void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      ListView originalSource = e.OriginalSource as ListView;
      if (originalSource == null)
        return;
      e.CanExecute = originalSource.SelectedItems.Count > 0;
    }

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Vibor.View.Helpers;component/outputview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.ListViewMessages = (ListView) target;
      else
        this._contentLoaded = true;
    }
  }
}
