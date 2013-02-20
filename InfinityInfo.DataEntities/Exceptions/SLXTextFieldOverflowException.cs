using System;
using System.Collections.Generic;
using System.Text;

namespace InfinityInfo.Saleslogix
{
    public class SLXTextFieldOverflowException : Exception
    {
        public SLXTextFieldOverflowException(SLXDataField field)
            : base("The data in the attached field too large for its destination. For more detail explicitly catch type SLXTextFieldOverflowException.")
        {
            _dataField = field;
        }

        private SLXDataField _dataField;
        public SLXDataField DataField
        {
            get { return _dataField; }
            set { _dataField = value; }
        }

    }
}
