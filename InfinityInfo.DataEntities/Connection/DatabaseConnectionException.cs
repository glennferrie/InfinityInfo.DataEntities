using System;

namespace InfinityInfo.DataEntities
{
    /// <summary>
    /// Exception thrown when connection fails.
    /// </summary>
    public class DatabaseConnectionException : Exception 
    {

        private String _connectionString;
        /// <summary>
        /// Connection string that failed.
        /// </summary>
        public String ConnectionString
        {
            get { return _connectionString; }
        }
        /// <summary>
        /// Initializes an nre instance of SaleslogixConnectionException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="connectionString"></param>
        public DatabaseConnectionException(String message, Exception innerException, String connectionString)
            : base(message, innerException)
        {
            _connectionString = connectionString;
        }
    }
}
