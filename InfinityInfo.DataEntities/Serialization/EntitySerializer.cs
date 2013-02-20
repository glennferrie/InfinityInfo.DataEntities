using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using InfinityInfo.DataEntities.Entities;

namespace InfinityInfo.DataEntities.Serialization
{
    public class EntitySerializer
    {
        public EntitySerializer() {}
        
        public object Deserialize(XmlDocument entity)
        {
            string aqn = entity.DocumentElement.Attributes["AQN"].InnerText;
            string serializedTypeName = entity.DocumentElement.Name;
            Type serializedType = Type.GetType(aqn);
                        
            if (serializedTypeName.Equals("DataEntityCollection"))
            {
                string entityCountXml = entity.DocumentElement.Attributes["EntityCount"].InnerText;
                int entityCount;

                if (!int.TryParse(entityCountXml, out entityCount))
                {
                    throw new ArgumentException("The XmlDocument passed does not have a valid EntityCount attribute.", "entity");
                }

                Array array = Array.CreateInstance(serializedType, entityCount);
                int position = 0;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    entity.PreserveWhitespace = false;
                    entity.Save(memoryStream);
                    memoryStream.Position = 0;

                    XmlTextReader reader = new XmlTextReader(memoryStream); 
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    
                    reader.Read(); // first read to initialize the reader.
                    if (reader.NodeType == XmlNodeType.XmlDeclaration) { reader.Read(); }

                    reader.ReadStartElement("DataEntityCollection");

                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        DataEntity slxentity = null;
                        XmlSerializer serializer = new XmlSerializer(serializedType);
                        slxentity = (DataEntity)serializer.Deserialize(reader);
                        array.SetValue(slxentity, position++);
                    }

                    reader = null;
                }

                return array;                
            }
            else
            {
                DataEntity slxentity = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    entity.PreserveWhitespace = false;
                    entity.Save(memoryStream);
                    memoryStream.Position = 0;
                    XmlSerializer serializer = new XmlSerializer(serializedType);

                    slxentity = (DataEntity)serializer.Deserialize(memoryStream);
                }
                return slxentity;
            }
        }

        public XmlDocument Serialize(DataEntity entity)
        {
            XmlDocument doc = new XmlDocument();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriter xmlStreamWriter = XmlWriter.Create(memoryStream, null);
                XmlSerializer streamSer = new XmlSerializer(entity.GetType());
                streamSer.Serialize(xmlStreamWriter, entity);
                xmlStreamWriter.Flush();
                xmlStreamWriter.Close();

                memoryStream.Position = 0;

                TextReader reader = new StreamReader(memoryStream);

                doc.LoadXml(reader.ReadToEnd());
            }

            return doc;
        }

        public XmlDocument Serialize(DataEntity[] entities)
        {
            XmlDocument doc = new XmlDocument();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriter xmlStreamWriter = XmlWriter.Create(memoryStream, null);
                xmlStreamWriter.WriteStartElement("SLXEntityCollection");
                xmlStreamWriter.WriteAttributeString("EntityCount", String.Format("{0}", entities.Length));

                foreach (DataEntity entity in entities)
                {
                    XmlSerializer streamSer = new XmlSerializer(entity.GetType());
                    streamSer.Serialize(xmlStreamWriter, entity);
                }

                xmlStreamWriter.WriteEndElement();
                xmlStreamWriter.Flush();
                xmlStreamWriter.Close();

                memoryStream.Position = 0;
                TextReader reader = new StreamReader(memoryStream);
                doc.LoadXml(reader.ReadToEnd());
            }
            // grab entity assemby qualified name for array generation.
            doc.DocumentElement.Attributes.Append((XmlAttribute)doc.DocumentElement.ChildNodes[0].Attributes["AQN"].Clone());
            return doc;
        }
    }
}
