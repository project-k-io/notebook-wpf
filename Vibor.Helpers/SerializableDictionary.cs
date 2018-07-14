// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.SerializableDictionary`2
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Vibor.Helpers
{
  [XmlRoot("Dictionary")]
  [Serializable]
  public class SerializableDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IXmlSerializable
  {
    private static readonly ILog Log = XLogger.GetLogger();

    public XmlSchema GetSchema()
    {
      return (XmlSchema) null;
    }

    public void ReadXml(XmlReader reader)
    {
      try
      {
        XmlSerializer fromType1 = XmlSerializer.FromTypes(new Type[1]
        {
          typeof (TKey)
        })[0];
        XmlSerializer fromType2 = XmlSerializer.FromTypes(new Type[1]
        {
          typeof (TValue)
        })[0];
        bool isEmptyElement = reader.IsEmptyElement;
        reader.Read();
        if (isEmptyElement)
          return;
        while (reader.NodeType != XmlNodeType.EndElement)
        {
          try
          {
            reader.ReadStartElement("item");
            reader.ReadStartElement("key");
            TKey key = (TKey) fromType1.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadStartElement("value");
            TValue obj = (TValue) fromType2.Deserialize(reader);
            reader.ReadEndElement();
            this.TryAdd(key, obj);
            reader.ReadEndElement();
            int content = (int) reader.MoveToContent();
          }
          catch (Exception ex)
          {
            SerializableDictionary<TKey, TValue>.Log.Error(ex);
            break;
          }
        }
        reader.ReadEndElement();
      }
      catch (Exception ex)
      {
        SerializableDictionary<TKey, TValue>.Log.Error(ex);
      }
    }

    public void WriteXml(XmlWriter writer)
    {
      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add("", "");
      XmlSerializer fromType1 = XmlSerializer.FromTypes(new Type[1]
      {
        typeof (TKey)
      })[0];
      XmlSerializer fromType2 = XmlSerializer.FromTypes(new Type[1]
      {
        typeof (TValue)
      })[0];
      foreach (TKey key in (IEnumerable<TKey>) this.Keys)
      {
        writer.WriteStartElement("item");
        writer.WriteStartElement("key");
        fromType1.Serialize(writer, (object) key);
        writer.WriteEndElement();
        writer.WriteStartElement("value");
        TValue obj = this[key];
        fromType2.Serialize(writer, (object) obj, namespaces);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }
  }
}
