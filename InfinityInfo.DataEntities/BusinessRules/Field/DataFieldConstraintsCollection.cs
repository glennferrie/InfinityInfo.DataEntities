using System;
using System.Collections.Generic;

namespace InfinityInfo.DataEntities.BusinessRules
{
    /// <summary>
    /// A collection of Field Constraints
    /// </summary>
    public class DataFieldConstraintsCollection : ICollection<DataFieldConstraint>
    {
        /// <summary>
        /// Initializes an instance of the SLXFieldConstraintsCollection class
        /// </summary>
        public DataFieldConstraintsCollection() {}

        private List<DataFieldConstraint> _constraints = new List<DataFieldConstraint>();

        #region ICollection<SLXFieldConstraint> Members

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(DataFieldConstraint item)
        {
            if (item == null) { throw new ArgumentNullException(); }
            if (item.Validator == null) { throw new ArgumentException("SLXFieldConstraint.Validator cannot be null."); }
            _constraints.Add(item);
        }
        /// <summary>
        /// The clear method throws and InvalidOperation Exception because you are not allowed to Clear the collection.
        /// </summary>
        public void Clear()
        {
            throw new InvalidOperationException("SLXFieldConstraints cannot be cleared. If you would like to change the constraints do so in the definition.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(DataFieldConstraint item)
        {
            return _constraints.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(DataFieldConstraint[] array, int arrayIndex)
        {
            throw new InvalidOperationException("Field Constraints are not allowed to be copied.");
        }
        /// <summary>
        /// The number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return _constraints.Count; }
        }
        /// <summary>
        /// This is a property indicating whether the collection is readonly.
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
        public bool Remove(DataFieldConstraint item)
        {
            throw new InvalidOperationException("SLXFieldConstraints cannot be removed. If you would like to change the constraints do so in the definition.");
        }

        #endregion

        #region IEnumerable<SLXFieldConstraint> Members
        /// <summary>
        /// IEnumerable&lt;SLXBusinessRule&gt;.GetEnumerator implementation
        /// </summary>
        /// <returns>IEnumerator&lt;SLXBusinessRule&gt;</returns>
        public IEnumerator<DataFieldConstraint> GetEnumerator()
        {
            return ((IEnumerable<DataFieldConstraint>)_constraints).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// IEnumerable.GetEnumerator implementation
        /// </summary>
        /// <returns>An instance of the System.Collections.IEnumerator class.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_constraints).GetEnumerator();
        }

        #endregion
    }
}
