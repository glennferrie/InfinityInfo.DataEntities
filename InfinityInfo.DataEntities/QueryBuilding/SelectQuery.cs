using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using InfinityInfo.DataEntities.Entities;

namespace InfinityInfo.DataEntities
{
    public sealed class SelectQuery : EntityQuery
    {
        public SelectQuery(DataEntity entity) 
        { 
            ExecutionMethod = QueryExecutionMethod.Select;
            
            AddRange(entity);
            string upperTable = TableName.ToUpper();
            _tableAssociations = String.Format("{0} {1} ", upperTable, _tableAliases[entity.GetHashCode()].Alias);

            RecurseAssociatedEntites(entity);
        }

        private void RecurseAssociatedEntites(DataEntityBase entity)
        {
            foreach (ReferenceEntity refent in entity.ReferenceEntities)
            {
                AddRange(refent);
                _tableAssociations += String.Format("LEFT JOIN {0} {1} ON ({2} = {3}) ",
                    refent.EntityTableName, _tableAliases[refent.GetHashCode()].Alias,
                    entity[refent.ForeignKeyFieldName].GetExplicitFieldName(),
                    refent[refent.EntityPrimaryKeyFieldName].GetExplicitFieldName());
                RecurseAssociatedEntites(refent);
            }
            foreach (DataEntity childent in entity.ChildEntities)
            {
                AddRange(childent);
                _tableAssociations += String.Format("LEFT JOIN {0} {1} ON ({2} = {3}) ",
                    childent.EntityTableName, _tableAliases[childent.GetHashCode()].Alias,
                    entity[entity.EntityPrimaryKeyFieldName].GetExplicitFieldName(),
                    childent[childent.EntityPrimaryKeyFieldName].GetExplicitFieldName());
                RecurseAssociatedEntites(childent);
            }
        }

        public override OleDbParameter[] GenerateOleDbParameters()
        {
            List<OleDbParameter> parameters = new List<OleDbParameter>();

            for (Int32 i = 0; i < FieldList.Count; i++)
            {
                OleDbType paramType = FieldList[i].GetOleDbType();
                OleDbParameter prm = null;

                int maxLength = -1;
                if (FieldList[i].ToString().Equals("StringDataField")) { maxLength = ((StringDataField)FieldList[i]).MaxLength; }

                if (FieldValueIsSet(FieldList[i]))
                {
                    if (FieldList[i].SetToNull)
                    {
                        // do not add parameters for fields that are set to Null. no parameter required.
                    }
                    else
                    {
                        if (FieldList[i].IsMultivalue)
                        {
                            if (FieldList[i].Comparator == DataFieldComparison.Range)
                            {
                                String fieldName = String.Format("{0}_{1}", FieldList[i].GetFieldAlias(), "L");
                                OleDbParameter prmLbound = new OleDbParameter(fieldName, paramType);
                                String fieldName1 = String.Format("{0}_{1}", FieldList[i].GetFieldAlias(), "U");
                                OleDbParameter prmUbound = new OleDbParameter(fieldName1, paramType);

                                switch (FieldList[i].DataType.Name)
                                {
                                    case "String":
                                        prmLbound.Value = ((StringDataField)FieldList[i]).Values[0];
                                        prmUbound.Value = ((StringDataField)FieldList[i]).Values[1];
                                        break;
                                    case "DateTime":
                                        prmLbound.Value = ((DateTimeDataField)FieldList[i]).Values[0];
                                        prmUbound.Value = ((DateTimeDataField)FieldList[i]).Values[1];
                                        break;
                                    case "Int32":
                                        prmLbound.Value = ((IntegerDataField)FieldList[i]).Values[0];
                                        prmUbound.Value = ((IntegerDataField)FieldList[i]).Values[1];
                                        break;
                                    case "Double":
                                        prmLbound.Value = ((FloatDataField)FieldList[i]).Values[0];
                                        prmUbound.Value = ((FloatDataField)FieldList[i]).Values[1];
                                        break;
                                    case "Single":
                                        prmLbound.Value = ((FloatDataField)FieldList[i]).Values[0];
                                        prmUbound.Value = ((FloatDataField)FieldList[i]).Values[1];
                                        break;

                                }

                                parameters.Add(prmLbound);
                                parameters.Add(prmUbound);
                            }
                            else
                            {
                                object[] values = (object[])FieldList[i].Value;
                                Int32 valueCount = 0;
                                foreach (object value in values)
                                {
                                    String fieldName = String.Format("{0}_{1}", FieldList[i].GetFieldAlias(), ++valueCount);
                                    OleDbParameter prmArr = null;
                                    if (paramType == OleDbType.VarChar)
                                    {
                                        if (maxLength > 0)
                                        {
                                            prmArr = new OleDbParameter(fieldName, paramType, maxLength);
                                        }
                                        else
                                        {
                                            prmArr = new OleDbParameter(fieldName, paramType);
                                        }
                                    }
                                    else
                                    {
                                        prmArr = new OleDbParameter(fieldName, paramType);
                                    }
                                    prmArr.Value = value;
                                    parameters.Add(prmArr);
                                }
                            }
                        }
                        else
                        {
                            if (paramType == OleDbType.VarChar)
                            {
                                if (maxLength > 0)
                                {
                                    prm = new OleDbParameter(FieldList[i].GetFieldAlias(), paramType, maxLength);
                                }
                                else
                                {
                                    prm = new OleDbParameter(FieldList[i].GetFieldAlias(), paramType);
                                }
                            }
                            else
                            {
                                prm = new OleDbParameter(FieldList[i].GetFieldAlias(), paramType);
                            }
                            prm.Value = FieldList[i].Value;
                            parameters.Add(prm);
                        }
                    }
                }
                 
            }
            return parameters.ToArray();
        }

        public override OleDbCommand GetExecutionCommand(OleDbConnection connection)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }

            if (connection.State != ConnectionState.Open) { connection.Open(); }

            OleDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = this.GetCommandText();
            cmd.Parameters.AddRange(this.GenerateOleDbParameters());
            
            return cmd;
        }

        public override String GetCommandText()
        {
            if (FieldList.Count < 1) { throw new InvalidOperationException("SLXSelectQuery.GetCommandText >> No Field Mappings are defined."); }

            String sql = null;
            String basePattern;
            StringBuilder fieldListBuilder = new StringBuilder();
            StringBuilder parameterPlaceHolders = new StringBuilder();
            String whereCondition = "";
            String separator = "";
            String conditionSeparator = "";
            SortedDictionary<int, string> sortExpressions = new SortedDictionary<int, string>();
            
 
            foreach(DataField field in FieldList)
            {
                if (FieldValueIsSet(field))
                {
                    if (field.SetToNull)
                    {
                        String cond = String.Format("{0} Is Null", field.GetExplicitFieldName());
                        if (field.InvertComparison)
                        {
                            cond = String.Format("NOT ({0})", cond);
                        }
                        whereCondition += (conditionSeparator + cond);
                    }
                    else
                    {
                        if (!field.IsMultivalue)
                        {
                            String cond = String.Format("{0} {1} ?", field.GetExplicitFieldName(), DataField.GetComparatorString(field.Comparator));
                            if (field.InvertComparison)
                            {
                                cond = String.Format("NOT ({0})", cond);
                            }
                            whereCondition += (conditionSeparator + cond);
                        }
                        else
                        {
                            if (field.Comparator == DataFieldComparison.Range)
                            {
                                whereCondition += conditionSeparator;
                                whereCondition += "(";
                                String condDateRange = String.Format("{0} >= ? AND {0} <= ?", field.GetExplicitFieldName());
                                if (field.InvertComparison)
                                {
                                    condDateRange = String.Format("NOT ({0})", condDateRange);
                                }
                                whereCondition += condDateRange;
                                whereCondition += ")";
                            }
                            else
                            {
                                object[] values = field.Values;
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
                    }
                    conditionSeparator = " AND ";
                }
                fieldListBuilder.Append(separator);
                fieldListBuilder.Append(field.GetExplicitFieldName() + " " + field.GetFieldAlias());
                separator = ", "; // after the first field each add'l field will be appended with a comma.   
                if (field.SortIndex != int.MinValue)
                {
                    while (sortExpressions.ContainsKey(field.SortIndex)) { field.SortIndex++; }
                    sortExpressions.Add(field.SortIndex, field.GetSortExpression());
                }
            }

            if (whereCondition == "")
            {
                whereCondition = "1 = 1 ";
            }

            basePattern = "SELECT {1} FROM {0} WHERE {2}";
            sql = String.Format(basePattern, _tableAssociations, fieldListBuilder.ToString(), whereCondition);
            if (sortExpressions.Count > 0)
            {
                string sortExpression = " ORDER BY ";
                bool isFirst = true;
                foreach(int key in sortExpressions.Keys)
                {
                    if (!isFirst) { sortExpression += ", "; }
                    isFirst = false;
                    sortExpression += sortExpressions[key];
                }
                sql += sortExpression;
            }
            return sql;
        }

        public void AddRange(DataEntityBase entity)
        {
            string upperTable = entity.EntityTableName.ToUpper();
            if (!_tableAliases.ContainsKey(entity.GetHashCode())) 
            {
                TableAlias alias;
                alias.Tablename = upperTable;
                alias.Alias = String.Format("A{0}", ++_tableIndex);
                _tableAliases.Add(entity.GetHashCode(), alias); 
            }

            foreach (DataField field in entity.FieldMappings)
            {
                Add(field);
                field.TableName = _tableAliases[entity.GetHashCode()].Alias;
            }
        }

        public override void AddRange(IEnumerable<DataField> items)
        {
            throw new NotImplementedException("This method is not implemented in SLXSelectQuery. Use 'AddRange(SLXEntityBase entity)' instead.");
        }

        Dictionary<Int32, TableAlias> _tableAliases = new Dictionary<Int32, TableAlias>();
        Int32 _tableIndex = 0;
        String _tableAssociations = String.Empty;
    }

    internal struct TableAlias
    {
        internal string Tablename;
        internal string Alias;
    }
}
