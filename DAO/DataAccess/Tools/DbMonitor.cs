using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DataAccess.Tools
{
    public class DbMonitor
    {
        const string QueryCheck = "select 1";
        const string HealthCheckDbTimeOut = "HealthCheck_DB_TimeOut";
        public bool HealthCheck()
        {
            int Value_Asserted = 1;
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    conn.Open();
                    return Value_Asserted == (int)conn.ExecuteScalar(QueryCheck, commandTimeout: int.Parse(ConfigurationManager.AppSettings[HealthCheckDbTimeOut]));
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
