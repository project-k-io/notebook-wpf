using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Vibor.Logging;
using Vibor.View.Helpers.ViewModels;

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
            Log.LoggingEvent += (s, e) => Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action) (() => ListViewMessages.ScrollIntoView(_model.AddNewRecord(e))));
        }

        private static void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as ListView;
            if (originalSource == null) return;

            var stringBuilder = new StringBuilder();
            foreach (var selectedItem in originalSource.SelectedItems)
                stringBuilder.AppendLine(selectedItem.ToString());

            Clipboard.SetText(stringBuilder.ToString());
        }

        private static void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as ListView;
            if (originalSource == null) return;

            e.CanExecute = originalSource.SelectedItems.Count > 0;
        }
    }
}