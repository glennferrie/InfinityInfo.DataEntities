using System;

namespace InfinityInfo.DataEntities
{
    public class FieldChangedEventArgs : EventArgs 
    {
        public FieldChangedEventArgs() {}

        private object _beforeValue;

        public object BeforeValue
        {
            get { return _beforeValue; }
            set { _beforeValue = value; }
        }
    }
}
