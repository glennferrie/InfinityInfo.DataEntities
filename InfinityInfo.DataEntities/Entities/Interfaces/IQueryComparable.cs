using System;

namespace InfinityInfo.DataEntities.Entities
{
    /// <summary>
    /// Enumeration that represents the comparison types supported by this framework.
    /// </summary>
    public enum DataFieldComparison
    {
        /// <summary>
        /// Data value equals the provided value
        /// </summary>
        Equals = 1, 
        /// <summary>
        /// Data value does not equal the provided value
        /// </summary>
        NotEquals = 2, 
        /// <summary>
        /// Data value is like the provided value, SQL Wildcard characters are not appended.
        /// </summary>
        Like = 4, 
        /// <summary>
        /// Data value is less than the provided value
        /// </summary>
        LessThan = 8, 
        /// <summary>
        /// Data value greater than the provided value
        /// </summary>
        GreaterThan = 16, 
        /// <summary>
        /// Data values are with in the range provided in the Values property.
        /// </summary>
        Range = 32
    }   
        
    /// <summary>
    /// Interface for fields used with SLXSelectQuery to retrieve records from the database.
    /// </summary>
    public interface IQueryComparable
    {
        /// <summary>
        /// Instance of SLXFieldComparison enum used to formulate where clause.
        /// </summary>
        DataFieldComparison Comparator { get; set; }

        /// <summary>
        /// If set to True, field comparison will be tested with the opposite of what was entered.
        /// Ex: Field Like '%Test%' --> Not (Field Like '%Test%')
        /// </summary>
        Boolean InvertComparison { get; set; }

        /// <summary>
        /// Note: This is not Case Sensitive.  It is opposite of that. Used to formulate where clause
        /// </summary>
        Boolean CaseInsensitive { get; set; }

        /// <summary>
        /// This is a field managed by the SLXDataField Value, Values properties and it is a 
        /// flag that signifies whether the field has more than one value.
        /// </summary>
        Boolean IsMultivalue { get; set; }
    }
}
