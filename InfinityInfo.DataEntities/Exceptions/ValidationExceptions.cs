using System;
using InfinityInfo.DataEntities.BusinessRules;
using InfinityInfo.DataEntities.Entities;

namespace InfinityInfo.DataEntities
{
    public class DataFieldValidationException : Exception
    {
        public DataFieldValidationException(DataField field, DataFieldConstraint constraint)
        {
            _dataField = field;
            _constraint = constraint;
            
        }

        private DataField _dataField;
        public DataField DataField
        {
            get { return _dataField; }
            set { _dataField = value; }
        }

        private DataFieldConstraint _constraint;
        public DataFieldConstraint FailedConstraint
        {
            get { return _constraint; }
            set { _constraint = value; }
        }

        public override string Message
        {
            get
            {
                return String.Format("{0}.{1} failed validation. Constraint: {2}, Desc: {3}", _dataField.OriginalTableName, _dataField.FieldName, _constraint.Description, _constraint.ValidationMessage);
            }
        }
    }

    public class BusinessRuleValidationException : Exception
    {
        public BusinessRuleValidationException(DataEntity entity, BusinessRule rule)
        {
            _entity = entity;
            _bizRule = rule;
        }

        private DataEntity _entity;
 
        public DataEntity Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }

        private BusinessRule _bizRule;

        public BusinessRule FailedBusinessRule
        {
            get { return _bizRule; }
            set { _bizRule = value; }
        }

        public override string Message
        {
            get
            {
                return String.Format("Entity Business Rule ({0}) > {1}: {2}", _entity.GetType().Name, _bizRule.Description, _bizRule.ValidationMessage);
            }
        }
    }
}
