using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using InfinityInfo.DataEntities.BusinessRules;
using InfinityInfo.DataEntities.Entities;

namespace InfinityInfo.DataEntities
{
    /// <summary>
    /// Represents a field in Saleslogix Table, supports ISLXComparable, ISLXSortable, ICloneable
    /// </summary>
    [Serializable]
    public class DataField : DataFieldBase, IQueryComparable, ICloneable, IQuerySortable, IXmlSerializable
    {
        #region Field Changed Event
        /// <summary>
        /// This is an event that is attached to the Field's value property. 
        /// </summary>
        public event EventHandler<FieldChangedEventArgs> ValueChanged;
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Initializes an instance of the SLXDataField class.
        /// </summary>
        public DataField() : base() { }

        /// <summary>
        /// Initializes an instance of the SLXDataField class.
        /// </summary>
        /// <param name="tableName">Saleslogix Tablename</param>
        /// <param name="fieldName">Saleslogix Fieldname</param>
        public DataField(String tableName, String fieldName)
            : base(tableName, fieldName) { }

        /// <summary>
        /// Initializes an instance of the SLXDataField class.
        /// </summary>
        /// <param name="tableName">Saleslogix TableName</param>
        /// <param name="fieldName">Saleslogix Fieldname</param>
        /// <param name="dataType">CLR Type represented by the underlying datavalue.</param>
        public DataField(String tableName, String fieldName, Type dataType)
            : base(tableName, fieldName) { DataType = dataType; }

        #endregion

        #region Properties
        private object _dataValue;

        /// <summary>
        /// Gets or sets the value of the DataField as object[]
        /// </summary>
        public virtual object[] Values
        {
            get
            {
                if (_dataValue != null && _dataValue.GetType().IsArray)
                {
                    Array array = (Array)_dataValue;
                    List<object> values = new List<object>();
                    foreach (object o in array) values.Add(o);
                    return values.ToArray();
                }
                else
                {
                    return new object[] { _dataValue };
                }
            }

            set
            {
                if (value == null)
                {
                    _isMultivalue = false;
                    _fieldIsSetToNull = true;
                }
                Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the DataField as object. 
        /// Note: if value.GetType().IsArray evaluates to true the value will be stored as an array.
        /// </summary>
        public virtual object Value
        {
            get 
            {
                if (this.IsMultivalue)
                {
                    Array array = (Array)_dataValue;
                    List<object> values = new List<object>();
                    foreach (object o in array) values.Add(o);

                    return values.ToArray()[0];
                }
                else
                {
                    return _dataValue;
                }
            }
            set 
            {
                if (value != null)
                {
                    if (value.GetType().IsArray)
                    { 
                        _isMultivalue = true; 
                    }
                    else
                    {
                        _isMultivalue = false;
                    }
                }
 
                if (ValueChanged != null)
                {
                    FieldChangedEventArgs args = new FieldChangedEventArgs();
                    args.BeforeValue = _dataValue;
                    _dataValue = value;
                    ValueChanged(this, args);
                }
                else
                {
                    _dataValue = value;
                }                
            }
        }

        private Boolean _fieldIsSetToNull;
        /// <summary>
        /// This allows you to flag a SLXField as explicitly null.  The need for this come from the fact that 
        /// unset field values are null and are excluded from queries, but if you need to retrieve data where 
        /// a particular field is null or update a specific field to be NULL in the database set this flag to true.
        /// </summary>
        public virtual Boolean SetToNull
        {
            get { return _fieldIsSetToNull; }
            set { _fieldIsSetToNull = value; }
        }

        /// <summary>
        /// Gets the Field's explicit name as it would be represented in 
        /// a SQL statement in the format "TableName.FieldName".  If this field has already
        /// been through query preparation then the TableName property would've been replaced
        /// with the table alias with respect to that statement.
        /// </summary>
        /// <returns></returns>
        public String GetExplicitFieldName()
        {
            return String.Format("{0}.{1}", TableName, FieldName); 
        }

        /// <summary>
        /// Gets the Field's alias as it would be represented in 
        /// a SQL statement in the format "TableName_FieldName".  If this field has already
        /// been through query preparation then the TableName property would've been replaced
        /// with the table alias with respect to that statement.
        /// </summary>
        /// <returns></returns>
        public String GetFieldAlias()
        {
            return String.Format("{0}_{1}", TableName, FieldName);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// This returns the string representation of a SLXFieldComparison object, 
        /// as it would be used in a SQL Statement.
        /// </summary>
        /// <param name="comparator"></param>
        /// <returns></returns>
        public static String GetComparatorString(DataFieldComparison comparator)
        {
            // Equals = 1, NotEquals = 2, Like = 4, LessThan = 8, GreaterThan = 16
            Int32 value = (Int32)comparator;
            switch (value)
            {
                case 1:
                    return " = ";
                case 2:
                    return " <> ";
                case 4:
                    return " LIKE ";
                case 8:
                    return " < ";
                case 16:
                    return " > ";
                case 17:
                    return " >= ";
                case 9:
                    return " <= ";
                default:
                    throw new ArgumentException("Invalid combination of SLXFieldComparison");
            }
        }
        #endregion

        #region ISLXComparable Members
        private DataFieldComparison _comparator = DataFieldComparison.Equals;
        private Boolean _invertComparison = false;
        private Boolean _caseInsensitive = false;
        private Boolean _isMultivalue = false;

        /// <summary>
        /// Used in Select Statements, this is the comparison used to compare the field data to the Value(s) property.
        /// </summary>
        [System.Xml.Serialization.XmlElement("Comparator")]
        public DataFieldComparison Comparator
        {
            get { return _comparator; }
            set { _comparator = value; }
        }

        /// <summary>
        /// Used in Select Statements, this allows you to invert the entire condition as depict in the field definition.
        /// See ISLXComparable definition for more detail.
        /// </summary>
        [System.Xml.Serialization.XmlElement("InvertComparison")]
        public Boolean InvertComparison
        {
            get { return _invertComparison; }
            set { _invertComparison = value; }
        }

        /// <summary>
        /// This makes comparisons case-insensitive by comparing the values in Uppercase
        /// </summary>
        [System.Xml.Serialization.XmlElement("CaseInsensitive")]
        public Boolean CaseInsensitive
        {
            get { return _caseInsensitive; }
            set { _caseInsensitive = value; }
        }

        /// <summary>
        /// Flag that determines whether a DataField has multiple values.  There is not reason 
        /// to set this field, it is maintained by the Value and Value(s) properties.
        /// </summary>
        [System.Xml.Serialization.XmlElement("IsMultivalue")]
        public Boolean IsMultivalue
        {
            get { return _isMultivalue; }
            set { _isMultivalue = value; }
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones the SLXDataField as the original instantiated type through reflection.
        /// </summary>
        /// <returns>cloned SLXDataField object instance.</returns>
        public virtual object Clone()
        {
            Type fieldType = GetType();
            System.Reflection.ConstructorInfo constructor = fieldType.GetConstructor(new Type[] { typeof(string), typeof(string) });
            DataField fld = (DataField)constructor.Invoke(new object[] { OriginalTableName, FieldName });
            
            fld.CaseInsensitive = CaseInsensitive;
            fld.Comparator = Comparator;
            fld.DataType = DataType;
            fld.Value = Value;
            fld.SetToNull = SetToNull;
            fld.InvertComparison = InvertComparison;
            fld.IsMultivalue = IsMultivalue;
            
            return fld;
        }

        #endregion

        #region ISLXSortable Members

        private int _sortIndex = int.MinValue;
        /// <summary>
        /// This is a sort index that, if set to an integer greater than zero will include this 
        /// SLXDataField as a sort criteria for an Select Statements that it is included in.  
        /// Select Statements are created using the SLXEntityFactory.
        /// </summary>
        [System.Xml.Serialization.XmlElement("SortIndex")]
        public int SortIndex
        {
            get
            {
                return _sortIndex;
            }
            set
            {
                _sortIndex = value;
            }
        }

        /// <summary>
        /// Gets the SQL string snippet that represents this SLXDataField's sort expression.
        /// </summary>
        /// <returns></returns>
        public string GetSortExpression()
        {
            return String.Format("{0} {1}", this.GetExplicitFieldName(), (_sortDir == QuerySortDirection.Descending) ? "DESC" : "ASC"); 
        }

        private QuerySortDirection _sortDir = QuerySortDirection.Ascending;
        /// <summary>
        /// This is the Sort Direction of the SLXDataField Sort Expression (ascending or descending)
        /// </summary>
        [System.Xml.Serialization.XmlElement("SortDirection")]
        public QuerySortDirection SortDirection
        {
            get { return _sortDir; }
            set { _sortDir = value; }
        }
        
        #endregion   

        #region Constraints

        private DataFieldConstraintsCollection _constraints = new DataFieldConstraintsCollection();

        /// <summary>
        /// This is the set of Field Constraints that will be validated when the entity that this 
        /// field is associated to posts. For more information on this see SLXFieldConstraint class.
        /// </summary>
        public DataFieldConstraintsCollection GetFieldConstraints()
        {
            return _constraints;
        }

        /// <summary>
        /// This is the method to add a FieldConstraint to the DataField.
        /// </summary>
        /// <param name="constraint"></param>
        public void AddFieldConstraint(DataFieldConstraint constraint)
        {
            if (constraint.Validator == null) { throw new ArgumentException("Constraint validator must be set to an instance of an object."); }
            _constraints.Add(constraint);
        }

        #endregion

        #region IXmlSerializable Members
        /// <summary>
        /// This always returns null.
        /// </summary>
        /// <returns>This always returns null.</returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }
        /// <summary>
        /// Generates the object from its Xml Representation.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            TableName = reader.GetAttribute("TableName");
            FieldName = reader.GetAttribute("FieldName");
            reader.ReadStartElement(this.GetType().FullName);

            while (reader.NodeType == XmlNodeType.Whitespace) { reader.Read(); }
            _caseInsensitive = reader.GetAttribute("CaseInsensitive").Equals("True") ? true : false;
            _comparator = (DataFieldComparison)Enum.Parse(typeof(DataFieldComparison), reader.GetAttribute("Comparator"));
            _invertComparison = reader.GetAttribute("InvertComparison").Equals("True") ? true : false;
            reader.ReadStartElement("Comparable");

            while (reader.NodeType == XmlNodeType.Whitespace) { reader.Read(); }
            _sortDir = (QuerySortDirection)Enum.Parse(typeof(QuerySortDirection), reader.GetAttribute("Direction"));
            string sortIndex = reader.GetAttribute("Index");
            Int32 sortIndexNum;
            if (Int32.TryParse(sortIndex, out sortIndexNum)) { _sortIndex = sortIndexNum; }
            reader.ReadStartElement("Sorting");

            while (reader.NodeType == XmlNodeType.Whitespace) { reader.Read(); }
            _isMultivalue = reader.GetAttribute("IsMultiValue").Equals("True") ? true : false;
            _fieldIsSetToNull = reader.GetAttribute("IsSetToNull").Equals("True") ? true : false;

            string dataType = reader.GetAttribute("DataType");

            reader.ReadStartElement("DataValue");

            List<object> typedValues = ReadXmlExtractTypedValues(reader, dataType);

            if (typedValues.Count > 0)
            {
                _dataValue = (typedValues.Count > 1) ? typedValues.ToArray() : typedValues.ToArray()[0];

            }

            reader.ReadEndElement(); // DataValue
            reader.ReadEndElement(); // this.GetType().Name
        }

        private List<object> ReadXmlExtractTypedValues(System.Xml.XmlReader reader, string dataType)
        {
            List<object> typedValues = new List<object>();

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Value");
                if (reader.NodeType == System.Xml.XmlNodeType.Text)
                {
                    if (dataType.Equals("System.DateTime"))
                    {
                        string untypedValue = reader.ReadContentAsString();
                        DateTime d1;
                        if (DateTime.TryParse(untypedValue, out d1)) { typedValues.Add(d1); }
                    }
                    else
                    {
                        if (dataType.Equals("System.String"))
                        {
                            typedValues.Add(reader.ReadContentAsString());
                        }
                        else
                        {
                            Type t1 = Type.GetType(dataType);
                            System.Reflection.ConstructorInfo c1 = t1.GetConstructor(System.Type.EmptyTypes);
                            object o1;
                            if (c1 != null) { o1 = c1.Invoke(new object[] { }); }
                            o1 = reader.ReadContentAs(t1, null);
                            typedValues.Add(o1);
                        }

                    }
                    reader.ReadEndElement();
                }

                if (reader.Name == "Value" && reader.NodeType == System.Xml.XmlNodeType.EndElement)
                {
                    reader.ReadEndElement();
                }

                
            }
            return typedValues;
        }

        /// <summary>
        /// Converts an object into its Xml Representation.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartAttribute("TableName");
            writer.WriteString(TableName);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("FieldName");
            writer.WriteString(FieldName);
            writer.WriteEndAttribute();

            writer.WriteStartElement("Comparable");
            writer.WriteStartAttribute("CaseInsensitive");
            writer.WriteString(_caseInsensitive.ToString());
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("Comparator");
            writer.WriteString(_comparator.ToString());
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("InvertComparison");
            writer.WriteString(_invertComparison.ToString());
            writer.WriteEndAttribute();
            writer.WriteEndElement();

            writer.WriteStartElement("Sorting");
            writer.WriteStartAttribute("Direction");
            writer.WriteString(_sortDir.ToString());
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("Index");
            writer.WriteString(_sortIndex.ToString());
            writer.WriteEndAttribute();
            writer.WriteEndElement();

            writer.WriteStartElement("DataValue");
            writer.WriteStartAttribute("IsMultiValue");
            writer.WriteString(this._isMultivalue.ToString());
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("IsPrimaryKey");
            writer.WriteString(this.IsPrimaryKey.ToString());
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("IsSetToNull");
            writer.WriteString(this._fieldIsSetToNull.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("DataType");
            writer.WriteString(this.DataType.FullName);
            writer.WriteEndAttribute();

            foreach (object o in Values)
            {
                writer.WriteStartElement("Value");
                writer.WriteString(String.Format("{0}", o));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
        #endregion

        #region Primary Key Identification

        /// <summary>
        /// Protected Boolean flag to signify that this field is a primary key.
        /// If this fields value is null when a post occurs, an ID will be generated based on the Tablename.
        /// </summary>
        protected Boolean _isPrimaryKey = false;

        /// <summary>
        /// Boolean flag to signify that this field is a primary key.
        /// If this fields value is null when a post occurs, an ID will be generated based on the Tablename.
        /// </summary>
        public Boolean IsPrimaryKey
        {
            get { return _isPrimaryKey; }
        }

        #endregion

        #region ParentEntity
        private DataEntityBase _parent = null;
        /// <summary>
        /// Entity that this DataField is associated to.
        /// </summary>
        public DataEntityBase ParentEntity
        {
            get { return _parent; }
            set { _parent = value; }
        }
        #endregion
    }
}
