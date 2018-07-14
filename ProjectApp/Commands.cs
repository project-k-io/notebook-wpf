using System.Windows.Input;
namespace ProjectApp
{
    public class Commands
    {
        static Commands()
        {
            Clear = new RoutedUICommand("Clear Project", nameof(Clear), typeof(Commands));
            Edit = new RoutedUICommand("Edit Project", nameof(Edit), typeof(Commands));
            FixTime = new RoutedUICommand("FixT ime", nameof(FixTime), typeof(Commands));
            ExtractContext = new RoutedUICommand("Extract Context", nameof(ExtractContext), typeof(Commands));
            FixContext = new RoutedUICommand("Fix Context", nameof(FixContext), typeof(Commands));
            FixTitles = new RoutedUICommand("Fix Titles", nameof(FixTitles), typeof(Commands));
            FixTypes = new RoutedUICommand("Fix Types", nameof(FixTypes), typeof(Commands));
            CopyTask = new RoutedUICommand("Copy Task    ", "CopyTask    ", typeof(Commands));
            ContinueTask = new RoutedUICommand("Continue Task", nameof(ContinueTask), typeof(Commands));
        }

        public static RoutedUICommand Clear { get; }
        public static RoutedUICommand Edit { get; }
        public static RoutedUICommand FixTime { get; }
        public static RoutedUICommand ExtractContext { get; }
        public static RoutedUICommand FixContext { get; }
        public static RoutedUICommand FixTitles { get; }
        public static RoutedUICommand FixTypes { get; }
        public static RoutedUICommand CopyTask { get; }
        public static RoutedUICommand ContinueTask { get; }
    }
}