using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharedModel.ValidationsAttrs;
using SharedModel.ValidationsAttrs.Mexico;

namespace SharedModel.Models.Services.Mexico
{
    public class PayOutMexico
    {
        public class LotBatch
        {
            /// <summary>
            /// [ES|EN][ID Lote|ID PayOut][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public Int64 payout_id { get; set; }
            /// <summary>
            /// [ES|EN][Nombre del Cliente|Customer Name][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string customer_name { get; set; }
            /// <summary>
            /// [ES|EN][Tipo de Transacción|Transaction Type][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string transaction_type { get; set; }
            /// <summary>
            /// [ES|EN][Estado|Status][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// [ES|EN][Fecha de Proceso|Process Date][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public string lot_date { get; set; }
            /// <summary>
            /// [ES|EN][Importe Bruto|Gross Amount][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public Int64 gross_amount { get; set; }
            /// <summary>
            /// [ES|EN][Importe Neto|Net Amount][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            public Int64 net_amount { get; set; }
            /// <summary>
            /// [ES|EN][Saldo|Balance][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            //public Int64 account_balance { get; set; }
            /// <summary>
            /// [ES|EN][Listado de Transacciones|List of Transactions][{LECTURA|ESCRITURA}{READ|WRITE}]
            /// </summary>
            private List<List.Response> pri_transaction_list = new List<List.Response>();
            public List<List.Response> transaction_list { get { return pri_transaction_list; } set { pri_transaction_list = value; } }

            //public string error { get; set; }

        }

        public class Create
        {
            public class Request
            {
                #region Transaction Detail
                [JsonProperty(Order = 21)]
                [Required(ErrorMessage = "Parameter :: merchant_id :: is required.#REQUIRED")]
                [StringLength(60, MinimumLength = 0, ErrorMessage = "Parameter :: merchant_id :: has minimun 0 characters and 60 characters maximum.#LENGTH")]
                [RegularExpression("^[a-zA-Z0-9\\s]{0,60}$", ErrorMessage = "Parameter :: merchant_id :: invalid format, only allow: letters and spaces.#INVALID")]
                public string merchant_id { get; set; }
                [StringLength(4, MinimumLength = 4, ErrorMessage = "Parameter :: concept_code :: has 4 characters.#LENGTH")]
                [JsonProperty(Order = 22)]
                public string concept_code { get; set; }
                [JsonProperty(Order = 24)]
                [Required(ErrorMessage = "Parameter :: currency :: is required.#REQUIRED")]
                [RegularExpression("^USD$|^MXN$", ErrorMessage = "Parameter :: currency :: invalid format, only allow codes: 'USD' | 'MXN'.#INVALID")]
                public string currency { get; set; }

                [StringLength(8, MinimumLength = 8, ErrorMessage = "Parameter :: payout_date :: has 8 characters length.#LENGTH")]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: payout_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).#INVALID")]

                private string _payout_date { get; set; }
                [JsonProperty(Order = 25)]
                [ExpiredDate(ErrorMessage = "Parameter :: payout_date :: invalid date: it must be greater than actual date.#INVALID")]
                public string payout_date { get { return this._payout_date == null || string.IsNullOrEmpty(this._payout_date) ? DateTime.Now.ToString("yyyyMMdd") : this._payout_date; } set { this._payout_date = value; } }


                [JsonProperty(Order = 26)]
                [Required(ErrorMessage = "Parameter :: amount :: is required.#REQUIRED")]
                [Range(100, 1000000000, ErrorMessage = "Parameter :: amount :: value out range [100 - 1000000000].#INVALID")]
                [RegularExpression("^[0-9]{3,10}$", ErrorMessage = "Parameter :: amount :: invalid length, must be between 3 and 10 characters.#LENGTH")]
                public Int64 amount { get; set; }

                #endregion

                #region Sender Information
                [JsonProperty(Order = 4)]
                [Required(ErrorMessage = "Parameter :: submerchant_code :: is required.#REQUIRED")]
                [StringLength(60, MinimumLength = 3, ErrorMessage = "Parameter :: submerchant_code :: has minimun 3 characters and 60 characters maximum.#LENGTH")]
                [RegularExpression("^[a-zA-Z0-9\\s\\-_]{3,60}$", ErrorMessage = "Parameter :: submerchant_code :: invalid format, only allow numbers, letters chars [-_.] and spaces, and length should be between 3 and 60 characters.#INVALID")]
                public string submerchant_code { get; set; } = "";
                [JsonProperty(Order = 5)]
                [RegularExpression("^[a-zA-Z0-9\\s]{0,22}$", ErrorMessage = "Parameter :: sender_taxid :: invalid format, only allow: letters and spaces, and length has between 1 and 20 characters.")]
                public string sender_taxid { get; set; }
                [JsonProperty(Order = 6)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: sender_name :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                public string sender_name { get; set; }
                [JsonProperty(Order = 7)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,300}$", ErrorMessage = "Parameter :: sender_address :: invalid format, only allow: letters and spaces, and length has between 1 and 300 characters.")]
                public string sender_address { get; set; }
                [JsonProperty(Order = 8)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,20}$", ErrorMessage = "Parameter :: sender_state :: invalid format, only allow: letters and spaces, and length has between 1 and 20 characters.")]
                public string sender_state { get; set; }
                [JsonProperty(Order = 9)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,20}$", ErrorMessage = "Parameter :: sender_country :: invalid format, only allow: letters and spaces, and length has between 1 and 20 characters.")]
                public string sender_country { get; set; }
                [JsonProperty(Order = 10)]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: sender_birthdate :: invalid value: allow only date format, length 8 characters (YYYMMDD).#INVALID")]
                public string sender_birthdate { get; set; }
                [JsonProperty(Order = 11)]
                [EmailAddress(ErrorMessage = "Parameter :: sender_email :: is not a valid mail.#INVALID")]
                public string sender_email { get; set; }

                [JsonProperty(Order = 11)]
                [Phone(ErrorMessage = "Parameter :: sender_phone_number :: is not a valid phone number.#INVALID")]
                public string sender_phone_number { get; set; }

                [JsonProperty(Order = 11)]
                [RegularExpression("^[A-Za-z0-9-]{1,10}$", ErrorMessage = "Parameter :: sender_zip_code :: invalid value: allow only: letters, numbers and hyphen.#INVALID")]
                public string sender_zip_code { get; set; }
                #endregion

                #region Beneficiario
                [JsonProperty(Order = 12)]
                [Required(ErrorMessage = "Parameter :: beneficiary_name :: is required.#REQUIRED")]
                [StringLength(60, MinimumLength = 1, ErrorMessage = "Parameter :: beneficiary_name :: has minimun 1 characters and 60 characters maximum.#LENGTH")]
                [RegularExpression("^([A-Za-z0-9\\u00C0-\\u00D6\\u00D8-\\u00f6\\u00f8-\\u00ff\\s][^\r\n]{1,60})$", ErrorMessage = "Parameter :: beneficiary_name :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.#INVALID")]
                public string beneficiary_name { get; set; } = "";

                //[Required(ErrorMessage = "Parameter :: beneficiary_document_id :: is required.#REQUIRED")]
                //[StringLength(13, MinimumLength = 12, ErrorMessage = "Parameter :: beneficiary_document_id :: invalid length .#INVALID")]
                [RegularExpression("^([A-ZÑ&]{3,4}) ?(?:- ?)?(\\d{2}(?:(?:0[1-9]|1[0-2])(?:0[1-9]|1[0-9]|2[0-8])|(?:0[469]|11)(?:29|30)|(?:0[13578]|1[02])(?:29|3[01]))|(?:0[048]|[2468][048]|[13579][26])0229) ?(?:- ?)?([A-Z\\d]{2})([A\\d])$", ErrorMessage = "Parameter :: beneficiary_document_id :: invalid RFC .#INVALID")]
                public string beneficiary_document_id { get; set; } = "";

                [Required(ErrorMessage = "Parameter :: bank_code :: is required.#REQUIRED")]
                [StringLength(3, MinimumLength = 3, ErrorMessage = "Parameter :: bank_code :: length must be 3.#LENGTH")]
                [ValidationsAttrs.Mexico.BankCode(ErrorMessage = "Parameter :: bank_code :: is invalid")]
                public string bank_code { get; set; }

                [JsonProperty(Order = 14)]
                [Required(ErrorMessage = "Parameter :: bank_account_type :: is required.#REQUIRED")]
                [StringLength(1, MinimumLength = 1, ErrorMessage = "Parameter :: bank_account_type :: has 1 characters length.#LENGTH")]
                [RegularExpression("^[AC]{1}$", ErrorMessage = "Parameter :: bank_account_type :: invalid format, only characters available: 'C' – checking account, 'A' – savings account.#INVALID")]
                public string bank_account_type { get; set; }

                [JsonProperty(Order = 15)]
                [Required(ErrorMessage = "Parameter :: beneficiary_account_number :: is required.#REQUIRED")]
                [StringLength(18, MinimumLength = 18, ErrorMessage = "Parameter :: beneficiary_account_number :: invalid length.#LENGTH")]
                [BeneficiaryAccountNumber(ErrorMessage = "Parameter :: beneficiary_account_number :: is invalid")]
                public string beneficiary_account_number { get; set; } = "";

                [JsonProperty(Order = 16)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,300}$", ErrorMessage = "Parameter :: beneficiary_address :: invalid format, only allow: letters, numbers and spaces, and length has between 1 and 300 characters.#INVALID")]
                public string beneficiary_address { get; set; }
                [JsonProperty(Order = 17)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: beneficiary_state :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.#INVALID")]
                public string beneficiary_state { get; set; }
                [JsonProperty(Order = 18)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: beneficiary_country :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.#INVALID")]
                public string beneficiary_country { get; set; }
                [JsonProperty(Order = 19)]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: beneficiary_birth_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).#INVALID")]
                public string beneficiary_birth_date { get; set; }
                [JsonProperty(Order = 20)]
                [EmailAddress(ErrorMessage = "Parameter :: beneficiary_email :: is not a valid mail.#INVALID")]
                public string beneficiary_email { get; set; }

                [JsonProperty(Order = 20)]
                [Phone(ErrorMessage = "Parameter :: beneficiary_phone_number :: is not a valid phone number.#INVALID")]
                public string beneficiary_phone_number { get; set; }

                [JsonProperty(Order = 20)]
                [RegularExpression("^[A-Za-z0-9-]{1,10}$", ErrorMessage = "Parameter :: beneficiary_zip_code :: invalid value: allow only: letters, numbers and hyphen.#INVALID")]
                public string beneficiary_zip_code { get; set; }
                #endregion
                public bool authenticate { get; set; }
                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                [JsonProperty(Order = 31)]
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }

            public class Request<T> where T : class
            {
            }

            public class Response : Request
            {
                #region Transaction Information
                [JsonProperty(Order = 0)]
                public Int64 transaction_id { get; set; }
                [JsonProperty(Order = 1)]
                public Int64 payout_id { get; set; }
                [JsonProperty(Order = 2)]
                public string status { get; set; }
                [JsonProperty(Order = 3)]
                public string Ticket { get; set; }
                #endregion
                [JsonProperty(Order = 27)]
                public Int64 exchange_rate { get; set; }
            }
        }

        public class Update
        {

        }

        public class Delete
        {
            public class Request
            {
                [RegularExpression("^[0-9]*$", ErrorMessage = "Parameter :: transaction_id :: invalid format, only allow: numbers.")]
                private long? _transaction_id { get; set; }

                public long? transaction_id { get { return _transaction_id == null || _transaction_id <= 0 ? -1 : _transaction_id; } set { _transaction_id = value; } }
                [Required(ErrorMessage = "Parameter :: payout_id :: is required.")]
                [Range(1, Int64.MaxValue, ErrorMessage = "Parameter :: payout_id :: invalid value, must be greater than 0.")]
                [RegularExpression("^[0-9]*$", ErrorMessage = "Parameter :: payout_id :: invalid format, only allow: numbers.")]
                public Int64 payout_id { get; set; }
                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }
            public class Response : Request
            {
                public string status { get; set; }
            }
        }

        public class List
        {
            public class Request
            {

                [RegularExpression("^[0-9]*$", ErrorMessage = "Parameter :: payout_id :: invalid format, only allow: numbers.")]
                public Int64 payout_id { get; set; }
                [RegularExpression("^[0-9]*$", ErrorMessage = "Parameter :: transaction_id :: invalid format, only allow: numbers.")]
                public Int64 transaction_id { get; set; }
                [StringLength(60, MinimumLength = 0, ErrorMessage = "Parameter :: merchant_id :: has minimun 0 characters and 60 characters maximum.#LENGTH")]
                [RegularExpression("^[a-zA-Z0-9\\s]{0,60}$", ErrorMessage = "Parameter :: merchant_id :: invalid format, only allow: letters and spaces.#INVALID")]
                public string merchant_id { get; set; }
                [StringLength(8, MinimumLength = 8, ErrorMessage = "Parameter :: date_from :: has 8 characters length.")]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: payout_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string date_from { get; set; }
                [StringLength(8, MinimumLength = 8, ErrorMessage = "Parameter :: date_to :: has 8 characters length.")]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: payout_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string date_to { get; set; }
                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }
            public class Response //: Request
            {
                public Int64 transaction_id { get; set; }
                public string beneficiary_document_type { get; set; }
                public string beneficiary_document_number { get; set; }
                public string beneficiary_name { get; set; }
                public string bank_account_type { get; set; }
                public string bank_code { get; set; }
                public string beneficiary_account_number { get; set; }
                public long amount { get; set; }
                public string transaction_date { get; set; }
                public string merchant_id { get; set; }
                public string concept_code { get; set; }
                public string currency { get; set; }
                public string status { get; set; }
                public string status_detail { get; set; }
                public Int64 exchange_rate { get; set; }
                public string submerchant_code { get; set; }
                public string sender_name { get; set; }
                public string sender_address { get; set; }
                public string sender_state { get; set; }
                public string sender_country { get; set; }
                public string sender_taxid { get; set; }
                public string sender_birthdate { get; set; }
                public string sender_email { get; set; }
                public string sender_phone_number { get; set; }
                public string beneficiary_phone_number { get; set; }
            }
        }

        public class DownloadLotBatchTransactionToBank
        {
            public class Request
            {
                public int PaymentType { get; set; }
                public string idMerchant { get; set; }
                public string idSubMerchant { get; set; }

                public string dateTo { get; set; }

                public string amount { get; set; }
                public int operationType { get; set; }
                public int includeDuplicateAmounts { get; set; }
                public int internalFiles { get; set; }
            }

            public class Response
            {
                public List<PayoutFile> PayoutFiles { get;set; }

                public int RowsPreRegister { get; set; }
                public string[] LinesPreRegister { get; set; }
                public string FileBase64_PreRegister { get; set; }

                public string PreRegisterLot { get; set; }
                public int idLotOut { get; set; }

                public string Status { get; set; }
                public string StatusMessage { get; set; }

                public int DownloadCount { get; set; }
                
            }

            public class PayoutFile
            {
                public int RowsPayouts { get; set; }
                public List<string> LinesPayouts { get; set; }
                public string FileBase64_Payouts { get; set; }
                public string FileTotal { get; set; }
            }
        }

        public class UploadTxtFromBank
        {
            public class Request
            {
                public string File { get; set; }
                public int CurrencyFxClose { get; set; }
                public string FileName { get; set; }
            }
            public class Response
            {
                public int Rows { get; set; }
                public string[] Lines { get; set; }
                public string Status { get; set; }
                public string StatusMessage { get; set; }
                public string FileBase64 { get; set; }
                public BatchLotDetail BatchLotDetail { get; set; }
                private List<TransactionDetail> _TransactionDetail = new List<TransactionDetail>();
                public List<TransactionDetail> TransactionDetail { get { return _TransactionDetail; } set { _TransactionDetail = value; } }
            }
            public class BatchLotDetail
            {
                public string InternalStatus { get; set; }
                public string InternalStatusDescription { get; set; }
            }
            public class TransactionDetail
            {
                public string Ticket { get; set; }
                public DateTime TransactionDate { get; set; }
                public decimal Amount { get; set; }
                public string Currency { get; set; }
                public Int64 LotNumber { get; set; }
                public string LotCode { get; set; }
                public string Recipient { get; set; }
                public string RecipientId { get; set; }
                public string RecipientCUIT { get; set; }
                public string RecipientCBU { get; set; }
                public string RecipientAccountNumber { get; set; }
                public DateTime AcreditationDate { get; set; }
                public string Description { get; set; }
                public string InternalDescription { get; set; }
                public string ConceptCode { get; set; }
                public string BankAccountType { get; set; }
                public string EntityIdentificationType { get; set; }
                public string InternalStatus { get; set; }
                public string InternalStatusDescription { get; set; }
                public string idEntityUser { get; set; }
                public string TransactionId { get; set; }
                public string StatusCode { get; set; }
            }
        }

        public class DownloadPayOutBankTxt
        {
            public enum FileType
            {
                [Description("*H2")]
                H2 = 1,
                [Description("*P2")]
                P2 = 2
            }
            public enum CurrencyType
            {
                [Description("$")]
                ARS = 1,
                [Description("U$S")]
                DOLAR = 2
            }
            public enum PayType
            {
                [Description("Pago de Haberes")]
                PAGO_DE_HABERES = 1,
                [Description("Pago a Proveedores")]
                PAGO_A_PROVEEDORES = 2
            }
            public class Header
            {
                /* string part :: from: 1 | to: 3 | length: 3 */
                [Required(ErrorMessage = "Parameter :: file_type :: is required.")]
                [RegularExpression("^[*H2|*P2]{3}$", ErrorMessage = "Parameter :: file_type :: invalid format, only allow: one number: '1' – Payroll payments, '2' – Suppliers payments.")]
                public string file_type { get; set; }
                ///* string part :: from: 4 | to: 9 | length: 6 */
                [Required(ErrorMessage = "Parameter :: business_code :: is required.")]
                [RegularExpression("^[0-9]{6,6}$", ErrorMessage = "Parameter :: business_code :: invalid format, only allow: numbers, length 6 characters.")]
                public string business_code { get; set; }
                /* string part :: from: 10 | to: 20 | length: 11 */
                [Required(ErrorMessage = "Parameter :: business_cuit :: is required.")]
                [RegularExpression("^[0-9]{11,11}$", ErrorMessage = "Parameter :: business_cuit :: invalid format, only allow: numbers, length 11 digits.")]
                public string business_cuit { get; set; }
                /* string part :: from: 21 | to: 21 | length: 1 */
                [RegularExpression("^[A|C|\\s]{1}$", ErrorMessage = "Parameter :: account_type :: invalid format, only allow 1 letter or 1 space: 'A' ==> Caja de Ahorro | 'C' ==> Cuenta Corriente | ' '.")]
                public string account_type { get; set; }
                /* string part :: from: 22 | to: 22 | length: 1 */
                [Required(ErrorMessage = "Parameter :: currency_type :: is required.")]
                //[RegularExpression("^[$]{1}$|^[U$S]{3}$", ErrorMessage = "Parameter :: currency_type :: invalid format, only allow: one of two digits: $ | U$S.")]
                [RegularExpression("^[1-2]{1}$", ErrorMessage = "Parameter :: currency_type :: invalid format, only allow: one of two digits: 1: $ | 2: U$S.")]
                public string currency_type { get; set; }
                ///* string part :: from: 23 | to: 34 | length: 12 */
                private string pri_account_debit { get; set; }
                public string account_debit
                {
                    get
                    {
                        string format_str = "000000000000";
                        Regex regex = new Regex("^[0-9]{12,12}$");
                        Match match = regex.Match(pri_account_debit);
                        return string.IsNullOrEmpty(pri_account_debit) || !match.Success ? format_str : pri_account_debit;
                    }
                    set
                    {
                        pri_account_debit = value;
                    }
                }
                ///* string part :: from: 35 | to: 60 | length: 26 */
                private string pri_cbu { get; set; }
                public string cbu
                {
                    get
                    {
                        string format_str = "00000000000000000000000000";
                        Regex regex = new Regex("^[0-9]{26,26}$");
                        Match match = regex.Match(pri_account_debit);
                        return string.IsNullOrEmpty(pri_cbu) || !match.Success ? format_str : pri_cbu;
                    }
                    set
                    {
                        pri_cbu = value;
                    }
                }
                ///* string part :: from: 61 | to: 74 | length: 14 */
                [Required(ErrorMessage = "Parameter :: total_amount :: is required.")]
                [RegularExpression("^[0-9]{14,14}$", ErrorMessage = "Parameter :: total_amount :: invalid format, only allow: numbers, length 14 digits.")]
                public string total_amount { get; set; }
                ///* string part :: from: 75 | to: 82 | length: 8 */
                [Required(ErrorMessage = "Parameter :: availability_date :: is required.")]
                [RegularExpression("^((19|20)\\d{2})((0|1)\\d{1})((0|1|2)\\d{1})", ErrorMessage = "Parameter::availability_date :: invalid format, format: AAAAMMDD.")]
                public string availability_date { get; set; }
                /* string part :: from: 83 | to: 150 | length: 68 */
                public string filler { get { return "                                                                    "; } }
                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
                public string FormatLine()
                {
                    return this.file_type + this.business_code + this.business_cuit + this.account_type + this.currency_type + this.account_debit + this.cbu + this.total_amount + this.availability_date + this.filler;
                }
            }
            public class Body
            {
                [Required(ErrorMessage = "Parameter :: beneficiary_name :: is required.")]
                [RegularExpression("^[A-Z\\s]{16,16}$", ErrorMessage = "Parameter :: beneficiary_name :: invalid format, only allow: letters and spaces, length 16 characters.")]
                public string beneficiary_name { get; set; }
                [Required(ErrorMessage = "Parameter :: beneficiary_cuit :: is required.")]
                [RegularExpression("^[0-9]{11,11}$", ErrorMessage = "Parameter :: beneficiary_cuit :: invalid format, only allow: numbers, length 11 digits.")]
                public string beneficiary_cuit { get; set; }
                [Required(ErrorMessage = "Parameter :: availability_date :: is required.")]
                [RegularExpression("^((19|20)\\d{2})((0|1)\\d{1})((0|1|2)\\d{1})", ErrorMessage = "Parameter::availability_date :: invalid format, format: AAAAMMDD.")]
                public string availability_date { get; set; }
                [Required(ErrorMessage = "Parameter :: account_type :: is required.")]
                [RegularExpression("^[C|A]{1}$", ErrorMessage = "Parameter :: account_type :: invalid format, only allow: one letter 'A': Caja de Ahorro | 'C': Cuenta corriente.")]
                public string account_type { get; set; }
                [Required(ErrorMessage = "Parameter :: currency_type :: is required.")]
                [RegularExpression("^[1-2]{1}$", ErrorMessage = "Parameter :: currency_type :: invalid format, only allow: one of two digits: 1: $ | 2: U$S.")]
                public string currency_type { get; set; }
                [RegularExpression("^[0-9]{12}$", ErrorMessage = "Parameter :: account_number :: invalid format, only allow: numbers: length 12 characters.")]
                public string account_number { get; set; }
                [RegularExpression("^[0-9]{26}$", ErrorMessage = "Parameter :: cbu :: invalid format, only allow: numbers: length 26 characters.")]
                public string cbu { get; set; }
                [Required(ErrorMessage = "Parameter :: transaction_code :: is required.")]
                public string transaction_code { get { return "32"; } }
                [Required(ErrorMessage = "Parameter :: pay_type :: is required.")]
                [RegularExpression("^[1-2]{1}$", ErrorMessage = "Parameter :: pay_type :: invalid format, only allow: one number: 1: Pago de Haberes | 2: Pago a Proveedores.")]
                public string pay_type { get; set; }
                [Required(ErrorMessage = "Parameter :: amount :: is required.")]
                [RegularExpression("^[0-9]{14}$", ErrorMessage = "Parameter :: amount :: invalid format, only allow: numbers: length 14 characters.")]
                public string amount { get; set; }
                private string pri_legend { get; set; }
                public string legend
                {
                    get
                    {
                        string format_str = "               ";
                        Regex regex = new Regex("^[a-zA-Z0-9\\s]{15}$");
                        Match match = regex.Match(pri_legend == null ? format_str : pri_legend);
                        return string.IsNullOrEmpty(pri_legend) || !match.Success ? format_str : pri_legend;
                    }
                    set
                    {
                        pri_legend = value;
                    }
                }
                private string pri_internal_identifiaction { get; set; }
                public string internal_identification
                {
                    get
                    {
                        string format_str = "                      ";
                        Regex regex = new Regex("^[a-zA-Z0-9\\s]{22}$");
                        Match match = regex.Match(pri_internal_identifiaction == null ? format_str : pri_internal_identifiaction);
                        return string.IsNullOrEmpty(pri_internal_identifiaction) || !match.Success ? format_str : pri_internal_identifiaction;
                    }
                    set
                    {

                        pri_internal_identifiaction = value;
                    }
                }
                private string pri_process_date { get; set; }
                public string process_date
                {
                    get
                    {
                        string format_str = "        ";
                        Regex regex = new Regex("^((19|20)\\d{2})((0|1)\\d{1})((0|1|2)\\d{1})");
                        Match match = regex.Match(pri_process_date == null ? format_str : pri_process_date);
                        return string.IsNullOrEmpty(pri_process_date) || !match.Success ? format_str : pri_process_date;
                    }
                    set
                    {
                        pri_process_date = value;
                    }
                }
                private string pri_concept_code { get; set; }
                public string concept_code
                {
                    get
                    {
                        string format_str = "  ";
                        Regex regex = new Regex("^[01]{2}$|^[02]{2}$|^[03]{2}$|^[04]{2}$|^[05]{2}$|^[06]{2}$|^[07]{2}$|^[08]{2}$|^[09]{2}$|^[10]{2}$|^[11]{2}$|^[12]{2}$|^[\\s\\s]{2}$");
                        Match match = regex.Match(pri_concept_code == null ? format_str : pri_concept_code);
                        return string.IsNullOrEmpty(pri_concept_code) || !match.Success ? format_str : pri_concept_code;
                    }
                    set
                    {
                        pri_concept_code = value;
                    }
                }
                private string pri_payment_to_commerce { get; set; }
                public string payment_to_commerce
                {
                    get
                    {
                        string format_str = "  ";
                        Regex regex = new Regex("^[PC]{2}$");
                        Match match = regex.Match(pri_payment_to_commerce == null ? format_str : pri_payment_to_commerce);
                        return string.IsNullOrEmpty(pri_payment_to_commerce) || !match.Success ? format_str : pri_payment_to_commerce;
                    }
                    set
                    {
                        pri_payment_to_commerce = value;
                    }
                }
                internal virtual string filler { get { return "         "; } }
                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
                public string FormatLine()
                {
                    return this.beneficiary_name + this.beneficiary_cuit + this.availability_date + this.account_type + this.currency_type + this.account_number + this.cbu + this.transaction_code + this.pay_type + this.amount + this.legend + this.internal_identification + this.process_date + this.concept_code + this.payment_to_commerce + this.filler;
                }
            }
            public class Footer
            {
                public string registry_type { get { return "*F"; } }
                [Required(ErrorMessage = "Parameter :: business_code :: is required.")]
                [RegularExpression("^[0-9]{6,6}$", ErrorMessage = "Parameter :: business_code :: invalid format, only allow: numbers, length 6 characters.")]
                public string business_code { get; set; }
                [Required(ErrorMessage = "Parameter :: total_rows :: is required.")]
                [RegularExpression("^[0-9]{7,7}$", ErrorMessage = "Parameter :: total_rows :: invalid format, only allow: numbers, length 7 characters.")]
                public string total_rows { get; set; }
                internal virtual string filler { get { return "                                                                                                                                       "; } }
                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
                public string FormatLine()
                {
                    return this.registry_type + this.business_code + this.total_rows + this.filler;
                }
            }
        }

        public class ErrorsCreateLog
        {
            public string idTransactionType { get; set; }
            public string idEntityUser { get; set; }
            public string beneficiaryName { get; set; }
            public string typeOfId { get; set; }
            public string beneficiaryId { get; set; }
            public string cbu { get; set; }
            public string bankCode { get; set; }
            public string accountType { get; set; }
            public string accountNumber { get; set; }
            public string amount { get; set; }
            public string paymentDate { get; set; }
            public string submerchantCode { get; set; }
            public string currency { get; set; }
            public string merchantId { get; set; }
            public string beneficiaryAddress { get; set; }
            public string beneficiaryCity { get; set; }
            public string beneficiaryCountry { get; set; }
            public string beneficiaryEmail { get; set; }
            public string beneficiaryBirthdate { get; set; }
            public string errors { get; set; }
            public string status { get; set; }

        }

        public class ClientBalance
        {
            public string merchant { get; set; }
            public string current_balance { get; set; }
            public string trx_in_progress_amt { get; set; }
            public string trx_in_progress_cnt { get; set; }
            public string balance_after_execution { get; set; }
            public string trx_received_amt { get; set; }
            public string trx_received_cnt { get; set; }
        }
    }
}
