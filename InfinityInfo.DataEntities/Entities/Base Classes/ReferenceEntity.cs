using System;
using System.Xml.Serialization;

namespace InfinityInfo.DataEntities.Entities
{
    [Serializable]
    public class ReferenceEntity : DataEntityBase, IXmlSerializable
    {
        public ReferenceEntity() 
        {
            ReferenceEntities.ParentEntity = this;
            FieldMappings.ParentEntity = this;
        }

        public ReferenceEntity(String tableName, String keyFieldName) : this(tableName, keyFieldName, keyFieldName) {}

        public ReferenceEntity(String tableName, String keyFieldName, String referenceFieldName)
        {
            base.EntityTableName = tableName;
            base.EntityPrimaryKeyFieldName = keyFieldName;
            _foreignKeyFieldName = referenceFieldName;

            ReferenceEntities.ParentEntity = this;
            FieldMappings.ParentEntity = this;

            InitializeEntity(null);
        }

        public void InitializeEntity(String ID)
        {
            if (!_initialized)
            {
                KeyDataField primaryKeyID = new KeyDataField(EntityTableName, EntityPrimaryKeyFieldName);
                if (ID != null) { primaryKeyID.Value = ID; }
                FieldMappings.Add(primaryKeyID);

                _initialized = false;
            }
        }

        private Boolean _initialized = false;

        private String _foreignKeyFieldName;

        [System.Xml.Serialization.XmlElement("ForeignKeyFieldName")]
        public String ForeignKeyFieldName
        {
            get { return _foreignKeyFieldName; }
            set { _foreignKeyFieldName = value; }
        }

        #region IXmlSerializable Members
        /// <summary>
        /// IXmlSerializable.GetSchema implementation.
        /// </summary>
        /// <returns>Always returns null.</returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }
        /// <summary>
        /// IXmlSerializable.ReadXml implementation.
        /// </summary>
        /// <param name="reader">XmlReader containing serialized Reference Entity Information.</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement("TableName");
            EntityTableName = reader.ReadString();
            reader.ReadEndElement();

            reader.ReadStartElement("PrimaryKeyFieldName");
            EntityPrimaryKeyFieldName = reader.ReadString();
            reader.ReadEndElement();

            reader.ReadStartElement("ForeignKeyFieldName");
            _foreignKeyFieldName = reader.ReadString();
            reader.ReadEndElement();

            // fields
            if (FieldMappings == null) { FieldMappings = new DataFieldCollection(); }
            FieldMappings.ReadXml(reader);

            // reference entities
            if (ReferenceEntities == null) { ReferenceEntities = new ReferenceEntityCollection(); }
            ReferenceEntities.ReadXml(reader);

            reader.ReadStartElement("Initialized");
            _initialized = (reader.ReadString().Equals("True")) ? true : false; 
            reader.ReadEndElement();
        }
        /// <summary>
        /// IXmlSerializable.WriteXml implementation.
        /// </summary>
        /// <param name="writer">XmlWriter used to output Reference Entity Serialization info.</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            //_baseTableName
            writer.WriteStartElement("TableName");
            writer.WriteString(EntityTableName);
            writer.WriteEndElement();
            //_baseTableIDName
            writer.WriteStartElement("PrimaryKeyFieldName");
            writer.WriteString(EntityPrimaryKeyFieldName);
            writer.WriteEndElement();
            //_foreignKeyFieldName
            writer.WriteStartElement("ForeignKeyFieldName");
            writer.WriteString(_foreignKeyFieldName);
            writer.WriteEndElement();

            this.FieldMappings.WriteXml(writer);
            
            //this._referenceEntities
            writer.WriteStartElement("ReferenceEntities");
            foreach (ReferenceEntity refEnt in ReferenceEntities)
            {
                refEnt.WriteXml(writer);
            }
            writer.WriteEndElement();

            //this._initialized
            writer.WriteStartElement("Initialized");
            writer.WriteString(_initialized.ToString());
            writer.WriteEndElement();
        }

        #endregion
    }
}
