using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using ProjectK.Extensions.Logging;
using ProjectK.ToolKit.ViewModels;

namespace ProjectK.Toolkit.Wpf.Controls.Output;

public partial class OutputView : UserControl
{
    private static readonly ILogger Log = LogManager.GetLogger<OutputView>();

    public OutputView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ListViewMessages.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute));

        if (!(ListViewMessages.Items is INotifyCollectionChanged collectionChanged))
            return;

        collectionChanged.CollectionChanged += (o, args) =>
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                if (args.NewItems == null || args.NewItems.Count <= 0) return;

                var item = args.NewItems[0];
                if (item == null)
                    return;

                ListViewMessages.ScrollIntoView(item);
            }
        };
    }

    private static void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
    {
        if (!(target is ListView listView))
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