namespace ProjectK.Notebook.WinApp.Settings
{
    public class LayoutSettings
    {
        public OutputSettings Output { get; set; } = new();
        public int NavigatorWidth { get; set; } = 200;
        public int OutputHeight { get; set; } = 200;
        public int PropertiesWidth { get; set; } = 200;
    }
}