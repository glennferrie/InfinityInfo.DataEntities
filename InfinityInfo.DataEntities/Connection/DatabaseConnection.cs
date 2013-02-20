using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace InfinityInfo.DataEntities
{
    /// <summary>
    /// Static class that provides the Saleslogix Connection from the configuration file and is used to generate new IDs.
    /// </summary>
    public static class DatabaseConnection
    {
        /// <summary>
        /// This is a OleDb connectionstring to the Saleslogix Database.  This is used for all posts and retrievals.
        /// It pulls connection from the ConfigurationManager.  It looks for an App setting named "SaleslogixConnection".
        /// </summary>
        public static String ConnectionString
        {
            get
            {
                String connectionString = ConfigurationManager.AppSettings["PrimaryConnection"];
                if (connectionString == null)
                {
                    throw new DatabaseConnectionException("Error Connecting to Saleslogix Database. Catch SaleslogixConnectionException for more detail.", null, "AppSetting 'SaleslogixConnection', not in configuration file");
                }
                return connectionString;
            }

        }

        /// <summary>
        /// Generates a single Saleslogix ID
        /// </summary>
        /// <param name="tableName">Tablename that the ID will be generated for</param>
        /// <returns>a SLX STANDARDID (12 character string)</returns>
        public static String GetIDFor(String tableName)
        {
            String[] ids = GetIDsFor(tableName, 1);

            if (ids != null)
            {
                return ids[0];
            }
            else
            {
                return "GETIDFAILED";
            }
        }
        /// <summary>
        /// Generates a set of Saleslogix IDs
        /// </summary>
        /// <param name="tableName">Tablename that the ID will be generated for</param>
        /// <param name="count">Number of IDs to generate</param>
        /// <returns>an Array of SLX STANDARDID (12 character string)</returns>
        public static String[] GetIDsFor(String tableName, Int32 count)
        {
            return GetIDsFor(tableName, count, ConnectionString);
        }
        /// <summary>
        /// Generates a set of Saleslogix IDs for a given connectionstring
        /// </summary>
        /// <param name="tableName">Tablename that the ID will be generated for</param>
        /// <param name="count">Number of IDs to generate</param>
        /// <param name="connString">Connection string use to generate the IDs</param>
        /// <returns>an Array of SLX STANDARDID (12 character string)</returns>
        public static String[] GetIDsFor(String tableName, Int32 count, string connString)
        {
            List<String> ids = new List<String>();

            using (OleDbConnection cn = new OleDbConnection(connString))
            {
                cn.Open();
                using (OleDbCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = String.Format("slx_dbids('{0}', {1})", tableName, count);
                    cmd.CommandType = CommandType.Text;
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(reader.GetString(0));
                        }
                        reader.Close();
                    }
                }
                cn.Close();
            }

            return ids.ToArray();
        }
    }
}
