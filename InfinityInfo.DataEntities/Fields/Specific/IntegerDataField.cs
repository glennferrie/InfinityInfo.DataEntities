using System;

namespace InfinityInfo.DataEntities
{
    [Serializable]
    public class IntegerDataField : DataField
    {
        #region Constructor(s)
        public IntegerDataField() : base() { DataType = typeof(Int32); }

        public IntegerDataField(String tableName, String fieldName)
            : base(tableName, fieldName, typeof(Int32)) { }
        #endregion

        public new Int32 Value
        {
            get { return (Int32)base.Value; }
            set { base.Value = value; }
        }
        
        [System.Xml.Serialization.XmlElement("TypedValues")]
        public new Int32[] Values
        {
            get
            {
                if (IsMultivalue)
                {
                    return (Int32[])base.Value;
                }
                else
                {
                    return new Int32[] { (Int32)base.Value };
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
