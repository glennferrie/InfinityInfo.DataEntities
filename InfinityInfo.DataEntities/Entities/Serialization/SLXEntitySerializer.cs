using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using InfinityInfo.Saleslogix.Entities;

namespace InfinityInfo.Saleslogix
{
    public sealed class SLXEntitySerializer
    {
        public SLXEntitySerializer() {}

        public XmlDocument Serialize(IEnumerable<SLXEntity> entity)
        {
            throw new NotImplementedException();
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

            //    writer.Formatting = Formatting.Indented;

            //    writer.WriteStartDocument();
            //    writer.WriteStartElement("SLXEntityDefinition");
            //    entity.Serialize(writer);
            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //    writer.Flush();

            //    stream.Position = 0;

            //    StreamReader reader = new StreamReader(stream);
            //    _doc = new XmlDocument();
            //    _doc.LoadXml(reader.ReadToEnd());
            //    reader.Close();
            //    reader = null;

            //    writer.Close();
            //    writer = null;
            //}           
        }

        public IEnumerable<SLXEntity> Deserialize(XmlDocument entityXml)
        {
            throw new NotImplementedException();
        }

        private void SerializeEntity(SLXEntity entity, XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        private SLXEntity Deserialize(XmlNode serializedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
