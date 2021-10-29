using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Configuration;
using SharedModel.Models.Security;
namespace DAO.DataAccess.Security
{
    public class DbLog
    {

        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;


        public string InsertTransactionErrors(List<TransactionError.ModelDB> ListTxError, string customer_id,string countryCode, bool TransactionMechanism) {

            var result = "";

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Log].[TransactionsError_Insert]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(ListTxError) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer_id };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 50) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                if (TransactionMechanism == true)
                {
                    _prm = new SqlParameter("@TransactionMechanism", SqlDbType.Bit) { Value = TransactionMechanism };
                    _cmd.Parameters.Add(_prm);
                }
                else
                {
                    _prm = new SqlParameter("@TransactionMechanism", SqlDbType.Bit) { Value = false };
                    _cmd.Parameters.Add(_prm);
                }

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
   

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public void Logout(string customer, bool TransactionMechanism, string countryCode) {

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Security].[LogOut]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Customer", SqlDbType.VarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);
        
                _prm = new SqlParameter("@Country", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                if (TransactionMechanism == true)
                {
                    _prm = new SqlParameter("@idTransactionMechanism", SqlDbType.Bit) { Value = TransactionMechanism };
                    _cmd.Parameters.Add(_prm);
                }
                else
                {
                    _prm = new SqlParameter("@idTransactionMechanism", SqlDbType.Bit) { Value = false };
                    _cmd.Parameters.Add(_prm);
                }

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public void InsertPayinErrors(SharedModel.Models.Services.Payins.PayinModel.RejectedPayins.TransactionError ListTxError, string customer_id, string countryCode)
        {

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Log].[PayinError_Insert]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(ListTxError) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer_id };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 50) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);


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
