using System;
using System.Collections.Generic;

namespace InfinityInfo.DataEntities
{
    public abstract class EntityQueryBase : IDisposable, ICollection<DataField>
    {
        private QueryExecutionMethod _executionMethod;
        public QueryExecutionMethod ExecutionMethod
        {
            get { return _executionMethod; }
            set { _executionMethod = value; }
        }

        private String _tableName = null;
        public String TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        protected List<DataField> FieldList = new List<DataField>();

        public void Dispose()
        {
            FieldList.Clear();
            FieldList = null;
        }

        public virtual void AddRange(IEnumerable<DataField> items)
        {
            foreach (DataField item in items) { Add(item); }
        }

        public void Add(DataField item)
        {
            if (this._executionMethod == QueryExecutionMethod.Select)
            {
                FieldList.Add(item);
            }
            else
            {
                if (item.IsPrimaryKey)
                {
                    if (item.Value == null && AutogeneratePrimaryKeys) { item.Value = DatabaseConnection.GetIDFor(item.OriginalTableName); }
                    FieldList.Add(item);
                }
                else
                {
                    if (item.SetToNull)
                    {
                        item.Value = null;
                        FieldList.Add(item);
                    }
                    else
                    {
                        // Do not consider dates that are set to DateTime.MinValue
                        if (item.DataType.Equals(typeof(DateTime)))
                        {
                            if (item.Value != null)
                            {
                                DateTime d = (DateTime)item.Value;
                                if (!d.Equals(DateTime.MinValue)) { FieldList.Add(item); }
                            }
                        }
                        else
                        {
                            if (item.Value != null)
                            {
                                FieldList.Add(item);
                            }
                        }
                    }
                }
            }
            if (_tableName == null) { _tableName = item.TableName; }
        }

        public void Clear()
        {
            FieldList.Clear();
            _tableName = null;
        }

        public bool Contains(DataField item)
        {
            return FieldList.Contains(item);
        }

        public void CopyTo(DataField[] array, int arrayIndex)
        {
            FieldList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return FieldList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DataField item)
        {
            return FieldList.Remove(item);
        }

        public IEnumerator<DataField> GetEnumerator()
        {
            return ((ICollection<DataField>)FieldList).GetEnumerator();
        }

        #region IEnumerable Members
        /// <summary>
        /// IEnumerable.GetEnumerator() implementation.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)FieldList).GetEnumerator();
        }

        #endregion

        protected Boolean AutogeneratePrimaryKeys = true;
    }
}
