using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.Filters;


namespace SharedBusiness.Filters
{
    public class BlFilters
    {

        public List<Filter.EntityUser> GetClients (string idEntityUser) {

            List<Filter.EntityUser> ListClients = new List<Filter.EntityUser>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListClients = dbFilters.GetClients(idEntityUser);
             
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            return ListClients;  
    
         }

        public List<Filter.TransactionType> GetTransactionTypes()
        {
            List<Filter.TransactionType> ListTransactionType = new List<Filter.TransactionType>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListTransactionType = dbFilters.GetTransactionTypes();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListTransactionType;
        }

        public List<Filter.TransactionTypeProvider> GetTransactionTypesProvider()
        {
            List<Filter.TransactionTypeProvider> ListTransactionType = new List<Filter.TransactionTypeProvider>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListTransactionType = dbFilters.GetTransactionTypesProvider();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListTransactionType;
        }

        public List<Filter.ProviderPayWayServices> GetProviderPayWayServices()
        {
            List<Filter.ProviderPayWayServices> ListTransactionType = new List<Filter.ProviderPayWayServices>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListTransactionType = dbFilters.GetProviderPayWayServices();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListTransactionType;
        }

        public List<Filter.Currency> GetListCurrency()
        {

            List<Filter.Currency> ListCurrency = new List<Filter.Currency>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListCurrency = dbFilters.GetListCurrency();

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
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListCountries = dbFilters.GetListCountries();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ListCountries;

        }
        //_---------------------------LINEA FRANCO RIVERO
        public List<Filter.CountryOfMerchant> GetListCountriesMerchant(Int64 idMerchant)
        {

            List<Filter.CountryOfMerchant> ListCountriesMerchant = new List<Filter.CountryOfMerchant>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListCountriesMerchant = dbFilters.GetListCountriesMerchant(idMerchant);

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ListCountriesMerchant;

        }

       // ---------------------------------- LINEA FRANCO RIVERO
        public List<Filter.Status> GetListStatus()
        {

            List<Filter.Status> ListStatus = new List<Filter.Status>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListStatus = dbFilters.GetListStatus();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ListStatus;

        }

        public List<Filter.EntitySubMerchant> GetListSubMerchantUser()
        {

            List<Filter.EntitySubMerchant> ListSubMerchant = new List<Filter.EntitySubMerchant>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListSubMerchant = dbFilters.GetListSubMerchantUser();

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
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListSubMerchant = dbFilters.GetListSubMerchantUserForSelect(idEntityUser);

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ListSubMerchant;

        }
        public Filter.EntitySubMerchant GetSubMerchantUser(long customerId, string subClientCode)
        {
            DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
            var subMerchant = dbFilters.GetSubMerchantUser(customerId, subClientCode);

            return subMerchant;

        }

        public List<Filter.RetentionReg> GetListRetentionsReg()
        {

            List<Filter.RetentionReg> ListRetentions= new List<Filter.RetentionReg>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListRetentions = dbFilters.GetListRetentionsReg();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ListRetentions;

        }

        public List<Filter.FieldValidation> GetListFieldsValidation()
        {

            List<Filter.FieldValidation> ListFields = new List<Filter.FieldValidation>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListFields = dbFilters.GetListFieldsValidation();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ListFields;

        }
        public List<Filter.ErrorType> GetListErrorTypes()
        {

            List<Filter.ErrorType> ListErrorTypes = new List<Filter.ErrorType>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListErrorTypes = dbFilters.GetListErrorTypes();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ListErrorTypes;

        }

        public List<Filter.SettlementInformation> GetSettlements(string merchantId, string dateFrom, string dateTo)
        {

            List<Filter.SettlementInformation> ListSettlement = new List<Filter.SettlementInformation>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                ListSettlement = dbFilters.GetBeneficiaries(merchantId,dateFrom,dateTo);

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ListSettlement;

        }

        public List<SharedModel.Models.Beneficiary.BlackList> GetBlackLists()
        {
            List<SharedModel.Models.Beneficiary.BlackList> blackLists = new List<SharedModel.Models.Beneficiary.BlackList>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                blackLists = dbFilters.GetBlacklist();
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
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                aml = dbFilters.GetAML(countryCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return aml;
        }

        public List<Filter.Providers> GetProviders(string providerCode)
        {

            List<Filter.Providers> providers = new List<Filter.Providers>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                providers = dbFilters.GetProviders(providerCode);

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return providers;

        }

        public List<Filter.InternalStatus> GetInternalStatuses()
        {

            List<Filter.InternalStatus> internalStatuses = new List<Filter.InternalStatus>();
            try
            {
                DAO.DataAccess.Filters.DbFilters dbFilters = new DAO.DataAccess.Filters.DbFilters();
                internalStatuses = dbFilters.GetInternalStatuses();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return internalStatuses;

        }

    }
}
