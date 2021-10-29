using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;
using SharedModel.Models.Tools;

namespace DAO.DataAccess.Services
{
    public class DbFile
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public bool ValidFile(string filename)
        {
            bool isValid = false;
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ValidFile]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@FileName", SqlDbType.VarChar) { Value = filename };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(_dt.Rows[0][0].ToString()))
                        isValid = true;
                    else
                        isValid = false;
                }
                else
                {
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            return isValid;
        }

        public void SaveFile(SharedModelDTO.File.FileModel FileProcess, string FileStatus, string FileName, string CountryCode, string ProviderCode)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[SaveFile]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@FileBytes", SqlDbType.VarBinary) { Value = FileProcess.file_bytes_compressed };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@TransactionType", SqlDbType.VarChar) { Value = FileProcess.transaction_type };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@DateTimeProcess", SqlDbType.BigInt) { Value = Convert.ToInt64(FileProcess.datetime_process) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@OriginalFileName", SqlDbType.VarChar) { Value = FileName };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@FileName", SqlDbType.VarChar) { Value = FileProcess.file_name };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@FileNameZip", SqlDbType.VarChar) { Value = FileProcess.file_name_zip };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@CountryCode", SqlDbType.VarChar) { Value = CountryCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@ProviderCode", SqlDbType.VarChar) { Value = ProviderCode };
                _cmd.Parameters.Add(_prm);

                if (!string.IsNullOrEmpty(FileStatus))
                {
                    _prm = new SqlParameter("@FileStatus", SqlDbType.VarChar) { Value = FileStatus };
                    _cmd.Parameters.Add(_prm);
                }

                _conn.Open();
                _cmd.ExecuteNonQuery();
                _conn.Close();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response> DownloadRetentionCertificates(long merchantId, string retentionType, DateTime date, string certificateNumber = null, long? internalDescriptionMerchantId = null, long? payoutId = null)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (certificateNumber == null && internalDescriptionMerchantId == null && payoutId == null)
                {
                    _cmd = new SqlCommand("[LP_Retentions_ARG].[DownloadRetentionsFilesByaDate]", _conn);
                }
                else
                {
                    _cmd = new SqlCommand("[LP_Retentions_ARG].[DownloadRetentionsFilesByFilter]", _conn);

                    _prm = certificateNumber == null
                        ? new SqlParameter("@certificateNumber", SqlDbType.VarChar) { Value = DBNull.Value }
                        : new SqlParameter("@certificateNumber", SqlDbType.VarChar) { Value = certificateNumber };

                    _cmd.Parameters.Add(_prm);
                    
                    _prm = internalDescriptionMerchantId == 0
                        ? new SqlParameter("@internalDescriptionMerchantId", SqlDbType.BigInt) { Value = DBNull.Value }
                        : new SqlParameter("@internalDescriptionMerchantId", SqlDbType.BigInt) { Value = internalDescriptionMerchantId };
                    
                    _cmd.Parameters.Add(_prm);

                    _prm = payoutId == 0
                        ? new SqlParameter("@payoutId", SqlDbType.BigInt) { Value = DBNull.Value }
                        : new SqlParameter("@payoutId", SqlDbType.BigInt) { Value = payoutId };
                    
                    _cmd.Parameters.Add(_prm);
                }

                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@merchantId", SqlDbType.BigInt) { Value = merchantId };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@retentionCode", SqlDbType.VarChar) { Value = retentionType };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@transactionsDate", SqlDbType.Date) { Value = date };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows[0][0].ToString() == String.Empty)
                {
                    return new List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response>>(_dt.Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response> DownloadRetentionCertificates(string customer_id, string retentionType)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[DownloadFiles]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.VarChar) { Value = customer_id };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@retentionCode", SqlDbType.VarChar) { Value = retentionType };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows[0][0].ToString() == String.Empty)
                {
                    return new List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response>>(_dt.Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response> DownloadCertificates(List<long> idTransactions)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _conn.Open();
                _cmd = new SqlCommand("SELECT * FROM LP_Operation.TransactionCertificates WHERE idTransaction IN (" + String.Join(",", idTransactions) + ")", _conn);
                _cmd.CommandType = CommandType.Text;
                _cmd.CommandTimeout = 0;

                var listCertificates = new List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response>();

                using (SqlDataReader rdr = _cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var entityResponse = new RetentionModel.RetentionCertificate.Response();
                        entityResponse.FileBytes = (byte[])rdr["CertificateFileBytes"]; ;
                        entityResponse.FileName = rdr["idTransaction"].ToString();
                        listCertificates.Add(entityResponse);
                    }
                }

                _conn.Close();

                return listCertificates;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
