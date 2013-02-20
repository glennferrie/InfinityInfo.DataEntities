using System;

namespace InfinityInfo.DataEntities
{
    [Serializable]
    public class DataFieldBase
    {
        public DataFieldBase() {  }

        public DataFieldBase(String tableName, String fieldName)
        {
            _tableName = tableName;
            _fieldName = fieldName;
        }

        #region Properties
        private String _private_tableName = null;
        private String _origTableName = null;
        
        private String _tableName
        {
            get { return _private_tableName; }
            set 
            { 
                _private_tableName = value;
                if (_origTableName == null) { _origTableName = value; }
            }
        }

        [System.Xml.Serialization.XmlElement("OriginalTableName")]
        public String OriginalTableName
        {
            get { return _origTableName; }
        }

        [System.Xml.Serialization.XmlElement("TableName")]
        public String TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        private String _fieldName;

        [System.Xml.Serialization.XmlElement("FieldName")]
        public String FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        private Type _dataType = typeof(String);

        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        #endregion

        #region overridden Object Methods

        public override bool Equals(object obj)
        {
            DataFieldBase field = (DataFieldBase)obj;

            return (field.TableName.Equals(_tableName) && field.FieldName.Equals(_fieldName));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Method(s) : GetOleDbType()
        public virtual System.Data.OleDb.OleDbType GetOleDbType()
        {
            String typeName = this.DataType.Name;
            System.Data.OleDb.OleDbType oleDbType = System.Data.OleDb.OleDbType.VarChar;

            switch (typeName)
            {
                case "String":
                    oleDbType = System.Data.OleDb.OleDbType.VarChar;
                    break;
                case "DateTime":
                    oleDbType = System.Data.OleDb.OleDbType.DBTimeStamp;
                    break;
                case "Int32":
                    oleDbType = System.Data.OleDb.OleDbType.Integer;
                    break;
                case "Double":
                    oleDbType = System.Data.OleDb.OleDbType.Double;
                    break;
                case "Single":
                    oleDbType = System.Data.OleDb.OleDbType.Single;
                    break;
                case "Decimal":
                    oleDbType = System.Data.OleDb.OleDbType.Decimal;
                    break;
                default:
                    throw new NotImplementedException(String.Format("SLXFieldMapping.GetOleDbType() >> Type Not Implemented in conversion: {0} [Mapping: {1}]", typeName, this.ToString()));
            }

            return oleDbType;
        }
        #endregion
    }
}
