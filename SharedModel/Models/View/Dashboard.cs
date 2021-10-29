using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SharedModel.Models.View
{
    public class Dashboard
    {
        public class DashboardLotes
        {

            /// <summary>
            /// [ES|EN][ID Lote|ID PayOut][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public Int64 idTransactionLot { get; set; }
            /// <summary>
            /// [ES|EN][Nombre del Cliente|Customer Name][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string customer_name { get; set; }
            /// <summary>
            /// [ES|EN][Tipo de Transacción|Transaction Type][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string transaction_type { get; set; }
            /// <summary>
            /// [ES|EN][Fecha de Acreditación|Accreditation Date][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string accreditation_date { get; set; }
            /// <summary>
            /// [ES|EN][Importe Bruto|Gross Amount][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string gross_amount { get; set; }
            /// <summary>
            /// [ES|EN][Importe Neto|Net Amount][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string net_amount { get; set; }
            public string lot_number { get; set; }
            /// <summary>
            /// [ES|EN][Retenciones de Impuestos|Tax Withholdings][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string tax_withholdings { get; set; }
            /// <summary>
            /// [ES|EN][Comisiones|Commissions][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string commissions { get; set; }
            /// <summary>
            /// [ES|EN][IVA|VAT][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string vat { get; set; }
            /// <summary>
            /// [ES|EN][Costo Banco|Bank Cost][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string bank_cost { get; set; }
            /// <summary>
            /// [ES|EN][IVA Costo Banco|VAT Bank Cost][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string bank_cost_vat { get; set; }
            /// <summary>
            /// [ES|EN][Percepción de Ingresos Brutos|Gross Revenue Perception][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string gross_revenue_perception { get; set; }
            /// <summary>
            /// [ES|EN][Impuesto al Débito|Debit Tax][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string tax_debit { get; set; }
            /// <summary>
            /// [ES|EN][Impuesto al Crédito|Credit Tax][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string tax_credit { get; set; }
            /// <summary>
            /// [ES|EN][Redondeo|Rounding][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string rounding { get; set; }
            /// <summary>
            /// [ES|EN][IVA a Pagar|VAT to Pay][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string pay_vat { get; set; }
            /// <summary>
            /// [ES|EN][Saldo|Balance][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string balance { get; set; }
            /// <summary>
            /// [ES|EN][Saldo Banco|Bank Balance][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string bank_balance { get; set; }
            /// <summary>
            /// [ES|EN][Estado|Status][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// [ES|EN][Listado de Transacciones|List of Transactions][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            private List<List.Response> pri_transaction_list = new List<List.Response>();
            public List<List.Response> transaction_list { get { return pri_transaction_list; } set { pri_transaction_list = value; } }

        }

        public class List
        {
            public class Request
            {
                public string idEntityAccount { get; set; }
                public string cycle { get; set; }
                public string id_status { get; set; }
                public string id_transactioType { get; set; }
                public string pageSize { get; set; }
                public string offset { get; set; }

            }
            public class Response
            {
                private Request _request = new Request();
                public Request request { get { return _request; } set { _request = value; } }

                private List<DashboardLotes> _dashboardLotes = new List<DashboardLotes>();
                public List<DashboardLotes> dashboardLotes { get { return _dashboardLotes; } set { _dashboardLotes = value; } }
            }
        }


        public class DashboardTransactions
        {
            public int transaction_id { get; set; }
            public DateTime TransactionDate { get; set; }
            public float amount { get; set; }
            public int transactionLot_id { get; set; }
            public int transactionType_id { get; set; }
            public int status_id { get; set; }
            public string status { get; set; }
            public int EntityAccount_id { get; set; }

            public int transactionRecipientDetail_id { get; set; }
            public string Recipient { get; set; }
            public string RecipientCUIT { get; set; }
            public string CBU { get; set; }
            public string RecipientAccountNumber { get; set; }
            public string TransactionAcreditationDate { get; set; }
            public string Description { get; set; }
            public string InternalDescription { get; set; }
            public string ConceptCode { get; set; }
            public string BankAccountType { get; set; }
            public string EntityIdentificationType { get; set; }
            public string CurrencyType { get; set; }
            public string PaymentType { get; set; }
            public float CreditTax { get; set; }
            public float DebitTax { get; set; }


        }
        public class ListTransaction
        {
            public class Request
            {
                public string idTransactionLot { get; set; }
                public string id_status { get; set; }
                public string pageSize { get; set; }
                public string offset { get; set; }

            }
            public class Response
            {
                /*  private Request _request = new Request();
                  public  Request request { get { return _request; } set { _request = value; } }

                  private List<DashboardTransactions> _dashboardTrans= new List<DashboardTransactions>();
                  public List<DashboardTransactions> dashboardTrans { get { return _dashboardTrans; } set { _dashboardTrans = value; } }
                  */
                public string TransactionList { get; set; }
            }
        }

        public class Indicators
        {
            public class Request
            {
                public string cycle { get; set; }

            }
            public class Response
            {
                public string indicators { get; set; }
            }
        }

        public class DollarToday
        {
            public class Response
            {
                public string dollarToday { get; set; }
            }
        }

        public class MainReport
        {
            public class Request
            {
                public string type { get; set; }

                public static explicit operator Request(Dictionary<string, string> data) => new Request
                {
                    type = data.ContainsKey("type") ? data["type"] : null
                };
            }
            public class Response
            {
                public string mainReportData { get; set; }
            }
        }

        public class ClientReport
        {
            public class Response
            {
                public string clientReportData { get; set; }
            }
        }

        public class ProviderCycle
        {
            public class ProviderTransaction
            {
                public int idProvider { get; set; }
                public string provider { get; set; }
                public string transactionType { get; set; }
                public double gross { get; set; }
                public double comission { get; set; }
                public double vat { get; set; }
                public double percIIBB { get; set; }
                public double percVat { get; set; }
                public double percProfit { get; set; }
                public double net { get; set; }
                public string cycle { get; set; }
                public DateTime? dateReceived { get; set; }

                public string date { get { return dateReceived != null ? dateReceived.Value.ToString("dd-MM-yyyy") : ""; } }
            }

            public class Response
            {
                private string data = string.Empty;
                public string Data { get { return data; } set { data = value; } }

                public class ResponseModel
                {
                    public ResponseModel(string data)
                    {
                        transactions = JsonConvert.DeserializeObject<List<ProviderTransaction>>(data);
                    }

                    //public ResponseModel(string data)
                    //{

                    //    JArray json = JArray.Parse(data);
                    //    foreach (JObject item in json)
                    //    {
                    //        if (!transactions.Any(o => o.provider == (string)((JValue)item["provider"]).Value))
                    //        {
                    //            transactions.Add(new ProviderTransaction { idProvider= Convert.ToInt32(((JValue)item["idProvider"]).Value),  provider = (string)((JValue)item["provider"]).Value });
                    //        }
                    //        ProviderTransaction p = transactions.First(o => o.provider == (string)((JValue)item["provider"]).Value);

                    //        p.transactionType = (string)((JValue)item["transactionType"]).Value;
                    //        p.gross = (double)((JValue)item["gross"]).Value;
                    //        p.comission = (double)((JValue)item["comission"]).Value;
                    //        p.vat = (double)((JValue)item["vat"]).Value;
                    //        p.percIIBB = (double)((JValue)item["percIIBB"]).Value;
                    //        p.percVat = (double)((JValue)item["percVat"]).Value;
                    //        p.percProfit = (double)((JValue)item["percProfit"]).Value;
                    //        p.net = (double)((JValue)item["net"]).Value;
                    //        p.cycle = (string)((JValue)item["cycle"]).Value;

                    //        if (item.ContainsKey("dateReceived"))
                    //        {
                    //            p.dateReceived = (DateTime)((JValue)item["dateReceived"]).Value;
                    //        }
                    //    }
                    //}

                    private List<ProviderTransaction> transactions = new List<ProviderTransaction>();
                    public List<ProviderTransaction> Transactions
                    {
                        get { return transactions; }
                        set { this.transactions = value; }
                    }
                }
            }
        }

        public class MerchantCycle
        {
            public class Response
            {
                public class MerchantsTransaction
                {
                    public Int32 idEntityMerchant { get; set; }
                    public string merchant { get; set; }
                    public string method { get; set; }
                    public double gross { get; set; }
                    public double comission { get; set; }
                    public double vat { get; set; }
                    public double ars { get; set; }
                    public double exrate { get; set; }
                    public double usd { get; set; }
                    public string cycle { get; set; }

                    public DateTime? payDate { get; set; }

                    public double exchange { get; set; }
                    public double revenueOper { get; set; }
                    public double revenueFx { get; set; }
                    public DateTime? StartCycle { get; set; }
                    public DateTime? EndCycle { get; set; }
                }

                private string data = string.Empty;
                public string Data { get { return data; } set { data = value; } }

                public class ResponseModel
                {
                    public ResponseModel(string data)
                    {
                        transactions = JsonConvert.DeserializeObject<List<MerchantsTransaction>>(data);                      
                    }

                    private List<MerchantsTransaction> transactions = new List<MerchantsTransaction>();
                    public List<MerchantsTransaction> Transactions
                    {
                        get { return transactions; }
                        set { this.transactions = value; }
                    }
                }
            }
        }

        public class CashProviderCycle
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter quote_currency is required.")]
                public string idProvider { get; set; }
                [Required(ErrorMessage = "Parameter quote_currency is required.")]
                public string cycle { get; set; }
                public General.ErrorModel.Error ErrorRow { get; set; } = new General.ErrorModel.Error();
            }

            public class Response : Request
            {
                public string Status { get; set; }
                public string StatusMessage { get; set; }
            }
        }
        public class CashMerchantCycle
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter :: idEntityMerchant :: is required.")]
                public string idEntityMerchant { get; set; }
                [Required(ErrorMessage = "Parameter :: StartCycle :: is required.")]
                public string StartCycle { get; set; }
                [Required(ErrorMessage = "Parameter :: EndCycle :: is required.")]
                public string EndCycle { get; set; }
                public General.ErrorModel.Error ErrorRow { get; set; } = new General.ErrorModel.Error();
            }

            public class Response : Request
            {
                public string Status { get; set; }
                public string StatusMessage { get; set; }
            }
        }
    }
}
