 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.View
{
    public class Report
    {
        public class List
        {
            public class Request
            {
                public string customer_id { get; set; }
                public string idEntityAccount { get; set; }
                public string cycle { get; set; }
                public string id_status { get; set; }
                public string id_transactioOper { get; set; }
                public string pageSize { get; set; }
                public string offset { get; set; }
                public string date { get; set; }
                public string dateFrom { get; set; }
                public string dateTo { get; set; }
                public string lotFrom { get; set; }
                public string lotTo { get; set; }
                public string merchant { get; set; }
                public string grossSign { get; set; }
                public string grossAmount { get; set; }
                public string netSign { get; set; }
                public string netAmount { get; set; }
                public string payMethod { get; set; }
                public string currency { get; set; }
                public string idEntitySubMerchant { get; set; }
                public string transactionType { get; set; }
                public string idReg { get; set; }
                public string cuit { get; set; }
                public string idTransaction { get; set; }
                public string month { get; set; }
                public string idEntityUser { get; set; }
                public string amount { get; set; }

                public string idTransactionType { get; set; }
                public string ticket { get; set; }
                public string merchantId { get; set; }
                public string idField { get; set; }
                public string idErrorType { get; set; }

                public string lotId { get; set; }
                public string countryCode { get; set; }
                public int orderBy { get; set; }
                public int orderType { get; set; }
                public int dateFilterBy { get; set; }


            }
            public class Response
            {
                private Request _request = new Request();
                public Request request { get { return _request; } set { _request = value; } }

                public string TransactionList { get; set; }
            }

            public class DataReport
            {

                public DateTime? ProcessedDate { get; set; }
                public DateTime? ProviderDate { get; set; }
                public DateTime? PaymentDate { get; set; }
                public DateTime? TransactionDate { get; set; }
                public DateTime? CollectionDate { get; set; }
                public DateTime? LotOutDate { get; set; }

                public string PayMethod { get; set; }
                public string idTransaction { get; set; }
                public string Provider { get; set; }
                public string TransactionOperation { get; set; }
                public string Mechanism { get; set; }
                public string LotNumber { get; set; }
                public string InternalClient_id { get; set; }
                public string Status { get; set; }
                public string DetailStatus { get; set; }
                public string Ticket { get; set; }
                public bool? Pay { get; set; }
                public bool? Cashed { get; set; }
                public bool? PayOut { get; set; }
                public string Merchant { get; set; }
                public string SubMerchantIdentification { get; set; }
                public string Identification { get; set; }
                public string LotOutId { get; set; }

                //Merchant
                public Decimal Amount { get; set; }
                public Decimal Withholding { get; set; }
                public Decimal WithholdingArba { get; set; }
                public Decimal Payable { get; set; }
                public Decimal FxMerchant { get; set; }
                public Decimal Pending { get; set; }
                public Decimal Confirmed { get; set; }
                public Decimal Com { get; set; }
                public Decimal NetCom { get; set; }
                public Decimal TotCom { get; set; }
                public Decimal TaxCountry { get; set; }
                public Decimal AccountArs { get; set; }
                public Decimal AccountUsd { get; set; }
            
                //Admin
                public Decimal ProviderCost { get; set; }
                public Decimal VatCostProv { get; set; }
                public Decimal TotalCostProv { get; set; }
                public Decimal PercIIBB { get; set; }
                public Decimal PercVat { get; set; }
                public Decimal PercProfit { get; set; }
                public Decimal PercOthers { get; set; }
                public Decimal Sircreb { get; set; }
                public Decimal TaxDebit { get; set; }
                public Decimal TaxCredit { get; set; }
                public Decimal RdoOperative { get; set; }
                public Decimal VatToPay { get; set; }
                public Decimal FxLP { get; set; }
                public Decimal RdoFx { get; set; }
                public Decimal AccountWhitoutCommission { get; set; }
                public Decimal PendingAtLPFx { get; set; }

                //public string Cash_VAT_Prov { get; set; }
                //public string Commission_Prov { get; set; }
                //public string Commission_With_Cash_Prov { get; set; }
                //public string Commission_With_VAT_Prov { get; set; }
                //public string Commission_Merchant { get; set; }
                //public string VAT_Merchant { get; set; }
                //public string Commission_whit_vat_Merchant { get; set; }




                //public string GrossValueClient { get; set; }
                //public string TaxWithholdings { get; set; }

                //public string Commission_Vat_Prov { get; set; }
                //public string Profit_Perception_prov { get; set; }
                //public string Vat_Perception_prov { get; set; }
                //public string Gross_Revenue_Perception_CABA_Prov { get; set; }
                //public string Gross_Revenue_Perception_BSAS_Prov { get; set; }
                //public string Gross_Revenue_Perception_OTHER_Prov { get; set; }
                ////public string Sircreb { get; set; }
                //public string NetPendingAmount_Prov { get; set; }
                //public string NetInTermAmount_Prov { get; set; }
                //public string NetPendingAmount_Merchant { get; set; }
                //public string NetInTermAmount_Merchant { get; set; }
                //public string NetPendingAmount_Merchant_USD { get; set; }
                //public string NetInTermAmount_Merchant_USD { get; set; }
                //public string RevenueOp { get; set; }
                //public string RevenuePendingOp { get; set; }
                //public string RevenueInTermOp { get; set; }
                //public string FxRate { get; set; }
                //public string ExRateAmountSale { get; set; }
                //public string FxRevenue { get; set; }
                //public string tax_debit { get; set; }
                //public string tax_credit { get; set; }
                //public string Bank_Cost { get; set; }
                //public string Bank_Cost_VAT { get; set; }
                //public string TotalCostRdo { get; set; }
            }
            public class DetailTransactionClient
            {

                public string Recipient { get; set; }
                public string LotNumber { get; set; }
                public string Merchant { get; set; }
                public string SubMerchantIdentification { get; set; }

                public string TransactionType { get; set; }

                public string Address { get; set; }
                public string Birthdate { get; set; }
                public string Country { get; set; }
                public string City { get; set; }
                public string Email { get; set; }
                public string RecipientCUIT { get; set; }
                public string CBU { get; set; }
                public DateTime? TransactionDate { get; set; }
                public Decimal GrossValueClient { get; set; }
                public Decimal GrossValueClientUsd { get; set; }
                public string CurrencyType { get; set; }
                public string CurrencyTypeUsd { get; set; }
                public string idTransaction { get; set; }
                public string Ticket { get; set; }
                public string AlternativeTicket { get; set; }
                public string InternalDescription { get; set; }
                public string BankBranch { get; set; }
                public string RecipientPhoneNumber { get; set; }
                public string SenderEmail { get; set; }
                public string SenderPhoneNumber { get; set; }
                public string Status { get; set; }
                public string DetailStatus { get; set; }
                public string ProcessedDate { get; set; }
                public string BankCode { get; set; }
                public decimal LocalTax { get; set; }
                public string idLotOut { get; set; }
                public string LotOutDate { get; set; }

            }
            public class OperationRetention
            {

                public string idTransaction { get; set; }
                public DateTime? TransactionDate { get; set; }
                public DateTime? ProcessedDate { get; set; }
                public string Recipient { get; set; }
                public string RecipientCUIT { get; set; }
                public string Merchant { get; set; }

                public string Description { get; set; }
                public string IdTransaction { get; set; }
                public string Ticket { get; set; }
                public string MerchantId { get; set; }
                public string FileName { get; set; }
                public string CertificateNumber { get; set; }
                public Decimal GrossAmount { get; set; }
                public Decimal TaxWithholdings { get; set; }
                public string Retention { get; set; }
                public Decimal NetAmount { get; set; }
                public Decimal GrossUSD { get; set; }
                public string CBU { get; set; }

                public string NroRegimen { get; set; }
            }

            public class HistoricalFX
            {
                public string Merchant { get; set; }
                public DateTime? ProcessDate { get; set; }
                public string Buy { get; set; }
                public string Base_Buy { get; set; }
                public string Spot { get; set; }
                public string Base_Sell { get; set; }
                public string Sell { get; set; }
            }

            public class MerchantReport
            {
                public string ProviderName { get; set; }
                public string Ccy { get; set; }
                public string Date { get; set; }
                public string AccountNumber { get; set; }
                public string Bic { get; set; }
                public string TrxType { get; set; }
                public string Description { get; set; }
                public string PayoneerId { get; set; }
                public string InternalId { get; set; }
                public Decimal Debit { get; set; }
                public Decimal Credit { get; set; }
                public Decimal AvailableBalance { get; set; }
            }

            public class MerchantReport_CSV_Format
            {
                [Description("Provider Name")]
                public string ProviderName { get; set; }
                [Description("CCY")]
                public string Ccy { get; set; }
                public string Date { get; set; }
                [Description("account number")]
                public string AccountNumber { get; set; }
                [Description("BIC")]
                public string Bic { get; set; }
                [Description("Trax Type")]
                public string TrxType { get; set; }
                public string Description { get; set; }
                [Description("Payoneer ID")]
                public string PayoneerId { get; set; }
                [Description("Internal ID")]
                public string InternalId { get; set; }
                public string Debit { get; set; }
                public string Credit { get; set; }
                [Description("Available Balance")]
                public string AvailableBalance { get; set; }
            }

        }
    }
}
