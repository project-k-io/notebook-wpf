using System.Xml.Serialization;
using ProjectK.Utils;

namespace ProjectK.Notebook.Models.Versions.Version1
{
    [XmlRoot("ProjectViewModel")]
    public class DataModel
    {
        public TaskModel RootTask { get; set; }

        public static DataModel ReadFromFile(string file)
        {
            return XFile2.ReadFromXmlFile<DataModel>(file);
        }
    }
}