using System.Xml.Serialization;

namespace ProjectK.Notebook.Models.Versions.Version1;

[XmlRoot("ProjectViewModel")]
public class DataModel
{
    public TaskModel RootTask { get; set; }
}