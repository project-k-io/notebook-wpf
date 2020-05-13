using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.ViewModels;

namespace ProjectK.Views
{
    public partial class OutputView : UserControl
    {
        private static readonly ILogger Log = LogManager.GetLogger<OutputView>();

        public OutputView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            ListViewMessages.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute));
#if AK_2
            Log.LoggingEvent += (s, e) => Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action) (() => ListViewMessages.ScrollIntoView(_model.AddNewRecord(e))));
#endif
        }

        private static void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if(!(target is ListView listView))    
                return;

            var stringBuilder = new StringBuilder();

            foreach (var selectedItem in listView.SelectedItems)
            {
                if (!(selectedItem is OutputRecordViewModel record))
                    continue;

                stringBuilder.AppendLine(record.Message);
            }

            var text = stringBuilder.ToString();
            Log.LogDebug($"[Clipboard] {text}");
            Clipboard.SetText(text);
        }

        private static void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(e.OriginalSource is ListView originalSource)) return;

            e.CanExecute = originalSource.SelectedItems.Count > 0;
        }
    }
}