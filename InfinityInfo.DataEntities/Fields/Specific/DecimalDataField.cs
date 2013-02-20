using System;

namespace InfinityInfo.DataEntities
{
    public class DecimalDataField : DataField
    {
        public DecimalDataField() : base() { DataType = typeof(Decimal); }

        public DecimalDataField(String tableName, String fieldName)
            : base(tableName, fieldName, typeof(Decimal)) { }

        public new Decimal Value
        {
            get { return (Decimal)CastWrapper(); }
            set { base.Value = value; }
        }

        private Decimal CastWrapper()
        {
            if (base.Value == null)
            {
                return Decimal.MinValue;
            }
            else
            {
                string originalTypeName = base.Value.GetType().Name;

                switch (originalTypeName)
                {
                    case "Decimal":
                        return (Decimal)base.Value;
                    case "Double":
                        base.Value = new decimal(((double)base.Value));
                        return (Decimal)base.Value;
                    case "Single":
                        base.Value = new decimal(((Single)base.Value));
                        return (Decimal)base.Value;
                    default:
                        throw new NotImplementedException(String.Format("TypeName: {0}, not Implemented in Decimal DataField CastWrapper", originalTypeName));
                }
            }
        }

        private Decimal[] ArrayCastWrapper()
        {
            return (Decimal[])base.Value;
        }

        public new Decimal[] Values
        {
            get
            {
                if (IsMultivalue)
                {
                    return ArrayCastWrapper();
                }
                else
                {
                    return new Decimal[] { CastWrapper() };
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
