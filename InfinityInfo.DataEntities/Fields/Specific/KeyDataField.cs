using System;

namespace InfinityInfo.DataEntities
{
    [Serializable]
    public sealed class KeyDataField : DataField
    {
        public KeyDataField()
        {
            _isPrimaryKey = true;
        }

        public KeyDataField(String tableName, String fieldName)
            : base(tableName, fieldName) { _isPrimaryKey = true; }

        public override bool SetToNull
        {
            get
            {
                return base.SetToNull;
            }
            set
            {
                throw new InvalidOperationException("KeyDataField: " + this.ToString() + " cannot be set to null.");
            }
        }

        public new String Value
        {
            get
            {
                return (String)base.Value;
            }
            set
            {
                base.Value = value;
            }
        }    
    }
}
