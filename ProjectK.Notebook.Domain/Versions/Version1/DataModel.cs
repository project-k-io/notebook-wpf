using System.Xml.Serialization;

namespace ProjectK.Notebook.Domain.Versions.Version1
{
    [XmlRoot("ProjectViewModel")]
    public class DataModel
    {
        public TaskModel RootTask { get; set; }
    }
}