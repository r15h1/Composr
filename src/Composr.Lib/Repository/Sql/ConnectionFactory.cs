using System.Data;

namespace Composr.Repository.Sql
{
    internal static class ConnectionFactory
    {
        /// <summary>
        /// creates a connection using the provided connection string
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <returns></returns>
        public static IDbConnection CreateConnection(string connectionstring)
        {
            return Connect(connectionstring);
        }

        /// <summary>
        /// creates a connection to the underlying database using the default connection string
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <returns></returns>
        public static IDbConnection CreateConnection()
        {
            return Connect(Composr.Lib.Util.Settings.ConnectionString);
        }

        private static IDbConnection Connect(string connectionstring)
        {
            IDbConnection conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            conn.Open();
            return conn;
        }
    }
}