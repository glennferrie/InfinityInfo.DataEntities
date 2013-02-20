using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using InfinityInfo.DataEntities.BusinessRules;
using InfinityInfo.DataEntities.Entities;

namespace InfinityInfo.DataEntities
{
    [Serializable]
    public class DataFieldCollection : ICollection<DataField>, IXmlSerializable
    {
        private Dictionary<string, DataField> _fields = new Dictionary<string, DataField>();

        public DataFieldCollection() { }

        private DataEntityBase _parent = null;

        public DataEntityBase ParentEntity
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public DataField this[string fieldName]
        {
            get
            {
                if (fieldName == null) { throw new ArgumentNullException("fieldName"); }
                if (_fields.ContainsKey(fieldName))
                {
                    return _fields[fieldName];
                }
                else
                {
                    throw new ArgumentOutOfRangeException("fieldName", "Unknown Fieldname: " + fieldName);
                }
            }
            set
            {
                _fields[fieldName] = value;
            }
        }

        public void Add(DataField item)
        {
            if (IsReadOnly) { throw new InvalidOperationException("Cannot Add item when Collection is Readonly. Set Parent Entity."); }
            item.ParentEntity = _parent;
            _fields.Add(item.FieldName, item);
        }

        public void Clear()
        {
            if (IsReadOnly) { throw new InvalidOperationException("Cannot Add item when Collection is Readonly. Set Parent Entity."); }
            _fields.Clear();
        }

        public bool Contains(DataField item)
        {
            if (_fields.ContainsKey(item.FieldName))
            {
                DataField fld = _fields[item.FieldName];
                if (!fld.OriginalTableName.Equals(item.OriginalTableName)) { return false; }
                return true;
            }
            return false;
        }

        public void CopyTo(DataField[] array, int arrayIndex)
        {
            _fields.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _fields.Keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return (_parent == null); }
        }

        public bool Remove(DataField item)
        {
            if (IsReadOnly) { throw new InvalidOperationException("Cannot Remove item when Collection is Readonly. Set Parent Entity."); }
            return _fields.Remove(item.FieldName);            
        }

        public IEnumerator<DataField> GetEnumerator()
        {
            return ((IEnumerable<DataField>)_fields.Values).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_fields.Values).GetEnumerator();
        }

        public IEnumerable<DataField> GetFieldsWithConstraints()
        {
            foreach (DataField field in _fields.Values)
            {
                bool constraintsExists = false;
                foreach (DataFieldConstraint constraint in field.GetFieldConstraints()) { constraintsExists = true; break; }
                if (constraintsExists) { yield return field; }
            }
        }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            Clear();
            if (reader.IsStartElement("DataFieldCollection"))
            {
                reader.ReadStartElement("DataFieldCollection");
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    while (reader.NodeType == XmlNodeType.Whitespace) { reader.Read(); }
                    Type fieldType = Type.GetType(reader.Name);
                    ConstructorInfo constructor = fieldType.GetConstructor(new Type[] { });
                    DataField fld = (DataField)constructor.Invoke(new object[] { });
                    fld.ReadXml(reader);
                    Add(fld);
                }
                reader.ReadEndElement();
                return;
            }
            throw new InvalidOperationException("Expected DataFieldCollection Node missing.");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("DataFieldCollection");

            foreach (DataField field in _fields.Values)
            {
                string fieldTypeName = field.GetType().FullName;
                writer.WriteStartElement(fieldTypeName);

                field.WriteXml(writer);

                writer.WriteEndElement(); // fieldTypeName
            }

            writer.WriteEndElement();
        }
    }
}
