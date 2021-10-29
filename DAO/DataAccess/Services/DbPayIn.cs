using Newtonsoft.Json;
using SharedModel.Models.Services.Payins;
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
    public class DbPayIn
    {
        static readonly object _locker = new object();
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;
        const int DeadlockErrorCode = 1205;

        public DataTable CreatePayin(SharedModel.Models.Services.Payins.PayinModel.Create.Request data, string customer, string countryCode, int retries = 0)
        {
            DataTable result = new DataTable();
            int expirationDays = int.Parse(ConfigurationManager.AppSettings["PAYIN_EXPIRE_EQUALS_OR_AFTER_DAYS"].ToString());
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[Payin_Generic_Entity_Operation_Create]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@expire_days", SqlDbType.Int, 3) { Value = expirationDays };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                //result = JsonConvert.DeserializeObject<SharedModelDTO.Models.LotBatch.LotBatchModel>(_ds.Tables[0].Rows[0][0].ToString());
                result = _ds.Tables[0];

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (SqlException sqlex)
            {
                // Deadlock 
                if (sqlex.Number == DeadlockErrorCode && retries <= int.Parse(ConfigurationManager.AppSettings["Deadlock_MaxRetriesCount"]))
                {
                    System.Threading.Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["Deadlock_DelayForRetryInMS"]));
                    result = CreatePayin(data, customer, countryCode, retries + 1);
                }
                else
                    throw;
            }
            return result;
        }

        public List<string> GetPayinPaymentMethods()
        {
            List<string> paymentMethods = new List<string>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand(@"select [P].[Code] 
                                        from[LP_configuration].[Provider][P]
                                        inner join[LP_Configuration].[ProviderPayWayServices][PPS] on[PPS].[idProvider] = [P].[idProvider]
                                        inner join[LP_Configuration].[PayWayServices][PS] on[PPS].[idPayWayService] = [PS].[idPayWayService]
                                        where[PS].[Code] = 'PAYINBTRA'", _conn);
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                foreach (DataRow row in _ds.Tables[0].Rows)
                {
                    paymentMethods.Add(row[0].ToString());
                }


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return paymentMethods;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PayinModel.List.Response> ListPayinTransactions(object data, string customer, string countryCode)
        {
            List<PayinModel.List.Response> listPayins = new List<PayinModel.List.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[Payin_Generic_Entity_Operation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@countryCode", SqlDbType.NVarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);


                if (!string.IsNullOrEmpty(_ds.Tables[0].Rows[0][0].ToString()))
                    listPayins = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payins.PayinModel.List.Response>>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return listPayins;
        }

        public List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response> ManagePayin(SharedModel.Models.Services.Payins.PayinModel.Manage.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response> result = new List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ManagePayIns]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                result = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
