using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Composr.Repository.Sql
{
    internal class QueryExecutor
    {
        public static T ExecuteSingle<T>(string storedprocedure, DynamicParameters parameters, int? offset = 0, int? limit = 10)
        {
            using (IDbConnection conn = ConnectionFactory.CreateConnection())
            {
                return conn.Query<T>(storedprocedure, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
        }

        public static IList<T> ExecuteList<T>(string storedprocedure, DynamicParameters parameters, int? offset = 0, int? limit = 10)
        {
            using (IDbConnection conn = ConnectionFactory.CreateConnection())
            {
                return conn.Query<T>(storedprocedure, parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }
}
