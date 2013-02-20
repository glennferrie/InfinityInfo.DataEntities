using System;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.Xml.Serialization;
using InfinityInfo.DataEntities.BusinessRules;

namespace InfinityInfo.DataEntities.Entities
{
    [Serializable]
    public class DataEntity : DataEntityBase, IPersistible, IDeletable, IXmlSerializable
    {
        public DataEntity() {  }

        public DataEntity(String tableName, String primaryKeyField) : this(tableName, primaryKeyField, null) { }

        public DataEntity(String tableName, String primaryKeyField, String ID)
        {
            base.EntityTableName = tableName;
            base.EntityPrimaryKeyFieldName = primaryKeyField;

            ChildEntities.ParentEntity = this;
            ReferenceEntities.ParentEntity = this;
            FieldMappings.ParentEntity = this;

            InitializeEntity(ID);
        }

        private void InitializeEntity(String ID)
        {
            if (!_initialized)
            {
                #region PrimaryKey Field Definition
                KeyDataField primaryKeyID = new KeyDataField(EntityTableName, EntityPrimaryKeyFieldName);
                if (ID != null) { primaryKeyID.Value = ID; }
                FieldMappings.Add(primaryKeyID);
                #endregion

                _initialized = true;
            }
        }

        [NonSerialized]
        private BusinessRuleCollection _businessRules = new BusinessRuleCollection();

        public BusinessRuleCollection GetBusinessRules()
        {
            return _businessRules;
        }

        public void AddBusinessRule(BusinessRule rule)
        {
            if (rule.Validator == null) { throw new ArgumentException("BusinessRule validator must be set to an instance of an object."); }
            _businessRules.Add(rule);
        }

        public int Save(QueryExecutionMethod method)
        {
            #region Validation

            #region FieldConstraints

            try
            {
                // parent entity : field constraints
                foreach (DataField field in FieldMappings.GetFieldsWithConstraints())
                {
                    foreach (DataFieldConstraint constraint in field.GetFieldConstraints())
                    {
                        if (!constraint.Validator(field)) { throw new DataFieldValidationException(field, constraint); }
                    }
                }

                // child entities : field constraints
                foreach (DataEntity ent in ChildEntities)
                {
                    foreach (DataField field in ent.FieldMappings.GetFieldsWithConstraints())
                    {
                        foreach (DataFieldConstraint constraint in field.GetFieldConstraints())
                        {
                            if (!constraint.Validator(field)) { throw new DataFieldValidationException(field, constraint); }
                        }
                    }
                }
            }
            finally
            {
                // collect and report
            }

            #endregion

            #region BusinessRules

            try
            {
                // parent entity : business rules
                foreach (BusinessRule rule in _businessRules)
                {
                    if (!rule.Validator(this)) { throw new BusinessRuleValidationException(this, rule); }
                }

                // child entities : business rules
                foreach (DataEntity ent in ChildEntities)
                {
                    foreach (BusinessRule rule in ent._businessRules)
                    {
                        if (!rule.Validator(ent)) { throw new BusinessRuleValidationException(ent, rule); }
                    }
                }
            }
            finally
            {
                // collect and report
            }
            #endregion

            #endregion

            #region Post Actions
            String connectionString = this.ActiveConnectionString;
            int affectedRecords = -1;
            if (this.PrimaryKeyID.Value == null) 
            { 
                method = QueryExecutionMethod.Insert;
            }
            EntityQuery[] queries = GetSLXQueries(method);
            using (OleDbConnection cn = new OleDbConnection(connectionString))
            {
                
                try
                {
                    #region Try Open SLX Connection
                    try
                    {
                        cn.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new DatabaseConnectionException("Error Connecting to Saleslogix Database. Catch SaleslogixConnectionException for more detail.", ex.InnerException, connectionString);
                    }
                    #endregion
                    foreach (EntityQuery query in queries)
                    {
                        #region Try Generate Query
                        OleDbCommand cmd = null;
                        try
                        {
                            cmd = query.GetExecutionCommand(cn);
                        }
                        catch (Exception ex)
                        {
                            throw new QueryBuildException("Error generating SQL Command from SLXQuery. Catch SLXQueryBuildException for more detail.", ex, query);
                        }
                        #endregion
                        #region Try Execute Query
                        try
                        {
                            affectedRecords = cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            throw new QueryExecutionException("Error executing generated query. Catch SLXQueryExecutionException for more detail.", ex, cmd);
                        }
                        finally
                        {
                            cmd.Dispose();
                        }
                        #endregion
                    }
                    cn.Close();
                }
                catch (DatabaseConnectionException ex)
                {
                    //if (transaction != null) { transaction.Rollback(); }
                    if (cn != null && cn.State == ConnectionState.Open) { cn.Close(); }
                    throw ex;
                }
                catch (QueryBuildException ex)
                {
                    //if (transaction != null) { transaction.Rollback(); }
                    if (cn != null && cn.State == ConnectionState.Open) { cn.Close(); }
                    throw ex;
                }
                catch (QueryExecutionException ex)
                {
                    //if (transaction != null) { transaction.Rollback(); }
                    if (cn != null && cn.State == ConnectionState.Open) { cn.Close(); }
                    throw ex;
                }
            }
            #endregion

            return affectedRecords;
        }

        public int Save()
        {
            return Save(QueryExecutionMethod.Update);
        }

        #region ISLXDeletable Members

        /// <summary>
        /// This method deletes the entity from the database by Primary Key Field Value.
        /// This method is explicitly implemented so classes of type SLXEntity should be 
        /// casted as ISLXDeletable before this method is accessible.
        /// </summary>
        void IDeletable.Delete()
        {
            #region Post Actions
            String connectionString = this.ActiveConnectionString;
            QueryExecutionMethod method = QueryExecutionMethod.Delete;
            EntityQuery[] queries = this.GetSLXQueries(method);

            OleDbTransaction transaction = null;
            using (OleDbConnection cn = new OleDbConnection(connectionString))
            {
                try
                {
                    #region Try Open SLX Connection
                    try
                    {
                        cn.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new DatabaseConnectionException("Error Connecting to Saleslogix Database. Catch SaleslogixConnectionException for more detail.", ex.InnerException, connectionString);
                    }
                    #endregion

                    #region attempt to start a transaction with a high-isolation (not oracle compliant)
                    try
                    {
                        transaction = cn.BeginTransaction(IsolationLevel.Chaos);
                    }
                    catch (Exception)
                    {
                        transaction = cn.BeginTransaction();
                    }
                    #endregion

                    foreach (EntityQuery query in queries)
                    {
                        #region Try Generate Query
                        OleDbCommand cmd = null;
                        try
                        {
                            cmd = query.GetExecutionCommand(cn);
                        }
                        catch (Exception ex)
                        {
                            throw new QueryBuildException("Error generating SQL Command from SLXQuery. Catch SLXQueryBuildException for more detail.", ex, query);
                        }
                        #endregion

                        #region Try Execute Query
                        try
                        {
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            throw new QueryExecutionException("Error executing generated query. Catch SLXQueryExecutionException for more detail.", ex, cmd);
                        }
                        #endregion
                    }
                    transaction.Commit();
                    cn.Close();
                }
                catch (DatabaseConnectionException ex)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    if (cn != null && cn.State == ConnectionState.Open) { cn.Close(); }
                    throw ex;
                }
                catch (QueryBuildException ex)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    if (cn != null && cn.State == ConnectionState.Open) { cn.Close(); }
                    throw ex;
                }
                catch (QueryExecutionException ex)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    if (cn != null && cn.State == ConnectionState.Open) { cn.Close(); }
                    throw ex;
                }
            }
            #endregion
        }

        #endregion

        private Boolean _initialized = false;

        #region IXmlSerializable Members
        /// <summary>
        /// IXmlSerializable.GetSchema implementation.
        /// </summary>
        /// <returns>Always returns null.</returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }
        /// <summary>
        /// IXmlSerializable.ReadXml implementation.
        /// </summary>
        /// <param name="reader">XmlReader containing serialized information.</param>
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsStartElement(this.GetType().Name)) 
            {
                reader.Read(); 
            }

            reader.ReadStartElement("TableName");
            EntityTableName = reader.ReadString();
            reader.ReadEndElement();

            reader.ReadStartElement("PrimaryKeyFieldName");
            EntityPrimaryKeyFieldName = reader.ReadString();
            reader.ReadEndElement();

            // fields
            if (FieldMappings == null) { FieldMappings = new DataFieldCollection(); }
            FieldMappings.ReadXml(reader);

            // child entities
            if (ChildEntities == null) { ChildEntities = new DataEntityBaseCollection(); }
            ChildEntities.ReadXml(reader);


            // reference entities
            if (ReferenceEntities == null) { ReferenceEntities = new ReferenceEntityCollection(); }
            ReferenceEntities.ReadXml(reader);

            reader.ReadStartElement("Initialized");
            _initialized = (reader.ReadString().Equals("True")) ? true : false;
            reader.ReadEndElement();

            reader.ReadEndElement();
        }
        /// <summary>
        /// IXmlSerializable.WriteXml implementation.
        /// </summary>
        /// <param name="writer">XmlWriter used to output serialization info.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("AQN", this.GetType().AssemblyQualifiedName);

            //this._baseTableName
            writer.WriteStartElement("TableName");
            writer.WriteString(EntityTableName);
            writer.WriteEndElement();

            //this._baseTableIDName
            writer.WriteStartElement("PrimaryKeyFieldName");
            writer.WriteString(EntityPrimaryKeyFieldName);
            writer.WriteEndElement();

            FieldMappings.WriteXml(writer);

            ChildEntities.WriteXml(writer);

            ReferenceEntities.WriteXml(writer);

            writer.WriteStartElement("Initialized");
            writer.WriteString(_initialized.ToString());
            writer.WriteEndElement();
        }

        #endregion
    }
}
