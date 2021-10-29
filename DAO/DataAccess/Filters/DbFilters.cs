using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using SharedModel.Models.Filters;
using System.Configuration;
using Newtonsoft.Json;

namespace DAO.DataAccess.Filters
{
    public class DbFilters
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public List<Filter.EntityUser> GetClients(string idEntityUser)
        {

            List<Filter.EntityUser> ListClients = new List<Filter.EntityUser>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Entity].[ListEntityUser]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _prm = new SqlParameter("@userIdentification", SqlDbType.VarChar, 5000) { Value = idEntityUser };
                _cmd.Parameters.Add(_prm);
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListClients = JsonConvert.DeserializeObject<List<Filter.EntityUser>>(_ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListClients;
        }

        public List<Filter.TransactionType> GetTransactionTypes()
        {

            List<Filter.TransactionType> Result = new List<Filter.TransactionType>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetTransactionTypes]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        Result.Add
                            (
                            new Filter.TransactionType
                            {
                                TT_Code = _dt.Rows[i][0].ToString()
                                ,
                                TT_Name = _dt.Rows[i][1].ToString()
                                ,
                                TT_Desc = _dt.Rows[i][2].ToString()
                                ,
                                TT_Country = Convert.ToInt64(_dt.Rows[i][3].ToString())
                                ,
                                TG_Code = _dt.Rows[i][4].ToString()
                                ,
                                TG_Name = _dt.Rows[i][5].ToString()
                                ,
                                TG_Desc = _dt.Rows[i][6].ToString()
                                ,
                                TG_Country = Convert.ToInt64(_dt.Rows[i][7].ToString())
                                ,
                                TO_Code = _dt.Rows[i][8].ToString()
                                ,
                                TO_Name = _dt.Rows[i][9].ToString()
                                ,
                                TO_Desc = _dt.Rows[i][10].ToString()
                                ,
                                TO_Country = Convert.ToInt64(_dt.Rows[i][11].ToString())
                                ,
                                idTransactionType = Convert.ToInt64(_dt.Rows[i][12].ToString())
                                ,
                                idTransactionGroup = Convert.ToInt64(_dt.Rows[i][13].ToString())
                                ,
                                idTransactionOperation = Convert.ToInt64(_dt.Rows[i][14].ToString())
                            }
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }

        public List<Filter.TransactionTypeProvider> GetTransactionTypesProvider()
        {

            List<Filter.TransactionTypeProvider> Result = new List<Filter.TransactionTypeProvider>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetTransactionTypeProvider]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        Result.Add
                            (
                            new Filter.TransactionTypeProvider
                            {
                                idTransactionTypeProvider = Convert.ToInt64(_dt.Rows[i][0].ToString())
                                ,
                                idTransactionType = Convert.ToInt64(_dt.Rows[i][1].ToString())
                                ,
                                idProvider = Convert.ToInt64(_dt.Rows[i][2].ToString())
                                ,
                                TT_Code = _dt.Rows[i][3].ToString()
                                ,
                                TT_Name = _dt.Rows[i][4].ToString()
                                ,
                                TT_Description = _dt.Rows[i][5].ToString()
                                ,
                                PROV_Code = _dt.Rows[i][6].ToString()
                                ,
                                PROV_Name = _dt.Rows[i][7].ToString()
                                ,
                                PROV_Description = _dt.Rows[i][8].ToString()
                            }
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }

        public List<Filter.ProviderPayWayServices> GetProviderPayWayServices()
        {

            List<Filter.ProviderPayWayServices> Result = new List<Filter.ProviderPayWayServices>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetProviderPayWayServices]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        Result.Add
                            (
                            new Filter.ProviderPayWayServices
                            {
                                idProviderPayWayService = Convert.ToInt64(_dt.Rows[i][0].ToString())
                                ,
                                idPayWayService = Convert.ToInt64(_dt.Rows[i][1].ToString())
                                ,
                                idProvider = Convert.ToInt64(_dt.Rows[i][2].ToString())
                                ,
                                PWS_Code = _dt.Rows[i][3].ToString()
                                ,
                                PWS_Name = _dt.Rows[i][4].ToString()
                                ,
                                PWS_Description = _dt.Rows[i][5].ToString()
                                ,
                                PROV_Code = _dt.Rows[i][6].ToString()
                                ,
                                PROV_Name = _dt.Rows[i][7].ToString()
                                ,
                                PROV_Description = _dt.Rows[i][8].ToString()
                            }
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }

        public List<Filter.Currency> GetListCurrency()
        {

            List<Filter.Currency> ListCurrency = new List<Filter.Currency>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Configuration].[ListCurrency]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListCurrency = JsonConvert.DeserializeObject<List<Filter.Currency>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListCurrency;
        }
        public List<Filter.Country> GetListCountries()
        {

            List<Filter.Country> ListCountries = new List<Filter.Country>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Location].[ListCountries]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListCountries = JsonConvert.DeserializeObject<List<Filter.Country>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListCountries;
        }


        //--------------Franco Rivero

        public List<Filter.CountryOfMerchant> GetListCountriesMerchant(Int64 idMerchant)
        {

            List<Filter.CountryOfMerchant> ListCountriesMerchant = new List<Filter.CountryOfMerchant>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Configuration].[ListCountriesByMerchant]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _prm = new SqlParameter("@idMerchant", SqlDbType.BigInt, 5000) { Value = idMerchant };
                _cmd.Parameters.Add(_prm);
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListCountriesMerchant = JsonConvert.DeserializeObject<List<Filter.CountryOfMerchant>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListCountriesMerchant;
        }

        //-------------------------------------Fin de linea--------
        public List<Filter.Status> GetListStatus()
        {

            List<Filter.Status> ListStatus = new List<Filter.Status>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Configuration].[ListStatus]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListStatus = JsonConvert.DeserializeObject<List<Filter.Status>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListStatus;
        }

        public Filter.EntitySubMerchant GetSubMerchantUser(long customerId, string subClientCode)
        {

            Filter.EntitySubMerchant subMerchant = null;

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Entity].[GetSubmerchantByCustomerIdAndSubclientCode]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                var _prm = new SqlParameter("@customerId", SqlDbType.BigInt, 5000) { Value = customerId };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@subclientCode", SqlDbType.NVarChar) { Value = subClientCode };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                subMerchant = JsonConvert.DeserializeObject<Filter.EntitySubMerchant>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return subMerchant;
        }

        public List<Filter.EntitySubMerchant> GetListSubMerchantUser()
        {

            List<Filter.EntitySubMerchant> ListSubMerchant = new List<Filter.EntitySubMerchant>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Entity].[ListSubmerchantUser]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListSubMerchant = JsonConvert.DeserializeObject<List<Filter.EntitySubMerchant>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListSubMerchant;
        }
        public List<Filter.EntitySubMerchant> GetListSubMerchantUserForSelect(string idEntityUser)
        {

            List<Filter.EntitySubMerchant> ListSubMerchant = new List<Filter.EntitySubMerchant>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Entity].[ListSubmerchantUserForSelect]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _prm = new SqlParameter("@idEntityUser", SqlDbType.VarChar, 5000) { Value = idEntityUser };
                _cmd.Parameters.Add(_prm);
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListSubMerchant = JsonConvert.DeserializeObject<List<Filter.EntitySubMerchant>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListSubMerchant;
        }

        public List<Filter.RetentionReg> GetListRetentionsReg()
        {

            List<Filter.RetentionReg> ListRetentions = new List<Filter.RetentionReg>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Retentions_ARG].[ListRetentionReg]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListRetentions = JsonConvert.DeserializeObject<List<Filter.RetentionReg>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListRetentions;
        }

        public List<Filter.FieldValidation> GetListFieldsValidation()
        {

            List<Filter.FieldValidation> ListRetentions = new List<Filter.FieldValidation>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetListFieldsValidation]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListRetentions = JsonConvert.DeserializeObject<List<Filter.FieldValidation>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListRetentions;
        }

        public List<Filter.ErrorType> GetListErrorTypes()
        {

            List<Filter.ErrorType> ListErrorTypes = new List<Filter.ErrorType>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[GetListErrorTypes]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListErrorTypes = JsonConvert.DeserializeObject<List<Filter.ErrorType>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListErrorTypes;
        }

        public List<Filter.SettlementInformation> GetBeneficiaries(string merchantId, string dateFrom, string dateTo)
        {

            List<Filter.SettlementInformation> ListBeneficiaries = new List<Filter.SettlementInformation>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[ListSettlementInformation]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                var _prm = new SqlParameter("@dateFrom", SqlDbType.NVarChar) { Value = dateFrom };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@dateTo", SqlDbType.NVarChar) { Value = dateTo };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@merchantId", SqlDbType.NVarChar) { Value = merchantId };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListBeneficiaries = JsonConvert.DeserializeObject<List<Filter.SettlementInformation>>(_ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListBeneficiaries;
        }

        public List<SharedModel.Models.Beneficiary.BlackList> GetBlacklist()
        {
            List<SharedModel.Models.Beneficiary.BlackList> blackLists = new List<SharedModel.Models.Beneficiary.BlackList>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Configuration].[GetBlackList]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                blackLists = JsonConvert.DeserializeObject<List<SharedModel.Models.Beneficiary.BlackList>>(_ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return blackLists;
        }

        public List<SharedModel.Models.Beneficiary.BlackList> GetAML(string countryCode)
        {
            List<SharedModel.Models.Beneficiary.BlackList> aml = new List<SharedModel.Models.Beneficiary.BlackList>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                string spName = "[LP_Configuration].[GetAMLBlackList]";
                if(countryCode == "BRA")
                {
                    spName = "[LP_Configuration].[GetAMLBlackList_BRA]";
                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand(spName, _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                aml = JsonConvert.DeserializeObject<List<SharedModel.Models.Beneficiary.BlackList>>(_ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return aml;
        }

        public List<Filter.Providers> GetProviders(string providerCode)
        {

            List<Filter.Providers> ListProviders = new List<Filter.Providers>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[ListProviders]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _prm = new SqlParameter("@transactionType", SqlDbType.VarChar) { Value = providerCode };
                _cmd.Parameters.Add(_prm);
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                ListProviders = JsonConvert.DeserializeObject<List<Filter.Providers>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ListProviders;
        }

        public List<Filter.InternalStatus> GetInternalStatuses()
        {

            List<Filter.InternalStatus> internalStatuses = new List<Filter.InternalStatus>();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Filter].[ListInternalStatus]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                internalStatuses = JsonConvert.DeserializeObject<List<Filter.InternalStatus>>(_ds.Tables[0].Rows[0][0].ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return internalStatuses;
        }


    }
}
