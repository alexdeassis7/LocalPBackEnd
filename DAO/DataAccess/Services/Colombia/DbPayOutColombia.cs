using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;
using SharedModel.Models.Services.Colombia.Banks.Bancolombia;
using Tools;

namespace DAO.DataAccess.Services.Colombia
{
    public class DbPayOutColombia
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;
        const int DeadlockErrorCode = 1205;
        public SharedModelDTO.Models.LotBatch.LotBatchModel CreateLotTransaction(SharedModelDTO.Models.LotBatch.LotBatchModel data, string customer, string countryCode, bool TransactionMechanism, int retries = 0)
        {
            SharedModelDTO.Models.LotBatch.LotBatchModel result = new SharedModelDTO.Models.LotBatch.LotBatchModel();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_Generic_Entity_Operation_Create]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 50) { Value = customer };
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
                    result = CreateLotTransaction(data, customer, countryCode, TransactionMechanism, retries + 1);
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

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_Generic_Entity_Operation_Update]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@Customer", SqlDbType.NVarChar, 12) { Value = customer };
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

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_Generic_Entity_Operation_Delete]", _conn);
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

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListLotTransaction(string customer)
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

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankBColombia(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_BANCOLOMBIA_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = _ds.Tables[1].Rows.Count;
                    result.LinesBeneficiaries = new string[_ds.Tables[1].Rows.Count];
                    for (int i = 0; i < _ds.Tables[1].Rows.Count; i++)
                    {
                        result.LinesBeneficiaries[i] = _ds.Tables[1].Rows[i][0].ToString();
                    }

                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankBColombiaSAS(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_BCOLOMBIASAS_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = _ds.Tables[1].Rows.Count;
                    result.LinesBeneficiaries = new string[_ds.Tables[1].Rows.Count];
                    for (int i = 0; i < _ds.Tables[1].Rows.Count; i++)
                    {
                        result.LinesBeneficiaries[i] = _ds.Tables[1].Rows[i][0].ToString();
                    }

                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankOccidente(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_OCCIDENTE_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = 1;
                    result.FileBase64_Beneficiaries = _ds.Tables[1].Rows[0][0].ToString();
                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankAccionesValores(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_ACCIVAL_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = _ds.Tables[1].Rows.Count;
                    result.LinesBeneficiaries = new string[_ds.Tables[1].Rows.Count];
                    for (int i = 0; i < _ds.Tables[1].Rows.Count; i++)
                    {
                        result.LinesBeneficiaries[i] = _ds.Tables[1].Rows[i][0].ToString();
                    }

                    //total amount
                    result.TotalAmount = _ds.Tables[2].Rows[0][0].ToString();

                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankScotia(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_SCOTIA_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = 1;

                    //total amount
                    result.TotalAmount = _ds.Tables[1].Rows[0][0].ToString();
                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankFinandina(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_FINANDINA_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = 1;
                    //total amount
                    result.TotalAmount = _ds.Tables[1].Rows[0][0].ToString();

                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankCajaSocial(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_BCSC_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = 1;

                    //total amount
                    result.TotalAmount = _ds.Tables[1].Rows[0][0].ToString();
                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }
        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankItau(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_ITAU_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = 1;

                    //total amount
                    result.TotalAmount = _ds.Tables[1].Rows[0][0].ToString();
                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankBbva(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_BBVA_Bank_Operation_Download]", _conn);
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

                _ds = new DataSet();
                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_ds.Tables.Count > 0)
                {
                    result.RowsPayouts = _ds.Tables[0].Rows.Count;
                    result.LinesPayouts = new string[_ds.Tables[0].Rows.Count];
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        result.LinesPayouts[i] = _ds.Tables[0].Rows[i][0].ToString();
                    }

                    result.RowsBeneficiaries = 1;

                    //total amount
                    result.TotalAmount = _ds.Tables[1].Rows[0][0].ToString();
                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBeneficiaries = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBeneficiaries = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public List<PayOutColombia.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBank(long CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<PayOutColombia.UploadTxtFromBank.TransactionDetail> Detail = new List<PayOutColombia.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_BANCOLOMBIA_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(uploadModel) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.BigInt) { Value = CurrencyFxClose };
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<PayOutColombia.UploadTxtFromBank.TransactionDetail>(row));
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

        public List<PayOutColombia.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBankBColombiaSas(long CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<PayOutColombia.UploadTxtFromBank.TransactionDetail> Detail = new List<PayOutColombia.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_BANCOLOMBIASAS_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(uploadModel) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.BigInt) { Value = CurrencyFxClose };
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<PayOutColombia.UploadTxtFromBank.TransactionDetail>(row));
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

        public List<PayOutColombia.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBankOccidente(long CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<PayOutColombia.UploadTxtFromBank.TransactionDetail> Detail = new List<PayOutColombia.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[COL_Payout_OCCIDENTE_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(uploadModel) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.BigInt) { Value = CurrencyFxClose };
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<PayOutColombia.UploadTxtFromBank.TransactionDetail>(row));
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
    }
}
