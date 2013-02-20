using System;

namespace InfinityInfo.DataEntities.BusinessRules
{
    /// <summary>
    /// This is a class that represents a value constraint that is applied to an SLXDataField.
    /// </summary>
    public class DataFieldConstraint
    {
        /// <summary>
        /// Initializes a new instance of SLXFieldConstraint.
        /// </summary>
        public DataFieldConstraint() {}

        private string _description = "Unnamed Constraint";
        /// <summary>
        /// This is the description or name of the constraint.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private String _source = "<unknown>";
        /// <summary>
        /// This is the name of the person or group that is responsible for the creation of the constraint.
        /// Though not necessary, if provides a way of assigning ownership.
        /// </summary>
        public String Source
        {
            get { return _source; }
            set { _source = value; }
        }

        private String _validationMessage;
        /// <summary>
        /// This is the validation message that may explain the constraint's purpose and/or the 
        /// proper course of action one should take for the validator to return true.
        /// </summary>
        public String ValidationMessage
        {
            get { return _validationMessage; }
            set { _validationMessage = value; }
        }

        private DataFieldValidator _validator = null;
        /// <summary>
        /// This is a delegate that takes a SLXDataField as the arguement and returns bool to report if the field's state is valid.
        /// It can be executed manually, but it always executed when the Entity's Post is executed.
        /// </summary>
        public DataFieldValidator Validator
        {
            get
            {
                if (_validator == null) { throw new NotImplementedException("SLXFieldConstraint: " + _description + " was defined, but not Implemented."); }
                return _validator;
            }
            set { _validator = value; }
        }
    }
}
