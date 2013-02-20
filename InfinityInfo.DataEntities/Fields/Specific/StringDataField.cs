using System;
using InfinityInfo.DataEntities.BusinessRules;

namespace InfinityInfo.DataEntities
{
    [Serializable]
    public class StringDataField : DataField
    {
        #region Constructor(s)
        public StringDataField() : base() { AddMaxLengthConstraint(); }

        public StringDataField(String tableName, String fieldName)
            : base(tableName, fieldName) { AddMaxLengthConstraint(); }

        public StringDataField(String tableName, String fieldName, Int32 maxLength)
            : base(tableName, fieldName) 
        { 
            _maxLength = maxLength;
            AddMaxLengthConstraint();
        }
        #endregion

        private void AddMaxLengthConstraint()
        {
            DataFieldConstraint constraint = new DataFieldConstraint();
            constraint.Description = "String DataField MaxLength Constraint";
            constraint.Source = "InfinityInfo DataEntities Design Team";
            constraint.ValidationMessage = @"
                The length of the value property, must be less that the defined Max Length.";
            constraint.Validator = MaxLengthConstraint;
            this.AddFieldConstraint(constraint);
        }

        private bool MaxLengthConstraint(DataField field)
        {
            StringDataField stringField = (StringDataField)field;
            if (stringField.MaxLength > 0)
            {
                foreach (string val in stringField.Values)
                {
                    if (val == null) { continue; }
                    if (val.Length > stringField.MaxLength) { return false; }
                }
                return true;
            }
            return true;
        }

        public new String[] Values
        {
            get
            {
                if (IsMultivalue)
                {
                    return (String[])base.Value;
                }
                else
                {
                    return new String[] { (String)base.Value };
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

        public new String Value
        {
            get { return (String)base.Value; }
            set { base.Value = value; }
        }

        private Int32 _maxLength = -1;

        [System.Xml.Serialization.XmlElement("MaxLength")]
        public Int32 MaxLength
        {
            get { return _maxLength; }
            set 
            { 
                _maxLength = value;
            }
        }
    }
}
