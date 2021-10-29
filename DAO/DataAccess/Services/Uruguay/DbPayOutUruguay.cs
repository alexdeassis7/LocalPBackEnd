using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DataAccess.Services.Uruguay
{
    public class DbPayOutUruguay
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

                _cmd = new SqlCommand("[LP_Operation].[URY_Payout_Generic_Entity_Operation_Create]", _conn);
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

        public SharedModel.Models.Services.Uruguay.PayOutUruguay.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBank(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Uruguay.PayOutUruguay.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Uruguay.PayOutUruguay.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[URY_Payout_BROU_Bank_Operation_Download]", _conn);
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

                    result.RowsBrou = _ds.Tables[1].Rows.Count;
                    result.LinesBrou = new string[_ds.Tables[1].Rows.Count];
                    for (int i = 0; i < _ds.Tables[1].Rows.Count; i++)
                    {
                        result.LinesBrou[i] = _ds.Tables[1].Rows[i][0].ToString();
                    }

                    result.Total_Payouts = _ds.Tables[2].Rows[0][0].ToString();
                    result.Total_Brou = _ds.Tables[3].Rows[0][0].ToString();

                }
                else
                {
                    result.RowsPayouts = 0;
                    result.RowsBrou = 0;
                }

                result.Status = "OK";
                result.StatusMessage = "OK";
            }

            catch (Exception ex)
            {
                result.RowsPayouts = 0;
                result.RowsBrou = 0;
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.TransactionDetail UpdateLotBatchTransactionFromBank(string Ticket, Int64 CurrencyFxClose, string DetailStatus, bool TransactionMechanism)
        {
            SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.TransactionDetail();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[URY_Payout_BANCOLOMBIA_Bank_Operation_Upload]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Ticket", SqlDbType.VarChar) { Value = Ticket };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CurrencyExchangeClose", SqlDbType.BigInt) { Value = CurrencyFxClose };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@DetailStatus", SqlDbType.VarChar) { Value = DetailStatus };
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

                    //Detail.Ticket = _dt.Rows[0][0].ToString();
                    //Detail.TransactionDate = Convert.ToDateTime(_dt.Rows[0][1]);
                    //Detail.Amount = Convert.ToDecimal(_dt.Rows[0][2]);
                    //Detail.Currency = _dt.Rows[0][3].ToString();
                    //Detail.LotNumber = Convert.ToInt64(_dt.Rows[0][4]);
                    //Detail.LotCode = _dt.Rows[0][5].ToString();
                    //Detail.Recipient = _dt.Rows[0][6].ToString();
                    //Detail.RecipientId = _dt.Rows[0][7].ToString();
                    //Detail.RecipientAccountNumber = _dt.Rows[0][8].ToString();
                    //Detail.AcreditationDate = Convert.ToDateTime(_dt.Rows[0][9]);
                    //Detail.Description = _dt.Rows[0][10].ToString();
                    //Detail.InternalDescription = _dt.Rows[0][11].ToString();
                    //Detail.ConceptCode = _dt.Rows[0][12].ToString();
                    //Detail.BankAccountType = _dt.Rows[0][13].ToString();
                    //Detail.EntityIdentificationType = _dt.Rows[0][14].ToString();
                    //Detail.InternalStatus = _dt.Rows[0][15].ToString();
                    //Detail.InternalStatusDescription = _dt.Rows[0][16].ToString();
                    //Detail.idEntityUser = _dt.Rows[0][17].ToString();
                    //Detail.TransactionId = _dt.Rows[0][18].ToString();
                    //Detail.StatusCode = _dt.Rows[0][19].ToString();

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
