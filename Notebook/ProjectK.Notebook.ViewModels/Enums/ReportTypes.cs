using System.ComponentModel;

namespace ProjectK.Notebook.ViewModels.Enums
{
    public enum ReportTypes
    {
        [Description("Worksheet")] Worksheet,
        [Description("Notes")] Notes,
        [Description("Rich Text")] RichText
    }
}