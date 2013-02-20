using System;
using System.Collections.Generic;

namespace InfinityInfo.DataEntities.BusinessRules
{
    /// <summary>
    /// A collection of Business Rules
    /// </summary>
    public sealed class BusinessRuleCollection : ICollection<BusinessRule>
    {
        /// <summary>
        /// /// Initializes an instance of the SLXBusinessRuleCollection class
        /// </summary>
        public BusinessRuleCollection() {  }

        private List<BusinessRule> _rules = new List<BusinessRule>();

        #region ICollection<SLXBusinessRule> Members
        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(BusinessRule item)
        {
            if (item == null) { throw new ArgumentNullException(); }
            if (item.Validator == null) { throw new ArgumentException("SLXBusinessRule.Validator cannot be null."); }
            _rules.Add(item);
        }
        /// <summary>
        /// The clear method throws and InvalidOperation Exception because you are not allowed to Clear the collection.
        /// </summary>
        public void Clear()
        {
            throw new InvalidOperationException("SLXBusinessRules cannot be cleared. If you would like to change the rules do so in the definition.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(BusinessRule item)
        {
            return _rules.Contains(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(BusinessRule[] array, int arrayIndex)
        {
            throw new InvalidOperationException("Business Rules are not allowed to be copied.");
        }
        /// <summary>
        /// The number of items in the Collection.
        /// </summary>
        public int Count
        {
            get { return _rules.Count; }
        }
        /// <summary>
        /// Property indicating whether the Collection is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Removes an item from the Collection.  This will throw and Invalid operation exception.
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns>Returns true if the item was located and removed.</returns>
        public bool Remove(BusinessRule item)
        {
            throw new InvalidOperationException("SLXBusinessRules cannot be removed. If you would like to change the rules do so in the definition.");
        }

        #endregion

        #region IEnumerable<SLXBusinessRule> Members
        /// <summary>
        /// IEnumerable&lt;SLXBusinessRule&gt;.GetEnumerator implementation
        /// </summary>
        /// <returns>IEnumerator&lt;SLXBusinessRule&gt;</returns>
        public IEnumerator<BusinessRule> GetEnumerator()
        {
            return ((IEnumerable<BusinessRule>)_rules).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// IEnumerable.GetEnumerator implementation
        /// </summary>
        /// <returns>An instance of the System.Collections.IEnumerator class.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_rules).GetEnumerator();
        }

        #endregion
    }
}
