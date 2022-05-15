using System.Windows;
using System.Windows.Controls;

namespace ProjectK.Notebook.Views.DataTemplateSelectors;

public class ItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate NodeTemplate { get; set; }
    public DataTemplate NoteTemplate { get; set; }
    public DataTemplate TaskTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        // Null value can be passed by IDE designer
        if (item == null) return null;

        if (!(item is string text))
            return null;

        return text switch
        {
            "Task" => TaskTemplate,
            "Note" => NoteTemplate,
            _ => NodeTemplate
        };
    }
}