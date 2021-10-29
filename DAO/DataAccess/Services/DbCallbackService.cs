using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DataAccess.Services
{
    public class DbCallbackService
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public string GetCallbackConfiguration(int idEntityUser)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Configuration].[GetCallbackConfiguration]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@idEntityUser", SqlDbType.VarChar) { Value = idEntityUser };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if(_dt.Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    return _dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCountryCodeByTxId(long? transaction_id)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
                connection.Open();
                // truncate table
                string sqlQuery = string.Format("SELECT [C].[ISO3166_1_ALFA003] " +
                                                "FROM LP_Operation.[Transaction][T] " +
                                                "INNER JOIN LP_Entity.EntityUser EU on EU.idEntityUser = T.idEntityUser " +
                                                "INNER JOIN LP_Location.Country C ON C.idCountry = EU.idCountry WHERE idTransaction = {0}", transaction_id);
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0).ToString();
                        }
                    }
                }
            }

            return "";
        }

        public string GetEntityUserByTxId(long? transaction_id)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
                connection.Open();
                // truncate table
                string sqlQuery = string.Format("SELECT [T].[idEntityUser] FROM LP_Operation.[Transaction] T WHERE idTransaction = {0}", transaction_id);
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt64(0).ToString();
                        }
                    }
                }
            }

            return null;
        }
    }
}
