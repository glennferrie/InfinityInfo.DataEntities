using System;

namespace InfinityInfo.DataEntities.Entities
{
    /// <summary>
    /// Used to describe sort direction.
    /// </summary>
    public enum QuerySortDirection 
    { 
        /// <summary>
        /// represents the SQL query sort keyword: ASC
        /// </summary>
        Ascending, 
        /// <summary>
        /// represents the SQL query sort keyword: DESC
        /// </summary>
        Descending 
    };
    
    /// <summary>
    /// interface applied to SLXDataField to allow multiple fields to be combined into a single SQL Sorting expression.
    /// </summary>
    public interface IQuerySortable
    {
        /// <summary>
        /// defaults to Int32.MinValue.  If this is set to anything greater than zero the field is used in the sort expression.
        /// </summary>
        Int32 SortIndex { get; set; }

        /// <summary>
        /// retrieves sort expression compiled based on index and direction
        /// </summary>
        /// <returns>Sort Expression in the form: A1.FIELDNAME ASC, where A1 is the property of the SLXDataField.TableName.</returns>
        String GetSortExpression();

        /// <summary>
        /// Sort direction (ascending or descending), Ascending is default.
        /// </summary>
        QuerySortDirection SortDirection { get; set; }
    }
}
