using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using ProjectK.Utils;
using ProjectK.View.Helpers.ViewModels;

namespace ProjectK.View.Helpers.Views
{
    public partial class OutputView : UserControl
    {
        private static readonly ILogger Log = LogManager.GetLogger<OutputView>();
        private readonly OutputViewModel _model = new OutputViewModel();

        public OutputView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            DataContext = _model;
            ListViewMessages.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute));
            _model.Init();
#if AK_2
            Log.LoggingEvent += (s, e) => Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action) (() => ListViewMessages.ScrollIntoView(_model.AddNewRecord(e))));
#endif
        }

        private static void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (!(e.OriginalSource is ListView originalSource)) return;

            var stringBuilder = new StringBuilder();
            foreach (var selectedItem in originalSource.SelectedItems)
                stringBuilder.AppendLine(selectedItem.ToString());

            Clipboard.SetText(stringBuilder.ToString());
        }

        private static void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(e.OriginalSource is ListView originalSource)) return;

            e.CanExecute = originalSource.SelectedItems.Count > 0;
        }
    }
}