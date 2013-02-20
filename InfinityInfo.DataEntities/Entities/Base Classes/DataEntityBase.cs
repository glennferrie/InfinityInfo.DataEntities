using System;
using System.Collections.Generic;
using System.Reflection;

namespace InfinityInfo.DataEntities.Entities
{
    /// <summary>
    /// This is the base class for both SLXEntity and SLXReferenceEntity 
    /// and it contains the properties that they share.
    /// </summary>
    [Serializable]
    public abstract class DataEntityBase : ICloneable
    {
        private String _connectionString = null;

        [System.Xml.Serialization.XmlElement("ActiveConnectionString")]
        public String ActiveConnectionString
        {
            set { _connectionString = value; }
            get 
            {
                if (_connectionString != null) { return _connectionString; }
                _connectionString = DatabaseConnection.ConnectionString;
                return _connectionString;
            }
        }

        private DataEntityBaseCollection _childEntities = new DataEntityBaseCollection();
        private ReferenceEntityCollection _referenceEntities = new ReferenceEntityCollection();

        [System.Xml.Serialization.XmlElement("ChildEntities")]
        public DataEntityBaseCollection ChildEntities
        {
            get { return _childEntities; }
            set { _childEntities = value; }
        }

        [System.Xml.Serialization.XmlElement("ReferenceEntities")]
        public ReferenceEntityCollection ReferenceEntities
        {
            get { return _referenceEntities; }
            set { _referenceEntities = value; }
        }

        private String _baseTableName;
        private String _baseTableIDName;

        [System.Xml.Serialization.XmlElement("BaseTableName")]
        public String EntityTableName
        {
            get { return _baseTableName; }
            set { _baseTableName = value; }
        }

        [System.Xml.Serialization.XmlElement("PrimaryKeyFieldName")]
        public String EntityPrimaryKeyFieldName
        {
            get { return _baseTableIDName; }
            set { _baseTableIDName = value; }
        }

        private DataFieldCollection _fields = new DataFieldCollection();
        [System.Xml.Serialization.XmlElement("FieldMappings")]
        public DataFieldCollection FieldMappings
        {
            get { return _fields; }
            set { _fields = value; }
        }

        public DataField this[String fieldName]
        {
            get
            {
                return _fields[fieldName];
            }
            set
            {
                _fields[fieldName] = value; 
            }
        }

        protected virtual EntityQuery[] GetSLXQueries(QueryExecutionMethod method)
        {
            if (method == QueryExecutionMethod.Select)
            {
                throw new InvalidOperationException("SLXQueryExecutionMethod.Select cannot be executed in this fashion.  Please use an SLXEntityFactory.");
            }
            List<EntityQuery> queries = new List<EntityQuery>();
            queries.Add(ConvertToSLXQuery(method));

            GetChildEntityQueries(this, method, queries);

            return queries.ToArray();
        }

        private void GetChildEntityQueries(DataEntityBase entity, QueryExecutionMethod method, List<EntityQuery> queries)
        {
            if (entity.ChildEntities.Count > 0)
            {
                foreach (DataEntity ent in entity.ChildEntities)
                {
                    if (PrimaryKeyID.Value != null) { ent.PrimaryKeyID.Value = PrimaryKeyID.Value; }
                    queries.Add(ent.ConvertToSLXQuery(method));
                    GetChildEntityQueries(ent, method, queries);
                }
            }
        }

        private EntityQuery ConvertToSLXQuery(QueryExecutionMethod method)
        {
            if (method == QueryExecutionMethod.Select)
            {
                throw new InvalidOperationException("SLXQueryExecutionMethod.Select cannot be executed in this fashion.  Please use an SLXEntityFactory.");
            }
            EntityQuery query = new EntityQuery();
            query.ExecutionMethod = method;
            query.AddRange(FieldMappings);
            return query;
        }

        public KeyDataField PrimaryKeyID
        {
            get { return (KeyDataField)_fields[_baseTableIDName]; }
        }

        public virtual object Clone()
        {
            Type myType = this.GetType();
            ConstructorInfo myConstructor = myType.GetConstructor(new Type[] { });
            DataEntity ent = (DataEntity)myConstructor.Invoke(new object[] { });

            // base properties
            ent._connectionString = this._connectionString;
            ent._baseTableIDName = this._baseTableIDName;
            ent._baseTableName = this._baseTableName;
            foreach (DataEntityBase ch in this._childEntities)
            {
                ent._childEntities.Add((DataEntityBase)ch.Clone());
            }
            foreach (DataEntityBase refEnt in this._referenceEntities)
            {
                ent._referenceEntities.Add((ReferenceEntity)refEnt.Clone());
            }
            ent._fields.Clear();
            foreach (DataField field in this._fields)
            {
                ent._fields.Add((DataField)field.Clone());
            }
            return ent;
        }

        #region Parent Entity For ChildEntity Connectivity

        private DataEntityBase _parent = null;
        /// <summary>
        /// This is a reverse reference point to allow childEntities and referenceEntities to have a link
        /// back to their parent entity.
        /// </summary>
        public DataEntityBase ParentEntity
        {
            get { return _parent; }
            set { _parent = value; }
        }

        #endregion
    }
}
