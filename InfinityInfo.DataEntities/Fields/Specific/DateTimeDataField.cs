using System;

namespace InfinityInfo.DataEntities
{

    [Serializable]
    public class DateTimeDataField : DataField
    {
        public DateTimeDataField() : base() { DataType = typeof(DateTime); }

        public DateTimeDataField(String tableName, String fieldName)
            : base(tableName, fieldName, typeof(DateTime)) { }

        public new DateTime Value
        {
            get
            {
                if (base.Value == null) { base.Value = DateTime.MinValue; }
                try
                {
                    return (DateTime)base.Value;
                }
                catch
                {
                    throw new Exception(base.Value.GetType().Name + " >> " + String.Format("{0}", base.Value));
                }
            }
            set { base.Value = value; }
        }

        public new DateTime[] Values
        {
            get
            {
                if (IsMultivalue)
                {
                    return (DateTime[])base.Value;
                }
                else
                {
                    return new DateTime[] { (DateTime)base.Value };
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
