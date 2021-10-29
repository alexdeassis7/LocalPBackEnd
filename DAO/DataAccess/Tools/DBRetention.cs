
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using SharedModelDTO.Models.Transaction;

namespace DAO.DataAccess.Tools
{
    public class DBRetention
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;


        public string Insert_Batch(string customer, List<SharedModel.Models.Tools.RetentionModel.TransactionResultAfip> jsonCuit, Boolean TransactionMechanism, string date)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[RegisteredEntities_Insert_Batch]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(jsonCuit) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@Customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@Date", SqlDbType.NVarChar, 8) { Value = date };
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

                return _ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Insert_Batch(string customer, List<SharedModel.Models.Tools.RetentionModel.TransactionResultArba> jsonCuit, Boolean TransactionMechanism, string date)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[RegisteredEntitiesArba_Insert_Batch]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(jsonCuit) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@Customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@Date", SqlDbType.NVarChar, 8) { Value = date };
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

                return _ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int Clean_All(string RetentionType)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (RetentionType == "AFIP")
                {
                    _cmd = new SqlCommand("[LP_Retentions_ARG].[RegisteredEntities_Clean_All]", _conn);
                }
                else if (RetentionType == "ARBA")
                {
                    _cmd = new SqlCommand("[LP_Retentions_ARG].[RegisteredEntitiesArba_Clean_All]", _conn);
                }

                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _conn.Open();
                var result = _cmd.ExecuteNonQuery();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return result;
            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

            }
        }
        public void ChangeRetentionCertificateStatus(int id)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[SetDownloadedCertificate]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@idTransactionCertificate", SqlDbType.BigInt) { Value = id };
                _cmd.Parameters.Add(_prm);

                _conn.Open();
                var result = _cmd.ExecuteNonQuery();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

            }
        }
        public List<TransAgForRetentionsModel> GetTransactionsAgroupedForRetentions(TransAgForRetentionsFilterModel filter)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                var result = new List<TransAgForRetentionsModel>();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ListTransactionAgForRetentions]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@merchantId", SqlDbType.Int) { Value = filter.MerchantId };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@certTypeCode", SqlDbType.NVarChar) { Value = filter.CertTypeCode };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@dateFrom", SqlDbType.Date) { Value = filter.dateFrom };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@dateTo", SqlDbType.Date) { Value = filter.dateTo };
                _cmd.Parameters.Add(_prm);

                _conn.Open();


                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                foreach (DataRow r in _dt.Rows)
                {
                    result.Add(new TransAgForRetentionsModel { Merchant = new MerchantModel { Id = (long)r["idEntityUser"], Name = (string)r["FirstName"] }, OrganismoCode = (string)r["OrganismoCode"], Fecha = (DateTime)r["Fecha"], Importe_Retenciones = (decimal)r["Importe_Retenciones"], Certificates_Created = (int)r["Certificates_Created"], Certificates_Pending = (int)r["Certificates_Pending"], Trx_Pending = (int)r["Trx_Pending"] });
                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return result;

            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

            }

        }
        public SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt DownloadTxtAfipMonthly(string customer_id, int year, int month, string typeFile)
        {

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt result = new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt();
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[DownloadAFIPWithholdingsMonthly]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer_id };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@year", SqlDbType.Int) { Value = year };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@month", SqlDbType.Int) { Value = month };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@typeFile", SqlDbType.VarChar) { Value = typeFile };
                _cmd.Parameters.Add(_prm);

                _conn.Open();
               

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

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                result.Status = "OK";
                result.StatusMessage = "OK";
                return result;
             
            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

            }
   

        }

        public SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel DownloadExcelAfipMonthly(string customer_id, int year, int month, string typeFile)
        {

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel result = new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel();
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[DownloadAFIPWithholdingsMonthly]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer_id };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@year", SqlDbType.Int) { Value = year };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@month", SqlDbType.Int) { Value = month };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@typeFile", SqlDbType.VarChar) { Value = typeFile };
                _cmd.Parameters.Add(_prm);
                _conn.Open();


                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                result.ListRetentions = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
       
                return result;

            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }
     


        }

        public SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt DownloadTxtArbaMonthly(string customer_id, int year, int month, string typeFile)
        {

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt result = new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt();
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[DownloadARBAWithholdingsMonthly]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer_id };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@year", SqlDbType.Int) { Value = year };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@month", SqlDbType.Int) { Value = month };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@typeFile", SqlDbType.VarChar) { Value = typeFile };
                _cmd.Parameters.Add(_prm);

                _conn.Open();


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

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                result.Status = "OK";
                result.StatusMessage = "OK";
                return result;

            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

            }


        }

        public SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel DownloadExcelArbaMonthly(string customer_id, int year, int month, string typeFile)
        {

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel result = new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel();
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[DownloadARBAWithholdingsMonthly]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer_id };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@year", SqlDbType.Int) { Value = year };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@month", SqlDbType.Int) { Value = month };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@typeFile", SqlDbType.VarChar) { Value = typeFile };
                _cmd.Parameters.Add(_prm);

                _conn.Open();


                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                result.ListRetentions = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return result;

            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }



        }

        public SharedModel.Models.Tools.RetentionModel.Whitelist.Response ListWhitelist(string customer, SharedModel.Models.Tools.RetentionModel.Whitelist.Request data, string countryCode)
        {

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                SharedModel.Models.Tools.RetentionModel.Whitelist.Response result = new SharedModel.Models.Tools.RetentionModel.Whitelist.Response();
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions].[WhiteList_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@country_code", SqlDbType.VarChar) { Value = countryCode };
                _cmd.Parameters.Add(_prm);
                _conn.Open();


                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                result.ListWhitelist = _ds.Tables[0].Rows[0][0].ToString();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return result;

            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }



        }

        public SharedModel.Models.Tools.RetentionModel.Whitelist.Response Insert_Cuit_To_Whitelist(SharedModel.Models.Tools.RetentionModel.WhiteList data, string customer)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                SharedModel.Models.Tools.RetentionModel.Whitelist.Response result = new SharedModel.Models.Tools.RetentionModel.Whitelist.Response();

                
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions].[WhiteList_Add]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.VarChar) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@IdentificationNumber", SqlDbType.VarChar) { Value = data.Numberdoc  };                                         
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@FirstName", SqlDbType.VarChar) { Value = data.FirstName };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@LastName", SqlDbType.VarChar) { Value = data.LastName };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@idEntityIdentificationType", SqlDbType.Int) { Value = data.idEntityIdentificationType };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@Details", SqlDbType.NVarChar, -1) { Value = JsonConvert.SerializeObject(data.Details) };
                _cmd.Parameters.Add(_prm);


                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                result.Status = _ds.Tables[0].Rows[0][0].ToString();
                result.StatusMessage = _ds.Tables[0].Rows[0][1].ToString();
                result.idWhitelist = _ds.Tables[0].Rows[0][2].ToString();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Update_Cuit_To_Whitelist(string customer, SharedModel.Models.Tools.RetentionModel.WhiteList whiteList, string updateType)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions].[WhiteList_Update]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@customer", SqlDbType.VarChar) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@idWhiteList", SqlDbType.Int) { Value = whiteList.idWhitelist };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@UpdateType", SqlDbType.VarChar) { Value = updateType };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@JSON", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(whiteList.Details) };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return _ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Delete_Cuit_To_Whitelist(string Numberdoc, string customer)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions].[Whitelist_Delete]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@IdentificationNumber", SqlDbType.VarChar) { Value = Numberdoc };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.VarChar) { Value = customer };
                _cmd.Parameters.Add(_prm);
                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return _ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus> ListCertificatesProcessInfo ()
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                List<SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus> resultList = new List<SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus>();
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Process].[ARG_Retentions_WCF_Services_System_LastRun_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _conn.Open();

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus status = new SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus();

                    status.Description = _dt.Rows[i]["Description"].ToString();                                          
                    status.ServiceIsRunning = Convert.ToBoolean(_dt.Rows[i]["ServiceIsRunning"]);
                    status.TotalOfCertificates = Convert.ToInt32(_dt.Rows[i]["TotalOfCertificates"]);
                    status.CertificatesPendingGeneration = Convert.ToInt32(_dt.Rows[i]["CertificatesPendingGeneration"]);
                    status.CertificatesPendingDownload = Convert.ToInt32(_dt.Rows[i]["CertificatesPendingDownload"]);
                    status.ProcessStart = _dt.Rows[i]["ProcessStart"].ToString();
                    status.ProcessEnd = _dt.Rows[i]["ProcessEnd"].ToString();

                    resultList.Add(status);
                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return resultList;

            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response ListRetentions(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions].[ListRetentions]", _conn);
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

        public SharedModel.Models.View.Report.List.Response RefundRetentions(SharedModel.Models.Tools.RetentionModel.Refund.Request data, string customer)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions].[RefundRetention]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                var test = JsonConvert.SerializeObject(data);
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


    }
}

