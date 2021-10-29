using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Filters
{
    public class Filter
    {



        public class EntityUser
        {
            public Int64 idEntityUser { get; set; }
            public string FirstName { get; set; }
            public string CountryCode { get; set; }
            public string Identification { get; set; }
            public string UserSiteIdentification { get; set; }
            public long CurrencyClient { get; set; }

        }

        public class TransactionType
        {
            public string TT_Code { get; set; }
            public string TT_Name { get; set; }
            public string TT_Desc { get; set; }
            public Int64 TT_Country { get; set; }
            public string TG_Code { get; set; }
            public string TG_Name { get; set; }
            public string TG_Desc { get; set; }
            public Int64 TG_Country { get; set; }
            public string TO_Code { get; set; }
            public string TO_Name { get; set; }
            public string TO_Desc { get; set; }
            public Int64 TO_Country { get; set; }
            public Int64 idTransactionType { get; set; }
            public Int64 idTransactionGroup { get; set; }
            public Int64 idTransactionOperation { get; set; }
        }

        public class TransactionTypeProvider
        {
            public Int64 idTransactionTypeProvider { get; set; }
            public Int64 idTransactionType { get; set; }
            public Int64 idProvider { get; set; }
            public string TT_Code { get; set; }
            public string TT_Name { get; set; }
            public string TT_Description { get; set; }
            public string PROV_Code { get; set; }
            public string PROV_Name { get; set; }
            public string PROV_Description { get; set; }
        }

        public class ProviderPayWayServices
        {
            public Int64 idProviderPayWayService { get; set; }
            public Int64 idPayWayService { get; set; }
            public Int64 idProvider { get; set; }
            public string PWS_Code { get; set; }
            public string PWS_Name { get; set; }
            public string PWS_Description { get; set; }
            public string PROV_Code { get; set; }
            public string PROV_Name { get; set; }
            public string PROV_Description { get; set; }
        }

        public class Currency
        {
            public Int64 idCurrencyType { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class Country
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        //--------Franco Rivero
        public class CountryOfMerchant
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string idEntityMerchant { get; set; }
            public Int64 idCountry { get; set; }
            public string Description { get; set; }
        }

        //--------Franco Rivero----
        public class Status
        {
            public Int64 idStatus { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }
        public class EntitySubMerchant
        {
            public string idEntityUser { get; set; }
            public string Identification { get; set; }
            public string Description { get; set; }
            public string idEntitySubMerchant { get; set; }
            public string SubMerchantIdentification { get; set; }
            public string MailAccount { get; set; }
            public string CountryCode { get; set; }
            public bool IsCorporate { get; set; }
        }

        public class RetentionReg
        {
            public Int64 idReg { get; set; }
            public string reg { get; set; }
            public string description { get; set; }
        }

        public class FieldValidation
        {
            public Int64 idField { get; set; }
            public string Name { get; set; }
            public Int64 idTransactionType { get; set; }
            public string CountryCode { get; set; }

        }
        public class ErrorType
        {

            public Int64 idErrorType { get; set; }
            public string Description { get; set; }
            public string errorType { get; set; }
            public Int64 idTransactionType { get; set; }
          
        }

        public class SettlementInformation
        {
            public long SettlementNumber { get; set; }
            public DateTime SettlementDate { get; set; }
            public byte[] PDF { get; set; }
            public long IdTransaction { get; set; }
            public string Recipient { get; set; }
            public long RecipientCUIT { get; set; }
            public string SubMerchantAddress { get; set; }
            public string RecipientAccountNumber { get; set; }
            public decimal GrossAmount { get; set; }
        }

        public class Providers 
        { 
            public string name { get; set; }

            public string code { get; set; }

            public string countryCode { get; set; }

            public int batchFileTxLimit { get; set; }
        }

        public class InternalStatus
        {
            public string name { get; set; }

            public string code { get; set; }

            public string countryCode { get; set; }

            public string providerCode { get; set; }

            public bool isError { get; set; }
        }
    }
}