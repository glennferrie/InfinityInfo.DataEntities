using System;

namespace InfinityInfo.DataEntities
{
    [Serializable]
    public class BooleanDataField : DataField
    {
        #region Constructor(s)

        public BooleanDataField() : base() { DataType = typeof(String); }

        public BooleanDataField(String tableName, String fieldName)
            : base(tableName, fieldName) { }
        #endregion

        public new Boolean Value
        {
            get 
            {
                if (base.Value == null)
                {
                    return false;
                }
                else
                {
                    return (Boolean)base.Value;
                }
            }
            set 
            {
                base.Value = value; 
            }
        }

        public new Boolean[] Values
        {
            get
            {
                if (IsMultivalue)
                {
                    return (Boolean[])base.Value;
                }
                else
                {
                    return new Boolean[] { (Boolean)base.Value };
                }
            }

            set
            {
                if (value == null)
                {
                    IsMultivalue = false;
                    SetToNull = true;
                }
                else
                {
                    if (value.GetType().IsArray) { IsMultivalue = true; }
                }
                base.Value = value;
            }
        }
    }
}
