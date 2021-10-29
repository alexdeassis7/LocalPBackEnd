using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Aspose.Cells;
using Newtonsoft.Json;
using SharedModel.Models.Services.Brasil;
using static SharedModel.Models.Database.General.BankCodesModel;
using Tools;
using SharedModel.Models.General;
using static SharedModel.Models.Services.Payouts.Payouts;
using Dapper;
using System.Linq;

namespace DAO.DataAccess.Services
{
    public class DbPayOut
    {
        static readonly object _locker = new object();
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;
        const int DeadlockErrorCode = 1205;
        public SharedModelDTO.Models.LotBatch.LotBatchModel CreateLotTransaction(SharedModelDTO.Models.LotBatch.LotBatchModel data, string customer, bool TransactionMechanism, string countryCode, int retries = 0)
        {
            SharedModelDTO.Models.LotBatch.LotBatchModel result = new SharedModelDTO.Models.LotBatch.LotBatchModel();

            try
            { 
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_Generic_Entity_Operation_Create]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
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
                result = JsonConvert.DeserializeObject<SharedModelDTO.Models.LotBatch.LotBatchModel>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (SqlException sqlex)
            {
                // Deadlock 
                if (sqlex.Number == DeadlockErrorCode && retries <= int.Parse(ConfigurationManager.AppSettings["Deadlock_MaxRetriesCount"]))
                {
                    System.Threading.Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["Deadlock_DelayForRetryInMS"]));
                    result = CreateLotTransaction(data, customer, TransactionMechanism, countryCode , retries + 1);
                }
                else
                    throw;
            }
            return result;
        }

        public SharedModelDTO.Models.LotBatch.LotBatchModel UpdateLotTransaction(SharedModelDTO.Models.LotBatch.LotBatchModel data, string customer, bool TransactionMechanism)
        {
            SharedModelDTO.Models.LotBatch.LotBatchModel result = new SharedModelDTO.Models.LotBatch.LotBatchModel();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_Generic_Entity_Operation_Update]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
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
                result = JsonConvert.DeserializeObject<SharedModelDTO.Models.LotBatch.LotBatchModel>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> DeleteLotTransaction(SharedModelDTO.Models.LotBatch.LotBatchModel data, string customer, bool TransactionMechanism)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> result = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_Generic_Entity_Operation_Delete]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
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
                result = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListLotTransactionARG(SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request data, string customer)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListlotBatch = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_Generic_Entity_Operation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                //var LotBach = JsonConvert.SerializeObject(_ds.Tables[0], Formatting.Indented);
                //var Transaction = JsonConvert.SerializeObject(_ds.Tables[1], Formatting.Indented);
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel();
                /* foreach (System.Data.DataRow Rows in _ds.Tables[0].Rows)
                 {*/
                //LotBatch = JsonConvert.DeserializeObject<SharedModelDTO.Models.LotBatch.LotBatchModel>(Rows[0].ToString());
                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());
                //ListlotBatch.Add(LotBatch);
                /*}*/


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public PayOutBrasil.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankBDOBRASIL(bool TransactionMechanism, string JSON)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_BDOB_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;
        }

        //alex
        public PayOutBrasil.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankITAU(bool TransactionMechanism, string JSON)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_ITAU_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;
        }
        public PayOutBrasil.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankSAFRA(bool TransactionMechanism, string JSON)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_SAFRA_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;
        }

        public PayOutBrasil.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankSANTANDER(bool TransactionMechanism, string JSON)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_SANTANDER_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListLotTransactionCOL(SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request data, string customer)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListlotBatch = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_Generic_Entity_Operation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                //var LotBach = JsonConvert.SerializeObject(_ds.Tables[0], Formatting.Indented);
                //var Transaction = JsonConvert.SerializeObject(_ds.Tables[1], Formatting.Indented);
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel();
                /* foreach (System.Data.DataRow Rows in _ds.Tables[0].Rows)
                 {*/
                //LotBatch = JsonConvert.DeserializeObject<SharedModelDTO.Models.LotBatch.LotBatchModel>(Rows[0].ToString());
                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());
                //ListlotBatch.Add(LotBatch);
                /*}*/


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListLotTransactionBRA(SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request data, string customer)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListlotBatch = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_Generic_Entity_Operation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel();

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListLotTransactionURY(SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request data, string customer)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListlotBatch = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[URY_Payout_Generic_Entity_Operation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel();

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }


        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListLotTransactionMEX(SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request data, string customer)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListlotBatch = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();


                _cmd = new SqlCommand("[LP_Operation].[MEX_Payout_Generic_Entity_Operation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel();

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListLotTransactionCHL(SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request data, string customer)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListlotBatch = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();


                _cmd = new SqlCommand("[LP_Operation].[CHL_Payout_Generic_Entity_Operation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel();

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankGALICIA(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_GALICIA_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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


                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankSantanderARG(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_SANTANDER_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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


                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankItauARG(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_ITAU_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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


                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankIcbcARG(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_ICBC_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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


                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankBBVA(bool TransactionMechanism, string JSON)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_BBVA_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            } catch(Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;
        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankBBVATuPago(bool TransactionMechanism, string JSON)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_BBVA_TUPAGO_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;
        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankSUPERVIELLE(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_SUPERVIELLE_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
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

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    result.Rows = _dt.Rows.Count;

                    result.Lines = new string[_dt.Rows.Count];

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        result.Lines[i] = _dt.Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.Rows = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.Rows = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromGALICIA(int CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_GALICIA_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.Int) { Value = CurrencyFxClose };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(uploadModel) };
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


                foreach (DataRow row in _ds.Tables[0].Rows)
                {
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>(row));
                }

                /* SI LA TRANSACCION FUE APROBADA */
                /*  if (DetailStatus == "60")
                  {
                      lock (_locker)
                      {
                          GenerateCertificates(Ticket);
                      }
                  }*/

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBBVA(int CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_BBVA_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(uploadModel) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.Int) { Value = CurrencyFxClose };
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

                /* SI LA TRANSACCION FUE APROBADA */
                /* if (DetailStatus == "I7")
                 {
                     lock (_locker)
                     {
                         GenerateCertificates(Ticket);
                     }
                 }*/

                foreach (DataRow row in _ds.Tables[0].Rows)
                {
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>(row));
                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBBVATuPago(int CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_BBVA_TUPAGO_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(uploadModel) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.Int) { Value = CurrencyFxClose };
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


                foreach (DataRow row in _ds.Tables[0].Rows)
                {
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>(row));
                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromSantander(int CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_SANTANDER_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(uploadModel) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.Int) { Value = CurrencyFxClose };
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


                foreach (DataRow row in _ds.Tables[0].Rows)
                {
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>(row));
                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromSUPERVIELLE(int CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Payout_SUPERVIELLE_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(uploadModel) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.Int) { Value = CurrencyFxClose };
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

                /* SI LA TRANSACCION FUE APROBADA */
                /* if (DetailStatus == "OK")
                 {
                     GenerateCertificates(Ticket);
                 }*/

                foreach (DataRow row in _ds.Tables[0].Rows)
                {
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail>(row));
                }


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog LogPayoutErrors(List<SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog> data, string customer, bool TransactionMechanism, string countryCode)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog result = new SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Log].[PayoutsErrors_Add]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
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
                result = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        
        public string GetClientBalance(string customer)
        {
            string result;
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetClientBalance]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@CustomerIdentification", SqlDbType.NVarChar, 50) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListPayoOuts(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListPayOuts]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ManagePayoOuts(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ManagePayOuts]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ReceivedPayouts(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ReceivedPayouts]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> OnHoldPayouts(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[OnHoldPayouts]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> GetExecutedPayouts(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[GetExecutedPayOuts]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail UpdatePayoOuts(string ticket, string countryCode, string provider, string status)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[Reject_Transactions]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Ticket", SqlDbType.NVarChar, 5000) { Value = ticket};
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@provider", SqlDbType.VarChar, 100) { Value = provider };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@DetailStatus", SqlDbType.VarChar, 100) { Value = status };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_dt.Rows.Count > 0)
                {

                    Detail.Ticket = _dt.Rows[0][0].ToString();
                    Detail.TransactionDate = Convert.ToDateTime(_dt.Rows[0][1]);
                    Detail.Amount = Convert.ToDecimal(_dt.Rows[0][2]);
                    Detail.Currency = _dt.Rows[0][3].ToString();
                    Detail.LotNumber = Convert.ToInt64(_dt.Rows[0][4]);
                    Detail.LotCode = _dt.Rows[0][5].ToString();
                    Detail.Recipient = _dt.Rows[0][6].ToString();
                    Detail.RecipientId = _dt.Rows[0][7].ToString();
                    Detail.RecipientAccountNumber = _dt.Rows[0][8].ToString();
                    Detail.AcreditationDate = Convert.ToDateTime(_dt.Rows[0][9]);
                    Detail.Description = _dt.Rows[0][10].ToString();
                    Detail.InternalDescription = _dt.Rows[0][11].ToString();
                    Detail.ConceptCode = _dt.Rows[0][12].ToString();
                    Detail.BankAccountType = _dt.Rows[0][13].ToString();
                    Detail.EntityIdentificationType = _dt.Rows[0][14].ToString();
                    Detail.InternalStatus = _dt.Rows[0][15].ToString();
                    Detail.InternalStatusDescription = _dt.Rows[0][16].ToString();
                    Detail.idEntityUser = _dt.Rows[0][17].ToString();
                    Detail.TransactionId = _dt.Rows[0][18].ToString();
                    Detail.StatusCode = _dt.Rows[0][19].ToString();

                }


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail CancelPayouts(string ticket, string countryCode)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[Cancel_Transactions]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Ticket", SqlDbType.NVarChar, 5000) { Value = ticket };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_dt.Rows.Count > 0)
                {

                    Detail.Ticket = _dt.Rows[0][0].ToString();
                    Detail.TransactionDate = Convert.ToDateTime(_dt.Rows[0][1]);
                    Detail.Amount = Convert.ToDecimal(_dt.Rows[0][2]);
                    Detail.Currency = _dt.Rows[0][3].ToString();
                    Detail.LotNumber = Convert.ToInt64(_dt.Rows[0][4]);
                    Detail.LotCode = _dt.Rows[0][5].ToString();
                    Detail.Recipient = _dt.Rows[0][6].ToString();
                    Detail.RecipientId = _dt.Rows[0][7].ToString();
                    Detail.RecipientAccountNumber = _dt.Rows[0][8].ToString();
                    Detail.AcreditationDate = Convert.ToDateTime(_dt.Rows[0][9]);
                    Detail.Description = _dt.Rows[0][10].ToString();
                    Detail.InternalDescription = _dt.Rows[0][11].ToString();
                    Detail.ConceptCode = _dt.Rows[0][12].ToString();
                    Detail.BankAccountType = _dt.Rows[0][13].ToString();
                    Detail.EntityIdentificationType = _dt.Rows[0][14].ToString();
                    Detail.InternalStatus = _dt.Rows[0][15].ToString();
                    Detail.InternalStatusDescription = _dt.Rows[0][16].ToString();
                    Detail.idEntityUser = _dt.Rows[0][17].ToString();
                    Detail.TransactionId = _dt.Rows[0][18].ToString();
                    Detail.StatusCode = _dt.Rows[0][19].ToString();

                }


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail ReceivedPayouts(string ticket, string countryCode)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ChangeStatusFromOnHoldToReceived]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Ticket", SqlDbType.NVarChar, 5000) { Value = ticket };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_dt.Rows.Count > 0)
                {

                    Detail.Ticket = _dt.Rows[0][0].ToString();
                    Detail.TransactionDate = Convert.ToDateTime(_dt.Rows[0][1]);
                    Detail.Amount = Convert.ToDecimal(_dt.Rows[0][2]);
                    Detail.Currency = _dt.Rows[0][3].ToString();
                    Detail.LotNumber = Convert.ToInt64(_dt.Rows[0][4]);
                    Detail.LotCode = _dt.Rows[0][5].ToString();
                    Detail.Recipient = _dt.Rows[0][6].ToString();
                    Detail.RecipientId = _dt.Rows[0][7].ToString();
                    Detail.RecipientAccountNumber = _dt.Rows[0][8].ToString();
                    Detail.AcreditationDate = Convert.ToDateTime(_dt.Rows[0][9]);
                    Detail.Description = _dt.Rows[0][10].ToString();
                    Detail.InternalDescription = _dt.Rows[0][11].ToString();
                    Detail.ConceptCode = _dt.Rows[0][12].ToString();
                    Detail.BankAccountType = _dt.Rows[0][13].ToString();
                    Detail.EntityIdentificationType = _dt.Rows[0][14].ToString();
                    Detail.InternalStatus = _dt.Rows[0][15].ToString();
                    Detail.InternalStatusDescription = _dt.Rows[0][16].ToString();
                    Detail.idEntityUser = _dt.Rows[0][17].ToString();
                    Detail.TransactionId = _dt.Rows[0][18].ToString();
                    Detail.StatusCode = _dt.Rows[0][19].ToString();

                }


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> RevertDownload(List<string> ticket, string countryCode)
        {
            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[RevertDownload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(ticket) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                Detail = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListPayoOutsMEX(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[MEX_ListPayOuts]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListPayoOutsMEXSabadell(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[MEX_ListPayOutsSabadell]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListPayoOutsCOLFinandina(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_ListPayOuts_FINANDINA]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListPayoOutsCOLCajaSocial(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_ListPayOuts_BCSC]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListPayoOutsCOLItau(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_ListPayOuts_ITAU]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListPayoOutsCOLBbva(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {
            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListlotBatch = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_ListPayOuts_BBVA]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public void GenerateCertificates(TicketsToCertificate tickets)
        {
            List<SharedModel.Models.Beneficiary.Beneficiary> beneficiaryDatas = new List<SharedModel.Models.Beneficiary.Beneficiary>();
            List<SharedModel.Models.Beneficiary.CertificateData> certificates = new List<SharedModel.Models.Beneficiary.CertificateData>();
            // OBTENGO LOS DATOS DEL BENEFICIARIO
            _conn.Open();

            _cmd = new SqlCommand("[LP_Filter].[GetBeneficiaryData]", _conn);
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.CommandTimeout = 0;
            _prm = new SqlParameter("@ticketNumbers", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(tickets) };
            _cmd.Parameters.Add(_prm);

            _ds = new DataSet();
            _da = new SqlDataAdapter(_cmd);
            _da.Fill(_ds);

            foreach (DataRow row in _ds.Tables[0].Rows)
            {
                beneficiaryDatas.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Beneficiary.Beneficiary>(row));
            }
            _conn.Close();

            if (beneficiaryDatas.Count > 0)
            {
                _conn.Open();
                _cmd = new SqlCommand("SELECT TOP 1 certificateNumber FROM LP_Operation.TransactionCertificates ORDER BY 1 DESC", _conn);
                _cmd.CommandType = CommandType.Text;
                _cmd.CommandTimeout = 0;

                SqlDataReader rdr = _cmd.ExecuteReader();

                string newLiquidNumber;
                int lastId;
                if (rdr.HasRows)
                {
                    rdr.Read();
                    lastId = Convert.ToInt32(rdr["certificateNumber"].ToString());
                }
                else
                {
                    lastId = 0;
                }
                _conn.Close();

                foreach (var beneficiary in beneficiaryDatas)
                {



                    newLiquidNumber = (lastId + 1).ToString();


                    /*  SE AGREGAN LOS DATOS AL TEMPLATE DEL EXCEL */
                    var filePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, @"Resources\CERTIFICACION DE INGRESO - LOCALPAYMENT SRL v2");

                    Aspose.Cells.License cellsLicense = new Aspose.Cells.License();
                    cellsLicense.SetLicense("Aspose.Total.lic");
                    var wb = new Workbook(filePath + ".xlsx");
                    wb.Settings.RecommendReadOnly = true;
                    var sheet = wb.Worksheets[0];
                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();



                    sheet.Cells[3, 3].PutValue(newLiquidNumber);
                    sheet.Cells[4, 3].PutValue(DateTime.Today.ToString("dd/MM/yyyy"));
                    sheet.Cells[8, 3].PutValue(beneficiary.Recipient);
                    sheet.Cells[9, 3].PutValue(beneficiary.DocumentNumber);
                    sheet.Cells[10, 3].PutValue(beneficiary.SubMerchantAddress);
                    sheet.Cells[11, 3].PutValue(beneficiary.AccountNumber + " - " + beneficiary.CBU);
                    sheet.Cells[18, 7].PutValue(beneficiary.Amount);

                    //// GENERAR PDF DE LA PLANTILLA CON ASPOSE
                    wb.Save(filePath + " - TICKET NRO. " + beneficiary.Ticket + ".pdf", SaveFormat.Pdf);

                    //GUARDAR ARCHIVO EN TABLA LP_Operation.TransactionCertificates
                    byte[] data = File.ReadAllBytes(filePath + " - TICKET NRO. " + beneficiary.Ticket + ".pdf");

                    certificates.Add(new SharedModel.Models.Beneficiary.CertificateData() { idTransaction = beneficiary.idTransaction, data = data, certificateNumber = newLiquidNumber });

                    File.Delete(filePath + " - TICKET NRO. " + beneficiary.Ticket + ".pdf");

                    lastId++;
                }

                _conn.Open();
                _cmd = new SqlCommand("[LP_Operation].[TransactionCertificates_Create]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(certificates) };
                _cmd.Parameters.Add(_prm);
                _cmd.ExecuteReader();
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
        }

        public List<BankCodes> GetBankCodes()
        {
            List<BankCodes> bankCodes = new List<BankCodes>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("SELECT C.ISO3166_1_ALFA003 AS countryCode, [BC].[Code] AS bankCode  FROM [LP_Configuration].[BankCode] BC INNER JOIN [LP_Location].[Country] C ON BC.idCountry = C.idCountry WHERE [BC].[Active] = 1 ", _conn);
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                foreach (DataRow row in _ds.Tables[0].Rows)
                {
                    bankCodes.Add(DataRowHelper.CreateItemFromRow<BankCodes>(row));
                }


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return bankCodes;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListTransactionsNotify(string customer, string countryCode, TransactionToNotify transactions)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListlotBatch = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[Payout_Generic_Entity_Operation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(transactions) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@countryCode", SqlDbType.NVarChar, 12) { Value = countryCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail ReturnPayoOuts(string ticket, string countryCode, string provider, string status)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[Return_Transactions]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Ticket", SqlDbType.NVarChar, 5000) { Value = ticket };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@provider", SqlDbType.VarChar, 100) { Value = provider };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@statusName", SqlDbType.VarChar, 100) { Value = status };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_dt.Rows.Count > 0)
                {

                    Detail.Ticket = _dt.Rows[0][0].ToString();
                    Detail.TransactionDate = Convert.ToDateTime(_dt.Rows[0][1]);
                    Detail.Amount = Convert.ToDecimal(_dt.Rows[0][2]);
                    Detail.Currency = _dt.Rows[0][3].ToString();
                    Detail.LotNumber = Convert.ToInt64(_dt.Rows[0][4]);
                    Detail.LotCode = _dt.Rows[0][5].ToString();
                    Detail.Recipient = _dt.Rows[0][6].ToString();
                    Detail.RecipientId = _dt.Rows[0][7].ToString();
                    Detail.RecipientAccountNumber = _dt.Rows[0][8].ToString();
                    Detail.AcreditationDate = Convert.ToDateTime(_dt.Rows[0][9]);
                    Detail.Description = _dt.Rows[0][10].ToString();
                    Detail.InternalDescription = _dt.Rows[0][11].ToString();
                    Detail.ConceptCode = _dt.Rows[0][12].ToString();
                    Detail.BankAccountType = _dt.Rows[0][13].ToString();
                    Detail.EntityIdentificationType = _dt.Rows[0][14].ToString();
                    Detail.InternalStatus = _dt.Rows[0][15].ToString();
                    Detail.InternalStatusDescription = _dt.Rows[0][16].ToString();
                    Detail.idEntityUser = _dt.Rows[0][17].ToString();
                    Detail.TransactionId = _dt.Rows[0][18].ToString();
                    Detail.StatusCode = _dt.Rows[0][19].ToString();

                }


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return Detail;
        }

        public int GetLastLotOutNumber()
        {
            int idLotOut = 0;
            // get lot out 
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
                connection.Open();
                string sqlQuery = string.Format("select MAX(idLotOut) FROM [LP_Operation].[Transaction] ");
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idLotOut = reader.GetInt32(0);
                        }
                    }
                }
            }

            return idLotOut;
        }

        public List<string> HoldTransactions(List<string> tickets, string countryCode)
        {
            List<string> ListlotBatch = new List<string>();

            try
            {
                int expirationDays = int.Parse(ConfigurationManager.AppSettings["ONHOLD_EXPIRE_EQUALS_OR_AFTER_DAYS"].ToString());
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ChangeStatusToOnHold]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(tickets) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@countryCode", SqlDbType.NVarChar, 12) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@expire_days", SqlDbType.Int) { Value = expirationDays };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                foreach (DataRow row in _ds.Tables[0].Rows)
                {
                    ListlotBatch.Add(row[0].ToString());
                }


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ChangeTxtToRecieved(List<string> tickets, string countryCode, string customer)
        {
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListlotBatch = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ChangeStatusToRecieved]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(tickets) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@countryCode", SqlDbType.NVarChar, 12) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@Identification", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ListlotBatch = JsonConvert.DeserializeObject<List<SharedModelDTO.Models.LotBatch.LotBatchModel>>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ListlotBatch;
        }

        public void RejectExpiredOnHold()
        {
            try
            {
                using (var _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    _conn.Open();

                    var _cmd = new SqlCommand("[LP_Operation].[RejectExpiredOnHold]", _conn);
                    _cmd.CommandType = CommandType.StoredProcedure;

                    _cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

}


