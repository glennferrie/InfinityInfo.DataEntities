using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace InfinityInfo.DataEntities
{
    public class EntityQuery : EntityQueryBase
    {
        public EntityQuery() { }

        #region Command Generation

        public virtual OleDbCommand GetExecutionCommand(OleDbConnection connection)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }

            if (ExecutionMethod == QueryExecutionMethod.Select)
            {
                throw new InvalidOperationException("Use DataEntityFactory to query data.");
            }

            if (connection.State != ConnectionState.Open) { connection.Open(); }

            OleDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = this.GetCommandText();
            cmd.Parameters.AddRange(this.GenerateOleDbParameters());
            
            return cmd;
        }

        public virtual OleDbParameter[] GenerateOleDbParameters()
        {
            OleDbParameter updateStatementIDParameter = null;
            List<OleDbParameter> parameters = new List<OleDbParameter>();
            for (Int32 i = 0; i < FieldList.Count; i++)
            {
                OleDbType paramType = FieldList[i].GetOleDbType();
                OleDbParameter prm = null;

                int maxLength = -1;
                if (FieldList[i].ToString().Contains("StringDataField")) { maxLength = ((StringDataField)FieldList[i]).MaxLength; }


                if (ExecutionMethod == QueryExecutionMethod.Select)
                {
                    if (FieldValueIsSet(FieldList[i]))
                    {
                        if (FieldList[i].IsMultivalue)
                        {
                            object[] values = (object[])FieldList[i].Value;
                            Int32 valueCount = 0;
                            foreach (object value in values)
                            {
                                String fieldName = String.Format("{0}_{1}", FieldList[i].FieldName, ++valueCount);
                                if (paramType == OleDbType.VarChar)
                                {
                                    if (maxLength > 0)
                                    {
                                        prm = new OleDbParameter(fieldName, paramType, maxLength);
                                    }
                                    else
                                    {
                                        prm = new OleDbParameter(fieldName, paramType);
                                    }
                                }
                                else
                                {
                                    prm = new OleDbParameter(fieldName, paramType);
                                }
                                prm.Value = value;
                                parameters.Add(prm);
                            }
                        }
                        else
                        {
                            if (paramType == OleDbType.VarChar)
                            {
                                if (maxLength > 0)
                                {
                                    prm = new OleDbParameter(FieldList[i].FieldName, paramType, maxLength);
                                }
                                else
                                {
                                    prm = new OleDbParameter(FieldList[i].FieldName, paramType);
                                }
                            }
                            else
                            {
                                prm = new OleDbParameter(FieldList[i].FieldName, paramType);
                            }
                            prm.Value = FieldList[i].Value;
                            parameters.Add(prm);
                        }
                    }
                }
                else
                {
                    #region Parameter Definition derived from SLXFieldMapping
                    if (paramType == OleDbType.VarChar)
                    {
                        if (maxLength > 0)
                        {
                            prm = new OleDbParameter(FieldList[i].FieldName, paramType, maxLength);
                        }
                        else
                        {
                            prm = new OleDbParameter(FieldList[i].FieldName, paramType);
                        }
                    }
                    else
                    {
                        prm = new OleDbParameter(FieldList[i].FieldName, paramType);
                    }
                    #endregion

                    #region Set Parameter Value and add to Collection
                    // based on IsPrimaryKey, ExecutionMethod, and whether the value is populated.
                    if (FieldList[i].IsPrimaryKey)
                    {
                        if (this.ExecutionMethod == QueryExecutionMethod.Update)
                        {
                            prm.Value = FieldList[i].Value;
                            updateStatementIDParameter = prm;
                        }
                        else
                        {
                            prm.Value = FieldList[i].Value;
                            parameters.Add(prm);
                        }
                    }
                    else
                    {
                        prm.Value = FieldList[i].Value;
                        parameters.Add(prm);
                    }
                    #endregion
                }
            }

            #region If Statement is an update the ID Parameter is moved to the tail end of the parameters collection
            if (updateStatementIDParameter != null)
            {
                parameters.Add(updateStatementIDParameter);
            }
            #endregion

            return parameters.ToArray();
        }

        public virtual String GetCommandText()
        {
            if (FieldList.Count < 1) { throw new InvalidOperationException("EntityQuery.GetCommandText >> No Field Mappings are defined."); }

            String sql = null;
            String basePattern;
            StringBuilder fieldListBuilder = new StringBuilder();
            StringBuilder parameterPlaceHolders = new StringBuilder();
            String whereCondition = "1 = 2"; // evaluate to false if not set 
            String separator = "";
            String conditionSeparator = "";

            if (ExecutionMethod == QueryExecutionMethod.Select) { whereCondition = ""; }

            foreach (DataField field in FieldList)
            {
                switch (ExecutionMethod)
                {
                    case QueryExecutionMethod.Insert:
                        fieldListBuilder.Append(separator);
                        fieldListBuilder.Append(field.FieldName);
                        parameterPlaceHolders.Append(separator);
                        parameterPlaceHolders.Append("?");
                        separator = ", "; // after the first field each add'l field will be appended with a comma.
                        break;
                    case QueryExecutionMethod.Update:
                        if (!field.IsPrimaryKey)
                        {
                            fieldListBuilder.Append(separator);
                            fieldListBuilder.Append(String.Format("{0} = ?", field.FieldName));
                            separator = ", "; // after the first field each add'l field will be appended with a comma.
                        }
                        else
                        {
                            whereCondition = String.Format("{0} = ?", field.FieldName);
                        }
                        break;
                    case QueryExecutionMethod.Delete:
                        fieldListBuilder.Append(String.Format("{0} = ?", field.FieldName));                        
                        break;
                    case QueryExecutionMethod.Select:
                        if (FieldValueIsSet(field))
                        {
                            if (!field.IsMultivalue)
                            {
                                String cond = String.Format("{0} {1} ?", field.GetExplicitFieldName(), DataField.GetComparatorString(field.Comparator));
                                whereCondition += (conditionSeparator + cond);
                                conditionSeparator = " AND ";
                            }
                            else
                            {
                                object[] values = (object[])field.Value;
                                whereCondition += conditionSeparator;
                                whereCondition += "(";
                                String multivalueSeparator = "";

                                foreach (object value in values)
                                {
                                    String cond1 = String.Format("{0} {1} ?", field.GetExplicitFieldName(), DataField.GetComparatorString(field.Comparator));
                                    if (field.InvertComparison)
                                    {
                                        cond1 = String.Format("NOT ({0})", cond1);
                                    }
                                    whereCondition += (multivalueSeparator + cond1);
                                    multivalueSeparator = " OR ";
                                }
                                whereCondition += ")";
                            }
                        }
                        fieldListBuilder.Append(separator);
                        fieldListBuilder.Append(field.GetExplicitFieldName());
                        separator = ", "; // after the first field each add'l field will be appended with a comma.
                        break;
                    default:
                        throw new InvalidOperationException("EntityQuery.GetCommandText() >> Unknown SLXQueryExecutionMethod");
                }

            }

            switch (ExecutionMethod)
            {
                case QueryExecutionMethod.Delete:
                    basePattern = "DELETE FROM {0} WHERE {1} ";
                    sql = String.Format(basePattern, TableName, fieldListBuilder.ToString());
                    break;

                case QueryExecutionMethod.Insert:
                    basePattern = "INSERT INTO {0} ({1}) VALUES ({2}) ";
                    sql = String.Format(basePattern, TableName, fieldListBuilder.ToString(), parameterPlaceHolders.ToString());
                    break;

                case QueryExecutionMethod.Update:
                    basePattern = "UPDATE {0} SET {1} WHERE {2}";
                    sql = String.Format(basePattern, TableName, fieldListBuilder.ToString(), whereCondition);
                    break;

                case QueryExecutionMethod.Select:
                    basePattern = "SELECT {1} FROM {0} WHERE {2}";
                    sql = String.Format(basePattern, TableName, fieldListBuilder.ToString(), whereCondition);
                    break;

                default:
                    throw new InvalidOperationException("EntityQuery.GetCommandText() >> Unknown SLXQueryExecutionMethod");
            }

            return sql;
        }

        #endregion

        #region Utility Methods
        protected Boolean FieldValueIsSet(DataField field)
        {
            if (field.SetToNull) { return true; }
            if (field.Value == null) { return false; }
            if (field.IsMultivalue) { return true; }

            if (field.DataType.Equals(typeof(DateTime)))
            {
                DateTime d = (DateTime)field.Value;
                if (d.Equals(DateTime.MinValue))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return (field.Value != null);
            }
        }
        #endregion
    }
}
