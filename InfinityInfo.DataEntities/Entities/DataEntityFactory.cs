using System;
using System.Collections.Generic;
using System.Data.OleDb;
using InfinityInfo.DataEntities.Entities;

namespace InfinityInfo.DataEntities
{
    public class DataEntityFactory
    {
        public DataEntityFactory() {}

        private DataEntity _query;

        public DataEntity QueryDefinition
        {
            get { return _query; }
        }
        
        private Int32 _resultLimit = -1;

        public Int32 ResultLimit
        {
            get { return _resultLimit; }
            set { _resultLimit = value; }
        }

        private bool QueryIsSLXEntity()
        {
            Type t = _query.GetType();
            if (t.Equals(typeof(DataEntity))) { return true; }
            while (t.BaseType != null)
            {
                t = t.BaseType;
                if (t.Equals(typeof(DataEntity))) { return true; }
            }
            return false;
        }

        public DataEntity[] Retrieve(DataEntity query)
        {
            _query = query;
            if (!QueryIsSLXEntity()) { throw new ArgumentException("Query must be of type SLXEntity or derived from SLXEntity.","query"); }

            Type t = _query.GetType();
            System.Reflection.ConstructorInfo constructor = t.GetConstructor(Type.EmptyTypes);

            List<DataEntity> entities = new List<DataEntity>();
            SelectQuery slxquery = new SelectQuery(_query);

            String commandText = slxquery.GetCommandText();
            OleDbParameter[] prms = slxquery.GenerateOleDbParameters();

            using (OleDbConnection cn = new OleDbConnection(_query.ActiveConnectionString))
            {
                cn.Open();
                using (OleDbCommand cmd = cn.CreateCommand())
                {
                    //Console.WriteLine(commandText);
                    cmd.CommandText = commandText;
                    cmd.Parameters.AddRange(prms);

                    Int32 recordCount = 0;

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            DataEntity ent = (DataEntity)constructor.Invoke(new object[] { });
                            //Console.WriteLine(ent.ToString());
                            ent.ActiveConnectionString = _query.ActiveConnectionString;

                            foreach (DataField field in _query.FieldMappings)
                            {
                                Int32 fieldPos = reader.GetOrdinal(field.GetFieldAlias());
                                if (!reader.IsDBNull(fieldPos))
                                {
                                    ent[field.FieldName].Value = reader[fieldPos];
                                }
                            }

                            foreach (DataEntityBase childEnt in _query.ChildEntities)
                            {
                                foreach (DataField field in childEnt.FieldMappings)
                                {
                                    Int32 fieldPos = reader.GetOrdinal(field.GetFieldAlias());
                                    if (!reader.IsDBNull(fieldPos))
                                    {
                                        ent.ChildEntities[childEnt.EntityTableName][field.FieldName].Value = reader[fieldPos];
                                    }
                                }
                            }

                            foreach (ReferenceEntity refEnt in _query.ReferenceEntities)
                            {
                                foreach (DataField field in refEnt.FieldMappings)
                                {
                                    Int32 fieldPos = reader.GetOrdinal(field.GetFieldAlias());
                                    if (!reader.IsDBNull(fieldPos))
                                    {
                                        ent.ReferenceEntities[refEnt.ForeignKeyFieldName][field.FieldName].Value = reader[fieldPos];
                                    }
                                }
                            }
                            entities.Add(ent);
                            recordCount++;
                            if (_resultLimit != -1 && recordCount == _resultLimit) { break; }
                        }
                        reader.Close();
                    }
                }
                cn.Close();
            }
            return entities.ToArray();
        }
    }
}
