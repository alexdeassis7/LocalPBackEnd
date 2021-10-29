using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace DAO.DataAccess.Services.Mexico
{
    public class DbPayOutMexico
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

                _cmd = new SqlCommand("[LP_Operation].[MEX_Payout_Generic_Entity_Operation_Create]", _conn);
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

        public SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBank(bool TransactionMechanism, string JSON, int internalFiles)
        {

            SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response();
            string MifelDownloadSp = "[LP_Operation].[MEX_Payout_MIFEL_Bank_Operation_Download]";

            if (internalFiles > 0)
            {
                MifelDownloadSp = "[LP_Operation].[MEX_Payout_MIFEL_Bank_Operation_Download_Internal]";
            }
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand(MifelDownloadSp, _conn);
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
                    result.PayoutFiles = new List<SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile>();
                    SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile PayoutFile;
                    PayoutFile = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile();
                    PayoutFile.LinesPayouts = new List<string>();
                    result.PreRegisterLot = _ds.Tables[0].Rows[0][2].ToString();

                    int fileNumber = int.Parse(_ds.Tables[0].Rows[0][0].ToString());
                    int auxCount = 0;
                    decimal fileTotal = 0;
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        auxCount++;
                        // Adding Line
                        PayoutFile.LinesPayouts.Add(_ds.Tables[0].Rows[i][1].ToString());
                        // Sum line amount
                        decimal lineAmount = 0;
                        if (!string.IsNullOrEmpty(_ds.Tables[0].Rows[i][3].ToString()))
                        {
                            lineAmount = decimal.Parse(_ds.Tables[0].Rows[i][3].ToString());
                        }
                        
                        fileTotal += lineAmount;

                        if((i+1) != _ds.Tables[0].Rows.Count && int.Parse(_ds.Tables[0].Rows[i+1][0].ToString()) != fileNumber)
                        {
                            PayoutFile.FileTotal = fileTotal.ToString().Replace(",", ".");
                            PayoutFile.RowsPayouts = auxCount-1; // substracting header record

                            fileNumber = int.Parse(_ds.Tables[0].Rows[i+1][0].ToString());
                            result.PayoutFiles.Add(PayoutFile);
                            // INITIALIZE NEW PAYOUTFILE
                            PayoutFile = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile();
                            PayoutFile.LinesPayouts = new List<string>();
                            fileTotal = 0;
                            auxCount = 0;
                        }
                    }
                    PayoutFile.FileTotal = fileTotal.ToString().Replace(",", ".");
                    PayoutFile.RowsPayouts = auxCount-1;
                    result.PayoutFiles.Add(PayoutFile);

                    result.RowsPreRegister = _ds.Tables[1].Rows.Count;
                    result.LinesPreRegister = new string[_ds.Tables[1].Rows.Count];
                    for (int i = 0; i < _ds.Tables[1].Rows.Count; i++)
                    {
                        result.LinesPreRegister[i] = _ds.Tables[1].Rows[i][0].ToString();
                        
                    }

                }
                else
                {
                    //result.RowsPayouts = 0;
                    result.RowsPreRegister = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                //result.RowsPayouts = 0;
                result.RowsPreRegister = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }


        public SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankSabadell(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response();
            string SabadellDownloadSp = "[LP_Operation].[MEX_Payout_SABADELL_Bank_Operation_Download]";

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand(SabadellDownloadSp, _conn);
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
                    result.PayoutFiles = new List<SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile>();
                    SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile PayoutFile;
                    PayoutFile = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile();
                    PayoutFile.LinesPayouts = new List<string>();
                    result.PreRegisterLot = _ds.Tables[0].Rows[0][2].ToString();

                    int fileNumber = int.Parse(_ds.Tables[0].Rows[0][0].ToString());
                    int auxCount = 0;
                    decimal fileTotal = 0;
                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        auxCount++;
                        // Adding Line
                        PayoutFile.LinesPayouts.Add(_ds.Tables[0].Rows[i][1].ToString());
                        // Sum line amount
                        decimal lineAmount = 0;
                        if (!string.IsNullOrEmpty(_ds.Tables[0].Rows[i][3].ToString()))
                        {
                            lineAmount = decimal.Parse(_ds.Tables[0].Rows[i][3].ToString());
                        }

                        fileTotal += lineAmount;

                        if ((i + 1) != _ds.Tables[0].Rows.Count && int.Parse(_ds.Tables[0].Rows[i + 1][0].ToString()) != fileNumber)
                        {
                            PayoutFile.FileTotal = fileTotal.ToString().Replace(",", ".");
                            PayoutFile.RowsPayouts = auxCount - 1; // substracting header record

                            fileNumber = int.Parse(_ds.Tables[0].Rows[i + 1][0].ToString());
                            result.PayoutFiles.Add(PayoutFile);
                            // INITIALIZE NEW PAYOUTFILE
                            PayoutFile = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile();
                            PayoutFile.LinesPayouts = new List<string>();
                            fileTotal = 0;
                            auxCount = 0;
                        }
                    }
                    PayoutFile.FileTotal = fileTotal.ToString().Replace(",", ".");
                    PayoutFile.RowsPayouts = auxCount - 1;
                    result.PayoutFiles.Add(PayoutFile);

                    result.RowsPreRegister = _ds.Tables[1].Rows.Count;
                    result.LinesPreRegister = new string[_ds.Tables[1].Rows.Count];
                    
                    result.DownloadCount = int.Parse(_ds.Tables[0].Rows[0][4].ToString());

                    for (int i = 0; i < _ds.Tables[1].Rows.Count; i++)
                    {
                        result.LinesPreRegister[i] = _ds.Tables[1].Rows[i][0].ToString();

                    }

                }
                else
                {
                    //result.RowsPayouts = 0;
                    result.RowsPreRegister = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                //result.RowsPayouts = 0;
                result.RowsPreRegister = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankSantander(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response();
            string SrmDownloadSp = "[LP_Operation].[MEX_Payout_SANTANDER_Bank_Operation_Download]";

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand(SrmDownloadSp, _conn);
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
                    result.PayoutFiles = new List<SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile>();
                    SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile PayoutFile;
                    PayoutFile = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile();
                    PayoutFile.LinesPayouts = new List<string>();

                    PayoutFile.RowsPayouts = _ds.Tables[0].Rows.Count;

                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        PayoutFile.LinesPayouts.Add(_ds.Tables[0].Rows[i][0].ToString());
                    }

                    PayoutFile.FileTotal = _ds.Tables[2].Rows[0][0].ToString();
                    result.PayoutFiles.Add(PayoutFile);

                    result.DownloadCount = _ds.Tables[0].Rows.Count;
                    result.RowsPreRegister = _ds.Tables[1].Rows.Count;
                    result.LinesPreRegister = new string[_ds.Tables[1].Rows.Count];

                    for (int i = 0; i < _ds.Tables[1].Rows.Count; i++)
                    {
                        result.LinesPreRegister[i] = _ds.Tables[1].Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.DownloadCount = 0;
                    result.RowsPreRegister = 0;
                    result.LinesPreRegister = new string[0];
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPreRegister = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBankBanorte(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response();
            string SrmDownloadSp = "[LP_Operation].[MEX_Payout_BANORTE_Bank_Operation_Download]";

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand(SrmDownloadSp, _conn);
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
                    result.PayoutFiles = new List<SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile>();
                    SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile PayoutFile;
                    PayoutFile = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.PayoutFile();
                    PayoutFile.LinesPayouts = new List<string>();

                    PayoutFile.RowsPayouts = _ds.Tables[0].Rows.Count;

                    for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                    {
                        PayoutFile.LinesPayouts.Add(_ds.Tables[0].Rows[i][0].ToString());
                    }

                    PayoutFile.FileTotal = _ds.Tables[2].Rows[0][0].ToString();
                    result.PayoutFiles.Add(PayoutFile);

                    result.DownloadCount = _ds.Tables[0].Rows.Count;
                    result.RowsPreRegister = _ds.Tables[1].Rows.Count;
                    result.LinesPreRegister = new string[_ds.Tables[1].Rows.Count];

                    for (int i = 0; i < _ds.Tables[1].Rows.Count; i++)
                    {
                        result.LinesPreRegister[i] = _ds.Tables[1].Rows[i][0].ToString();
                    }
                }
                else
                {
                    result.DownloadCount = 0;
                    result.RowsPreRegister = 0;
                    result.LinesPreRegister = new string[0];
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPreRegister = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBank(Int64 CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel> uploadModel)
        {
            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[MEX_Payout_MIFEL_Bank_Operation_Upload]", _conn);
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>(row));
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

        public List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBankSantander(Int64 CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel> uploadModel)
        {
            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[MEX_Payout_SANTANDER_Bank_Operation_Upload]", _conn);
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>(row));
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

        public List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBankSabadell(Int64 CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel> uploadModel)
        {
            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[MEX_Payout_SABADELL_Bank_Operation_Upload]", _conn);
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>(row));
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
        public List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> UpdateLotBatchFromBankPreRegister(string idBankPreRegisterLot, string accountWithErrors, bool TransactionMechanism)
        {
            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> DetailList = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();
            

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[MEX_Payout_MIFEL_Bank_Operation_Upload_PreRegister]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@BankPreRegisterLot", SqlDbType.VarChar) { Value = idBankPreRegisterLot };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@accountWithErrors", SqlDbType.VarChar) { Value = accountWithErrors };
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

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow row in _dt.Rows)
                    {
                        SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail();
                        Detail.Ticket = row[0].ToString();
                        Detail.TransactionDate = Convert.ToDateTime(row[1]);
                        Detail.Amount = Convert.ToDecimal(row[2]);
                        Detail.Currency = row[3].ToString();
                        Detail.LotNumber = Convert.ToInt64(row[4]);
                        Detail.LotCode = row[5].ToString();
                        Detail.Recipient = row[6].ToString();
                        Detail.RecipientId = row[7].ToString();
                        Detail.RecipientAccountNumber = row[8].ToString();
                        Detail.AcreditationDate = Convert.ToDateTime(row[9]);
                        Detail.Description = row[10].ToString();
                        Detail.InternalDescription = row[11].ToString();
                        Detail.ConceptCode = row[12].ToString();
                        Detail.BankAccountType = row[13].ToString();
                        Detail.EntityIdentificationType = row[14].ToString();
                        Detail.InternalStatus = row[15].ToString();
                        Detail.InternalStatusDescription = row[16].ToString();
                        Detail.idEntityUser = row[17].ToString();
                        Detail.TransactionId = row[18].ToString();
                        Detail.StatusCode = row[19].ToString();

                        DetailList.Add(Detail);
                    }

                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return DetailList;
        }
    }
}
