using System;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Vibor.Logging;

namespace Vibor.Helpers
{
    [XmlRoot("Dictionary")]
    [Serializable]
    internal class SerializableDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IXmlSerializable
    {
        private static readonly ILog Log = LogManager.GetLogger("SerializableDictionary");

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                var fromType1 = XmlSerializer.FromTypes(new Type[1]
                {
                    typeof(TKey)
                })[0];
                var fromType2 = XmlSerializer.FromTypes(new Type[1]
                {
                    typeof(TValue)
                })[0];
                var isEmptyElement = reader.IsEmptyElement;
                reader.Read();
                if (isEmptyElement) return;

                while (reader.NodeType != XmlNodeType.EndElement)
                    try
                    {
                        reader.ReadStartElement("item");
                        reader.ReadStartElement("key");
                        var key = (TKey) fromType1.Deserialize(reader);
                        reader.ReadEndElement();
                        reader.ReadStartElement("value");
                        var obj = (TValue) fromType2.Deserialize(reader);
                        reader.ReadEndElement();
                        TryAdd(key, obj);
                        reader.ReadEndElement();
                        var content = (int) reader.MoveToContent();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        break;
                    }

                reader.ReadEndElement();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            var fromType1 = XmlSerializer.FromTypes(new Type[1]
            {
                typeof(TKey)
            })[0];
            var fromType2 = XmlSerializer.FromTypes(new Type[1]
            {
                typeof(TValue)
            })[0];
            foreach (var key in Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                fromType1.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                var obj = this[key];
                fromType2.Serialize(writer, obj, namespaces);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }
}