using System;
using System.Collections.Generic;

namespace InfinityInfo.DataEntities.Entities
{
    [Serializable]
    public sealed class ReferenceEntityCollection : ICollection<ReferenceEntity>, System.Xml.Serialization.IXmlSerializable
    {
        public ReferenceEntityCollection() { }

        private List<ReferenceEntity> _entities = new List<ReferenceEntity>();

        private DataEntityBase _parent = null;
        public DataEntityBase ParentEntity { get { return _parent; } set { _parent = value; } }

        public ReferenceEntity this[String referenceFieldName]
        {
            get
            {
                foreach (ReferenceEntity ent in _entities)
                {
                    if (ent.ForeignKeyFieldName.Equals(referenceFieldName)) { return ent; }
                }
                return null;
            }
        }

        public Boolean Contains(String referenceFieldName)
        {
            foreach (ReferenceEntity ent in _entities)
            {
                if (ent.ForeignKeyFieldName.Equals(referenceFieldName)) { return true; }
            }
            return false;
        }

        public void Add(ReferenceEntity item)
        {
            if (this.Contains(item)) { throw new InvalidOperationException("This Collection already contains an entity with the same referenceFieldName."); }
            item.ParentEntity = _parent;
            _entities.Add(item);
        }

        public void Clear()
        {
            _entities.Clear();
        }

        public Boolean Contains(ReferenceEntity item)
        {
            foreach (ReferenceEntity ent in _entities)
            {
                if (ent.ForeignKeyFieldName.Equals(item.ForeignKeyFieldName)) { return true; }
            }
            return false;
        }

        public void CopyTo(ReferenceEntity[] array, int arrayIndex)
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

        public Boolean Remove(ReferenceEntity item)
        {
            return _entities.Remove(item);
        }

        public IEnumerator<ReferenceEntity> GetEnumerator()
        {
            return ((IEnumerable<ReferenceEntity>)_entities).GetEnumerator();
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
                reader.ReadStartElement("ReferenceEntities");
            }
            else
            {
                reader.ReadStartElement("ReferenceEntities");

                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    string aqn = reader.GetAttribute("AQN");
                    string typeName = reader.Name;
                    reader.ReadStartElement();
                    Type entityType = Type.GetType(aqn);
                    System.Reflection.ConstructorInfo emptyConstructor = entityType.GetConstructor(Type.EmptyTypes);
                    ReferenceEntity entity = (ReferenceEntity)emptyConstructor.Invoke(new object[] { });
                    entity.ReadXml(reader);
                    _entities.Add(entity);
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ReferenceEntities");

            foreach (ReferenceEntity refEnt in _entities) 
            {
                Type refEntType = refEnt.GetType();

                writer.WriteStartElement(refEntType.FullName);
                //Console.WriteLine(refEntType.AssemblyQualifiedName);
                //Console.WriteLine();
                writer.WriteAttributeString("AQN", refEntType.AssemblyQualifiedName);
                refEnt.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
