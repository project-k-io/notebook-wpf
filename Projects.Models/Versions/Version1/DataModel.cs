using System.Xml.Serialization;
using Vibor.Helpers;

namespace Projects.Models.Versions.Version1
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