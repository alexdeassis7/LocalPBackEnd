using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.Security;
namespace DAO.View
{
    public class DbReport
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;
        public SharedModel.Models.View.Report.List.Response AccountTransaction(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListAccountTransaction]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 50) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Report.List.Response ListMerchantBalance(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[MerchantReport]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 50) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@returnAsJson", SqlDbType.Int, 50) { Value = 1 };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Report.List.Response ListMerchantProyectedBalance(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[MerchantProyectedAccountBalanceReport]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 50) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@returnAsJson", SqlDbType.Int, 50) { Value = 1 };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Report.List.Response ListActivityReport(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ActivityReport]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 50) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@returnAsJson", SqlDbType.Int, 50) { Value = 1 };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Report.List.Response ListTransaction(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListTransaction]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 50) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public SharedModel.Models.View.Report.List.Response ListTransactionClientDetails(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListTransactionClientsDetails]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Report.List.Response ListHistoricalFx(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListHistoricalFx]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Report.List.Response ListOperationRetention(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListRetentionOperation]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Report.List.Response ListOperationRetentionMonthly(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListRetentionAFIPMonthly]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public SharedModel.Models.View.Report.List.Response List_Merchant_Report(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListReconciliationReport]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public SharedModel.Models.View.Report.List.Response ListTransactionsError(SharedModel.Models.View.Report.List.Request data, string customer_id)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();
            try
            {

                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Log].[ListTransactionsError]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer_id };
                _cmd.Parameters.Add(_prm);


                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                //result = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog>(_ds.Tables[0].Rows[0][0].ToString());
                result.TransactionList = _ds.Tables[0].Rows[0][0].ToString();
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {

                throw;
            }
            return result;


        }

    }
}


