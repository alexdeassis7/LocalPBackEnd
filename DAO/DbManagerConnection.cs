using System;
using System.Configuration;
using System.Data.Linq;
using System.Data.SqlClient;

namespace DAO
{
    public class DbManagerConnection
    {
        #region Singleton
        //private static SqlConnection DbManagerConnectionInstanceSqlConnectionInstance;
        //private static DbManagerConnection DbManagerConnectionInstance;
        //public static DbManagerConnection GetDbManegerConnection
        //{
        //    get
        //    {
        //        if (DbManagerConnectionInstance == null)
        //        {
        //            lock (typeof(DbManagerConnection))
        //            {
        //                if (DbManagerConnectionInstance == null)
        //                {
        //                    DbManagerConnectionInstance = new DbManagerConnection();
        //                }
        //            }
        //        }
        //        return DbManagerConnectionInstance;
        //    }
        //}
        //public static SqlConnection GetDbManagerConnectionSql
        //{
        //    get
        //    {
        //        if (DbManagerConnectionInstanceSqlConnectionInstance == null)
        //        {
        //            lock (typeof(SqlConnection))
        //            {
        //                if (DbManagerConnectionInstanceSqlConnectionInstance == null)
        //                {
        //                    DbManagerConnectionInstanceSqlConnectionInstance = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
        //                }
        //            }
        //        }
        //        return DbManagerConnectionInstanceSqlConnectionInstance;
        //    }
        //}
        #endregion

        #region Constructor
        public DbManagerConnection() { }
        #endregion

        #region MembersContext
        public Interfaces.Security.IDbSecurity DbContextSecurity { get; }
        #endregion
    }
}
