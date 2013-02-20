namespace InfinityInfo.DataEntities
{
    /// <summary>
    /// Specifies the type of SQL statement that will be generated.
    /// </summary>
    public enum QueryExecutionMethod
    {
        /// <summary>
        /// Insert SQL Statement
        /// </summary>
        Insert = 0, 
        /// <summary>
        /// Update SQL Statement
        /// </summary>
        Update = 1, 
        /// <summary>
        /// Select SQL Statement
        /// </summary>
        Select = 2, 
        /// <summary>
        /// Delete SQL Statement
        /// </summary>
        Delete = 3
    };
}
