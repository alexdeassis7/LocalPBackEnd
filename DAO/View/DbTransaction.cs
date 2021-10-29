using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.View
{
    public class DbTransaction
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public SharedModel.Models.View.TransactionModel.Close.Response TransactionsExchangeClosed(SharedModel.Models.View.TransactionModel.Close.Request model)
        {
            SharedModel.Models.View.TransactionModel.Close.Response response = new SharedModel.Models.View.TransactionModel.Close.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[TransactionExchangeClosed]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Value", SqlDbType.VarChar) { Value = model.value };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(model) };
                _cmd.Parameters.Add(_prm);

                _conn.Open();
                _cmd.ExecuteNonQuery();
                _conn.Close();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                response.status = "OK";
                response.status_message = "OK";
            }
            catch (Exception ex)
            {
                response.status = "ERROR";
                response.status_message = ex.ToString();
            }

            return response;
        }

        public SharedModel.Models.View.TransactionModel.Transaction.Response TransactionCreate(string customer, SharedModel.Models.View.TransactionModel.Transaction.Response model, bool transactionMechanism)
        {
            try
            {
                SharedModel.Models.View.TransactionModel.Transaction.Response response = model;

                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[CreateGenericTransaction]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _prm = new SqlParameter("@Customer", SqlDbType.VarChar) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@JSON", SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(model) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@TransactionMechanism", SqlDbType.Bit) { Value = transactionMechanism };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                response.status = (bool)_ds.Tables[0].Rows[0][0] ? "OK" : "Error";
                response.status_message = (string)_ds.Tables[0].Rows[0][1];

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SharedModel.Models.View.TransactionModel.TransactionLot.Response> GetTransactionLots(string lotId = null)
        {
            List<SharedModel.Models.View.TransactionModel.TransactionLot.Response> TransactionLots = new List<SharedModel.Models.View.TransactionModel.TransactionLot.Response>();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListTransactionsToNotify]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _prm = new SqlParameter("@lotId", SqlDbType.VarChar) { Value = lotId };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            
                if (!_ds.Tables[0].Rows[0].IsNull(0))
                    TransactionLots = JsonConvert.DeserializeObject<List<SharedModel.Models.View.TransactionModel.TransactionLot.Response>>(_ds.Tables[0].Rows[0][0].ToString());
       
            }
            catch (Exception)
            {

                throw;
            }
            return TransactionLots;
        }
    }
}
