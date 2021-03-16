﻿namespace ProjectK.Notebook.WinApp.Models
{
    public class LayoutSettingsModel
    {
        public WindowSettingsModel Window { get; set; } = new();
        public OutputSettingsModel Output { get; set; } = new();
        public MainViewSettingsModel MainViewSettings { get; set; } = new();
    }
}