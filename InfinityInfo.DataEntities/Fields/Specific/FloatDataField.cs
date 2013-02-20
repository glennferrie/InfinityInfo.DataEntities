using System;

namespace InfinityInfo.DataEntities
{
    [Serializable]
    public class FloatDataField : DataField
    {

        public FloatDataField() : base() { DataType = typeof(Double); }

        public FloatDataField(String tableName, String fieldName)
            : base(tableName, fieldName, typeof(Double)) { }

        public new Double Value
        {
            get { return (Double)base.Value; }
            set { base.Value = value; }
        }

        public new Double[] Values
        {
            get
            {
                if (IsMultivalue)
                {
                    return (Double[])base.Value;
                }
                else
                {
                    return new Double[] { (Double)base.Value };
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
