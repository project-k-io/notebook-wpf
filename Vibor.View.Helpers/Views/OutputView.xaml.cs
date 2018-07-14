// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.OutputView
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Vibor.Logging;
using Vibor.ViewModels;

namespace Vibor.View.Helpers.Views
{
    public partial class OutputView : UserControl, IComponentConnector
    {
        private static readonly ILog Log = LogManager.GetLogger();
        private readonly OutputViewModel _model = new OutputViewModel();

        public OutputView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            DataContext = _model;
            ListViewMessages.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted,
                CopyCmdCanExecute));
            _model.Init();
#if AK_1
            OutputView.Log.LoggingEvent +=
 (EventHandler<LoggingEventArgs>) ((s, e) => this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (() => this.ListViewMessages.ScrollIntoView((object) this._model.AddNewRecord(e)))));
#endif
        }

        private static void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as ListView;
            if (originalSource == null)
                return;
            var stringBuilder = new StringBuilder();
            foreach (var selectedItem in originalSource.SelectedItems)
                stringBuilder.AppendLine(selectedItem.ToString());
            Clipboard.SetText(stringBuilder.ToString());
        }

        private static void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as ListView;
            if (originalSource == null)
                return;
            e.CanExecute = originalSource.SelectedItems.Count > 0;
        }
    }
}