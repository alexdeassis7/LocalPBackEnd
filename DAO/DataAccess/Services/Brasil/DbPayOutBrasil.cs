using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tools;

namespace DAO.DataAccess.Services.Brasil
{
    public class DbPayOutBrasil
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

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_Generic_Entity_Operation_Create]", _conn);
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

        public SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelResponse DownloadBatchLotTransactionToFastCash(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelResponse result = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelResponse();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_FASTCASH_Bank_Operation_Download]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JSON };
                _cmd.Parameters.Add(_prm);


                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                result.transactions = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelFastcash>>(_ds.Tables[0].Rows[0][0].ToString());

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail UpdateLotBatchTransactionFromBank(string Ticket, Int64 CurrencyFxClose, bool DetailStatus, string DetailMessage, bool TransactionMechanism)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_FASTCASH_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Ticket", SqlDbType.VarChar) { Value = Ticket };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.BigInt) { Value = CurrencyFxClose };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@TxSuccess", SqlDbType.Bit) { Value = DetailStatus };
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

                    Detail.Ticket = _dt.Rows[0][0].ToString();
                    Detail.TransactionDate = Convert.ToDateTime(_dt.Rows[0][1]);
                    Detail.Amount = Convert.ToDecimal(_dt.Rows[0][2]);
                    Detail.Currency = _dt.Rows[0][3].ToString();
                    Detail.LotNumber = Convert.ToInt64(_dt.Rows[0][4]);
                    Detail.LotCode = _dt.Rows[0][5].ToString();
                    Detail.Recipient = _dt.Rows[0][6].ToString();
                    Detail.RecipientCUIT = _dt.Rows[0][7].ToString();
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

        public List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromPlural(Int64 CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModelPlural> uploadModel)
        {
            List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_PLURAL_Bank_Operation_Upload]", _conn);
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail>(row));
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

        public List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromSafra(Int64 CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_SAFRA_Bank_Operation_Upload]", _conn);
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail>(row));
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

        public List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBankDoBrasil(Int64 CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_BDOB_Bank_Operation_Upload]", _conn);
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail>(row));
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

        public List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBankSantander(Int64 CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[BRA_Payout_SANTANDER_Bank_Operation_Upload]", _conn);
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail>(row));
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
