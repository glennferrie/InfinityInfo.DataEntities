using System;

namespace InfinityInfo.DataEntities
{
    public class QueryExecutionException : Exception
    {
        private System.Data.OleDb.OleDbCommand _command;

        public System.Data.OleDb.OleDbCommand Command
        {
            get { return _command; }
        }

        public QueryExecutionException(String message, Exception innerException, System.Data.OleDb.OleDbCommand command)
            : base(message, innerException)
        {
            _command = command;
        }
    }

    public class QueryBuildException : Exception
    {
        private EntityQuery _query;

        public EntityQuery Query
        {
            get { return _query; }
        }

        public QueryBuildException(String message, Exception innerException, EntityQuery query)
        {
            _query = query;
        }
    }
}
