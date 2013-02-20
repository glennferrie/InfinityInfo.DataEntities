using System;
using System.Collections.Generic;

namespace InfinityInfo.DataEntities.Entities
{
    [Serializable]
    public class DataEntityBaseCollection : ICollection<DataEntityBase>, System.Xml.Serialization.IXmlSerializable
    {
        public DataEntityBaseCollection() { }

        private List<DataEntityBase> _entities = new List<DataEntityBase>();

        private DataEntityBase _parent = null;
        public DataEntityBase ParentEntity { get { return _parent; } set { _parent = value; } }

        public DataEntityBase this[String entityTableName]
        {
            get
            {
                foreach (DataEntityBase ent in _entities)
                {
                    if (ent.EntityTableName.Equals(entityTableName)) { return ent; }
                }
                return null;
            }
        }

        public Boolean Contains(String entityTableName)
        {
            foreach (DataEntityBase ent in _entities)
            {
                if (ent.EntityTableName.Equals(entityTableName)) { return true; }
            }
            return false;
        }

        public void Add(DataEntityBase item)
        {
            item.ParentEntity = _parent;
            _entities.Add(item);
        }

        public void Clear()
        {
            _entities.Clear();
        }

        public Boolean Contains(DataEntityBase item)
        {
            foreach (DataEntity ent in _entities)
            {
                if (ent.EntityTableName.Equals(item.EntityTableName)) { return true; }
            }
            return false;
        }

        public void CopyTo(DataEntityBase[] array, int arrayIndex)
        {
            _entities.CopyTo(array, arrayIndex);
        }

        public Int32 Count
        {
            get { return _entities.Count; }
        }

        public Boolean IsReadOnly
        {
            get { return false; }
        }

        public Boolean Remove(DataEntityBase item)
        {
            return _entities.Remove(item);
        }

        public IEnumerator<DataEntityBase> GetEnumerator()
        {
            return ((IEnumerable<DataEntityBase>)_entities).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_entities).GetEnumerator();
        }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _entities.Clear();
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement("ChildEntities");
            }
            else
            {
                reader.ReadStartElement("ChildEntities");

                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    string aqn = reader.GetAttribute("AQN");
                    string typeName = reader.Name;
                    reader.ReadStartElement();
                    Type entityType = Type.GetType(aqn);
                    System.Reflection.ConstructorInfo emptyConstructor = entityType.GetConstructor(Type.EmptyTypes);
                    DataEntity entity = (DataEntity)emptyConstructor.Invoke(new object[] { });
                    entity.ReadXml(reader);
                    _entities.Add(entity);
                    reader.ReadEndElement();
                }

                reader.ReadEndElement();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ChildEntities");

            foreach (DataEntity childEnt in _entities)
            {
                Type childEntType = childEnt.GetType();
                writer.WriteStartElement(childEntType.FullName);
                //Console.WriteLine(childEntType.AssemblyQualifiedName);
                //Console.WriteLine();
                //writer.WriteAttributeString("AQN", childEntType.AssemblyQualifiedName);

                childEnt.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
