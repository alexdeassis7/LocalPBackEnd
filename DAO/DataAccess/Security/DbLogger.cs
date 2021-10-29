using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAO.DataAccess.Security
{
    public static class DbLogger
    {
        public static void SetDbLog(string message)
        {
            try
            {
                SqlConnection _conn;
                SqlCommand _cmd;
                SqlParameter _prm;
                SqlDataAdapter _da;
                DataSet _ds;
                DataTable _dt;

                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Log].[FillSystemLog]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _prm = new SqlParameter("@Message", SqlDbType.VarChar) { Value = message };
                _cmd.Parameters.Add(_prm);

                _conn.Open();
                _cmd.ExecuteNonQuery();
                _conn.Close();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
