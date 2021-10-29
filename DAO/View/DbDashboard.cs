using Newtonsoft.Json;
using SharedModel.Models.View;
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
    public class DbDashboard
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public List<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel> ListLots(SharedModel.Models.View.Dashboard.List.Request data, string customer)
        {
            List<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel> result = new List<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListBatchLot]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel>>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Dashboard.ListTransaction.Response ListTransaction(SharedModel.Models.View.Dashboard.ListTransaction.Request data, string customer)
        {
            SharedModel.Models.View.Dashboard.ListTransaction.Response result = new SharedModel.Models.View.Dashboard.ListTransaction.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListTransactionLot]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
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

        public SharedModel.Models.View.Dashboard.Indicators.Response DashboardIndicators(SharedModel.Models.View.Dashboard.Indicators.Request data, string customer)
        {
            SharedModel.Models.View.Dashboard.Indicators.Response result = new SharedModel.Models.View.Dashboard.Indicators.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[DashboardIndicartors]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.indicators = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Dashboard.MainReport.Response DashboardMainReport(Dashboard.MainReport.Request request, string customer)
        {
            SharedModel.Models.View.Dashboard.MainReport.Response result = new SharedModel.Models.View.Dashboard.MainReport.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetMainReportData]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@type", SqlDbType.NVarChar, 6) { Value = request.type };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.mainReportData = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Dashboard.ClientReport.Response DashboardClientReport(string customer)
        {
            Dashboard.ClientReport.Response result = new Dashboard.ClientReport.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetClientReportData]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.clientReportData = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Dashboard.ProviderCycle.Response DashboardProviderCycleReport(string customer)
        { 
            SharedModel.Models.View.Dashboard.ProviderCycle.Response result = new SharedModel.Models.View.Dashboard.ProviderCycle.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetProviderCycle]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.Data = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Dashboard.MerchantCycle.Response DashboardMerchantCycleReport(string customer)
        {
            SharedModel.Models.View.Dashboard.MerchantCycle.Response result = new SharedModel.Models.View.Dashboard.MerchantCycle.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetMerchantCycle]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.Data = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Dashboard.DollarToday.Response DashboardDollarToday(string customer)
        {
            SharedModel.Models.View.Dashboard.DollarToday.Response result = new SharedModel.Models.View.Dashboard.DollarToday.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetDollarToday]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result.dollarToday = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Dashboard.CashProviderCycle.Response DashboardCashProviderCycle(Dashboard.CashProviderCycle.Request request, string customer)
        {
            SharedModel.Models.View.Dashboard.CashProviderCycle.Response result = new SharedModel.Models.View.Dashboard.CashProviderCycle.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[TransactionProviderCashing]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@idTransaction", SqlDbType.BigInt) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                result.Status = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.View.Dashboard.CashMerchantCycle.Response DashboardCashMerchantCycle(Dashboard.CashMerchantCycle.Request request, string customer)
        {
            SharedModel.Models.View.Dashboard.CashMerchantCycle.Response result = new SharedModel.Models.View.Dashboard.CashMerchantCycle.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[TransactionEntityPayment]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@idEntityUser", SqlDbType.BigInt) { Value = request.idEntityMerchant };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@DateFrom", SqlDbType.VarChar) { Value = request.StartCycle };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@DateTo", SqlDbType.VarChar) { Value = request.EndCycle };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                result.Status = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                result.Status = "ERROR";
                result.StatusMessage = ex.ToString();

            }
            return result;
        }
    }
}
