using System;
using InfinityInfo.DataEntities.BusinessRules;

namespace InfinityInfo.DataEntities.Entities
{
    public interface IPersistible
    {
        /// <summary>
        /// Method that invokes the entity post.
        /// </summary>
        /// <returns>Number of rows affected.</returns>
        Int32 Save(QueryExecutionMethod method);

        /// <summary>
        /// Gets a Collection of business rules to be applied during the post.
        /// </summary>
        BusinessRuleCollection GetBusinessRules();
    }
}
