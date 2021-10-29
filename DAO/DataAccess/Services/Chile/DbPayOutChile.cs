using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tools;

namespace DAO.DataAccess.Services.Chile
{
    public class DbPayOutChile
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public SharedModelDTO.Models.LotBatch.LotBatchModel CreateLotTransaction(SharedModelDTO.Models.LotBatch.LotBatchModel data, string customer, string countryCode, bool TransactionMechanism)
        {
            SharedModelDTO.Models.LotBatch.LotBatchModel result = new SharedModelDTO.Models.LotBatch.LotBatchModel();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[CHL_Payout_Generic_Entity_Operation_Create]", _conn);
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
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public SharedModel.Models.Services.Chile.PayOutChile.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToBank(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Chile.PayOutChile.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Chile.PayOutChile.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[CHL_Payout_BCHILE_Bank_Operation_Download]", _conn);
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
                    _dt = _ds.Tables[0];
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
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public SharedModel.Models.Services.Chile.PayOutChile.DownloadLotBatchTransactionToBank.Response DownloadBatchLotTransactionToItau(bool TransactionMechanism, string JSON)
        {

            SharedModel.Models.Services.Chile.PayOutChile.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Chile.PayOutChile.DownloadLotBatchTransactionToBank.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[CHL_Payout_ITAU_Bank_Operation_Download]", _conn);
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
                    _dt = _ds.Tables[0];
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
                result.Status = "ERROR";
                result.StatusMessage = ex.Message;
            }

            return result;

        }

        public List<SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.TransactionDetail> UpdateLotBatchTransactionFromBankItau(Int64 CurrencyFxClose, bool TransactionMechanism, List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel)
        {
            List<SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.TransactionDetail> Detail = new List<SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.TransactionDetail>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[CHL_Payout_ITAU_Bank_Operation_Upload]", _conn);
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
                    Detail.Add(DataRowHelper.CreateItemFromRow<SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.TransactionDetail>(row));
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
