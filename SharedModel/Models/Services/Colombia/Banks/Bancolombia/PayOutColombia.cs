using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SharedModel.ValidationsAttrs;


namespace SharedModel.Models.Services.Colombia.Banks.Bancolombia
{
    public class PayOutColombia
    {
        public class LotBatchColombia
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
        }

        public class Create
        {
            public class Request
            {
                #region Sender Information
                [JsonProperty(Order = 4)]
                [Required(ErrorMessage = "Parameter :: submerchant_code :: is required.")]
                [StringLength(60, MinimumLength = 3, ErrorMessage = "Parameter :: submerchant_code :: has minimun 3 characters and 60 characters maximum.")]
                [RegularExpression("^[a-zA-Z0-9\\s\\-_]{3,60}$", ErrorMessage = "Parameter :: submerchant_code :: invalid format, only allow numbers, letters chars [-_.] and spaces, and length should be between 3 and 60 characters.#INVALID")]
                public string submerchant_code { get; set; }
                [JsonProperty(Order = 5)]
                //  [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,20}$", ErrorMessage = "Parameter :: SenderTaxid :: invalid format, only allow: letters and spaces, and length has between 1 and 20 characters.")]
                public string sender_taxid { get; set; }
                [JsonProperty(Order = 6)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: SenderName :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                public string sender_name { get; set; }
                [JsonProperty(Order = 7)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,300}$", ErrorMessage = "Parameter :: sender_address :: invalid format, only allow: letters and spaces, and length has between 1 and 300 characters.")]
                public string sender_address { get; set; }
                [JsonProperty(Order = 8)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,20}$", ErrorMessage = "Parameter :: SenderState :: invalid format, only allow: letters and spaces, and length has between 1 and 20 characters.")]
                public string sender_state { get; set; }
                [JsonProperty(Order = 9)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,20}$", ErrorMessage = "Parameter :: SenderCountry :: invalid format, only allow: letters and spaces, and length has between 1 and 20 characters.")]
                public string sender_country { get; set; }
                [JsonProperty(Order = 10)]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: sender_birthdate :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string sender_birthdate { get; set; }
                [JsonProperty(Order = 11)]
                [EmailAddress(ErrorMessage = "Parameter :: sender_email :: is not a valid e-mail.#INVALID")]
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
                [StringLength(60, MinimumLength = 1, ErrorMessage = "Parameter :: beneficiary_name :: has minimun 1 character length and 60 characters maximum.#LENGTH")]
                [RegularExpression("^([A-Za-z\\u00C0-\\u00D6\\u00D8-\\u00f6\\u00f8-\\u00ff\\s][^\r\n]{1,60})$", ErrorMessage = "Parameter :: beneficiary_name :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.#INVALID")]
                public string beneficiary_name { get; set; }
                [JsonProperty(Order = 13)]
                [Required(ErrorMessage = "Parameter :: type_of_id :: is required.#REQUIRED")]
                [StringLength(1, MinimumLength = 1, ErrorMessage = "Parameter :: type_of_id :: has 1 character length.#LENGTH")]
                [RegularExpression("^[1-5]{1}$", ErrorMessage = "Parameter :: type_of_id :: invalid format, only characters available: '1' - Cédula de ciudadanía '2' - Cédula de extranjería, '3' - NIT, '4' - Tarjeta de identidad, '5' - Pasaporte.#INVALID")]
                public string type_of_id { get; set; }
                [JsonProperty(Order = 14)]
                [Required(ErrorMessage = "Parameter :: id :: is required.#REQUIRED")]
                [StringLength(15, MinimumLength = 1, ErrorMessage = "Parameter :: id :: has 1 minimum characters length and maximum 15 characters length.#LENGTH")] //ver minimum
                [RegularExpression("^[0-9]{1,15}$", ErrorMessage = "Parameter :: id :: invalid length, must be only numbers and between 1 and 15 characters.#INVALID")]
                public string id { get; set; }
                [JsonProperty(Order = 15)]
                [Required(ErrorMessage = "Parameter :: account_type :: is required.#REQUIRED")]
                [StringLength(2, MinimumLength = 2, ErrorMessage = "Parameter :: account_type :: has 2 characters length.#LENGTH")]
                [RegularExpression("^[27|37]{2}$", ErrorMessage = "Parameter :: account_type :: invalid format, only characters available: '27' – checking account, '37' – savings account.#INVALID")]
                public string account_type { get; set; }
                [JsonProperty(Order = 16)]
                [Required(ErrorMessage = "Parameter :: bank_code :: is required. #REQUIRED")]
                [BankCode(ErrorMessage = "Parameter :: bank_code :: invalid value .#INVALID")]
                public string bank_code { get; set; }
                [JsonProperty(Order = 17)]
                [Required(ErrorMessage = "Parameter :: beneficiary_account_number :: is required.#REQUIRED")]
                [StringLength(20, MinimumLength = 4, ErrorMessage = "Parameter :: beneficiary_account_number :: has minimum 4 characters and maximum 20 characters length.#LENGTH")]
                [RegularExpression("^[0-9]{4,20}$", ErrorMessage = "Parameter :: beneficiary_account_number :: invalid format, only allow: numbers and length has 20 characters.")]
                public string beneficiary_account_number { get; set; }
                [JsonProperty(Order = 18)]
                //[RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,150}$", ErrorMessage = "Parameter :: address :: invalid format, only allow: letters, numbers and spaces, and length has between 1 and 150 characters.#INVALID")]
                [StringLength(300, MinimumLength = 0, ErrorMessage = "Parameter :: beneficiary_address :: has minimun 0 characters and 300 characters maximum.#LENGTH")]
                public string beneficiary_address { get; set; }
                [JsonProperty(Order = 19)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: beneficiary_state :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.#INVALID")]
                public string beneficiary_state { get; set; }
                [JsonProperty(Order = 20)]
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,30}$", ErrorMessage = "Parameter :: beneficiary_country :: invalid format, only allow: letters and spaces, and length has between 1 and 30 characters.#INVALID")]
                public string beneficiary_country { get; set; }
                [JsonProperty(Order = 21)]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: beneficiary_birth_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).#INVALID")]
                public string beneficiary_birth_date { get; set; }
                [JsonProperty(Order = 22)]
                [EmailAddress(ErrorMessage = "Parameter :: beneficiary_email :: is not a valid e-mail.#INVALID")]
                public string beneficiary_email { get; set; }

                [JsonProperty(Order = 22)]
                [Phone(ErrorMessage = "Parameter :: beneficiary_phone_number :: is not a valid phone number.#INVALID")]
                public string beneficiary_phone_number { get; set; }

                [JsonProperty(Order = 20)]
                [RegularExpression("^[A-Za-z0-9-]{1,10}$", ErrorMessage = "Parameter :: beneficiary_zip_code :: invalid value: allow only: letters, numbers and hyphen.#INVALID")]
                public string beneficiary_zip_code { get; set; }
                #endregion

                #region Transaction Detail Information
                [JsonProperty(Order = 23)]
                [Required(ErrorMessage = "Parameter :: merchant_id :: is required.#REQUIRED")]
                [StringLength(60, MinimumLength = 0, ErrorMessage = "Parameter :: merchant_id :: has minimun 0 characters and 60 characters maximum.#LENGTH")]
                [RegularExpression("^[a-zA-Z0-9\\s]{0,60}$", ErrorMessage = "Parameter :: merchant_id :: invalid format, only allow: letters and spaces.#INVALID")]
                public string merchant_id { get; set; }
                [StringLength(4, MinimumLength = 0, ErrorMessage = "Parameter :: concept_code :: has minimum 0 and 4 characters maximum.#LENGTH")]
                [JsonProperty(Order = 24)]
                public string concept_code { get; set; }

                [JsonProperty(Order = 26)]
                [Required(ErrorMessage = "Parameter :: currency :: is required.#REQUIRED")]
                [RegularExpression("^USD$|^COP$", ErrorMessage = "Parameter :: currency :: invalid format, only allow codes: 'USD' | 'COP'.#INVALID")]
                public string currency { get; set; }
                private string _payout_date { get; set; }
                [JsonProperty(Order = 27)]
                [ExpiredDate(ErrorMessage = "Parameter :: _payout_date :: invalid date: it must be greater than actual date.#INVALID")]
                public string payout_date { get { return this._payout_date == null || string.IsNullOrEmpty(this._payout_date) ? DateTime.Now.ToString("yyyyMMdd") : this._payout_date; } set { this._payout_date = value; } }

                [JsonProperty(Order = 28)]
                [Required(ErrorMessage = "Parameter :: amount :: is required.#REQUIRED")]
                [Range(100, 99999999999999, ErrorMessage = "Parameter :: amount :: value out range [100 - 99999999999999].#INVALID")]
                [RegularExpression("^[0-9]{3,14}$", ErrorMessage = "Parameter :: amount :: invalid length, must be between 3 and 14 characters (12 integers + 2 decimals.#LENGTH")]
                public Int64 amount { get; set; }

                #endregion

                public bool authenticate { get; set; }
                //[Required(ErrorMessage = "Parameter :: subclient_code :: is required.")]
                //[StringLength(60, MinimumLength = 3, ErrorMessage = "Parameter :: subclient_code :: has minimun 3 characters and 60 characters maximum.")]




                //private string _beneficiary_softd { get; set; }
                //public string beneficiary_softd { get { return string.IsNullOrEmpty(_beneficiary_softd) ? "" : _beneficiary_softd; } set { _beneficiary_softd = value; } }

                //private int _concept_code { get; set; }
                //public int concept_code { get { return 0; } set { _concept_code = value; } }

                //[RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,60}$", ErrorMessage = "Parameter :: annotation :: invalid format, only allow: letters, numbers and spaces, and length has between 1 and 60 characters.")]
                //public string annotation { get; set; }

                //[RegularExpression("^[0-9]{30}$", ErrorMessage = "Parameter :: telephone_number :: invalid format, only allow: numbers and length has 30 characters.")]
                //public string telephone_number { get; set; }

                //[RegularExpression("^[0-9]{30}$", ErrorMessage = "Parameter :: fax_number :: invalid format, only allow: numbers and length has 30 characters.")]
                //public string fax_number { get; set; }

                //[RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: birth_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                //public string birth_date { get; set; }

                ////[Required(ErrorMessage = "Parameter :: subclient_code :: is required.")]
                //[StringLength(60, MinimumLength = 3, ErrorMessage = "Parameter :: subclient_code :: has minimun 3 characters and 60 characters maximum.")]
                //[RegularExpression("^[a-zA-Z0-9\\s\\-_]{3,60}$", ErrorMessage = "Parameter :: subclient_code :: invalid format, only allow numbers, letters chars [-_.] and spaces, and length should be between 3 and 60 characters.")]
                //public string subclient_code { get; set; }


                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                [JsonProperty(Order = 31)]
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
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
                [JsonProperty(Order = 29)]
                public Int64 exchange_rate { get; set; }
                [JsonProperty(Order = 30)]
                public Int64 gmf_tax { get; set; }

                #endregion
                //public Int32 withholding_tax { get; set; }
            }
        }

        public class DownloadLotBatchTransactionToBank
        {
            public class Request
            {
                public int PaymentType { get; set; }
                public string FileCode { get; set; }
                public string idMerchant { get; set; }
                public string idSubMerchant { get; set; }

                public string dateTo { get; set; }
                public string amount { get; set; }
            }
            public class Response
            {
                public int RowsPayouts { get; set; }
                public string[] LinesPayouts { get; set; }

                public int RowsBeneficiaries { get; set; }
                public string[] LinesBeneficiaries { get; set; }

                public string Status { get; set; }
                public string StatusMessage { get; set; }
                public string FileBase64_Payouts { get; set; }
                public string FileBase64_Beneficiaries { get; set; }
                public int idLotOut { get; set; }
                public string TotalAmount { get; set; }
            }
    }


        public class UploadTxtFromBank
        {
            public class Request
            {
                public string File { get; set; }
                public Int64 CurrencyFxClose { get; set; }
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

        public class Update
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter :: transaction_id :: is required.")]
                [RegularExpression("^[0-9]*$", ErrorMessage = "Parameter :: transaction_id :: invalid format, only allow: numbers.")]
                public Int64 transaction_id { get; set; }

                [Required(ErrorMessage = "Parameter :: payout_id :: is required.")]
                [RegularExpression("^[0-9]*$", ErrorMessage = "Parameter :: payout_id :: invalid format, only allow: numbers.")]
                public Int64 payout_id { get; set; }

                [Required(ErrorMessage = "Parameter :: type_of_id :: is required.")]
                [StringLength(1, MinimumLength = 1, ErrorMessage = "Parameter :: type_of_id :: has 1 character length.")]
                [RegularExpression("^[1-5]{1}$", ErrorMessage = "Parameter :: type_of_id :: invalid format, only characters available: '1' - Cédula de ciudadanía '2' - Cédula de extranjería, '3' - NIT, '4' - Tarjeta de identidad, '5' - Pasaporte.")]
                public string type_of_id { get; set; }

                [Required(ErrorMessage = "Parameter :: id :: is required.")]
                [StringLength(15, MinimumLength = 1, ErrorMessage = "Parameter :: id :: has 1 minimum characters length and maximum 15 characters length.")] //ver minimum
                [RegularExpression("^[0-9]{1,15}$", ErrorMessage = "Parameter :: id :: invalid length, must be only numbers and between 1 and 15 characters.")]
                public string id { get; set; }

                [Required(ErrorMessage = "Parameter :: beneficiary_name :: is required.")]
                [StringLength(60, MinimumLength = 1, ErrorMessage = "Parameter :: beneficiary_name :: has minimun 1 characters and 60 characters maximum. ")]
                [RegularExpression("^[a-zA-Z0-9\\s]{1,60}$", ErrorMessage = "Parameter :: beneficiary_name :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                public string beneficiary_name { get; set; }

                [Required(ErrorMessage = "Parameter :: account_type :: is required.")]
                [StringLength(2, MinimumLength = 2, ErrorMessage = "Parameter :: account_type :: has 2 characters length.")]
                [RegularExpression("^[27|37]{2}$", ErrorMessage = "Parameter :: account_type :: invalid format, only characters available: '27' – checking account, '37' – savings account.")]
                public string account_type { get; set; }

                [Required(ErrorMessage = "Parameter :: bank_code :: is required.")]
                ///*ver*/[RegularExpression("^[1001-1507]{4}$", ErrorMessage = "Parameter :: bank_code :: invalid format, only allow Colombia bank codes.")]
                public string bank_code { get; set; }
                
                [Required(ErrorMessage = "Parameter :: beneficiary_account_number :: is required.")]
                [StringLength(20, MinimumLength = 4, ErrorMessage = "Parameter :: beneficiary_account_number :: has minimum 4 characters and maximum 20 characters length.")]
                //[RegularExpression("^[0-9]$", ErrorMessage = "Parameter :: beneficiary_account_number :: invalid format, only allow: numbers and length has 20 characters.")]
                public string beneficiary_account_number { get; set; }

                [Required(ErrorMessage = "Parameter :: amount :: is required.")]
                [Range(100, 100000000, ErrorMessage = "Parameter :: amount :: value out range [100 - 100000000].")]
                [RegularExpression("^[0-9]{3,9}$", ErrorMessage = "Parameter :: amount :: invalid length, must be between 3 and 9 characters.")]
                public Int64 amount { get; set; }

                [Required(ErrorMessage = "Parameter :: beneficiary_softd :: is required.")]
                [StringLength(15, MinimumLength = 0, ErrorMessage = "Parameter :: beneficiary_softd :: has minimun 0 characters and 15 characters maximum.")]
                [RegularExpression("^[a-zA-Z\\s]{0,15}$", ErrorMessage = "Parameter :: beneficiary_softd :: invalid format, only allow: letters and spaces.")]
                private string _beneficiary_softd { get; set; }
                public string beneficiary_softd { get { return string.IsNullOrEmpty(_beneficiary_softd) ? "" : _beneficiary_softd; } set { _beneficiary_softd = value; } }

                [Required(ErrorMessage = "Parameter :: site_transaction_id :: is required.")]
                [StringLength(22, MinimumLength = 0, ErrorMessage = "Parameter :: site_transaction_id :: has minimun 0 characters and 22 characters maximum.")]
                [RegularExpression("^[a-zA-Z0-9\\s]{0,22}$", ErrorMessage = "Parameter :: site_transaction_id :: invalid format, only allow: letters and spaces.")]
                public string site_transaction_id { get; set; }

                [StringLength(4, MinimumLength = 0, ErrorMessage = "Parameter :: concept_code :: has minimum 0 and 4 characters maximum.#LENGTH")]
                public string concept_code { get; set; }

                [Required(ErrorMessage = "Parameter :: currency :: is required.")]
                [RegularExpression("^[ARS|USD|BRL|EUR|UYU|COP|PYG]{3}$", ErrorMessage = "Parameter :: currency :: invalid format, only allow codes: 'ARS' | 'USD' | 'BRL' | 'EUR' | 'UYU' | 'COP' | 'PYG'.")]
                public string currency { get; set; }

                [Required(ErrorMessage = "Parameter :: payout_date :: is required.")]
                [StringLength(8, MinimumLength = 8, ErrorMessage = "Parameter :: payout_date :: has 8 characters length.")]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: payout_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                private string _payout_date { get; set; }
                public string payout_date { get { return this._payout_date == null || string.IsNullOrEmpty(this._payout_date) ? DateTime.Now.ToString("yyyyMMdd") : this._payout_date; } set { this._payout_date = value; } }

                [EmailAddress(ErrorMessage = "Parameter :: email :: is not a valid mail.")]
                public string email { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,150}$", ErrorMessage = "Parameter :: address :: invalid format, only allow: letters, numbers and spaces, and length has between 1 and 150 characters.")]
                public string address { get; set; }

                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: birth_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string birth_date { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: country :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                public string country { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: city :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                public string city { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,60}$", ErrorMessage = "Parameter :: annotation :: invalid format, only allow: letters, numbers and spaces, and length has between 1 and 60 characters.")]
                public string annotation { get; set; }

                [Required(ErrorMessage = "Parameter :: subclient_code :: is required.")]
                [StringLength(60, MinimumLength = 3, ErrorMessage = "Parameter :: subclient_code :: has minimun 3 characters and 60 characters maximum.")]
                [RegularExpression("^[a-zA-Z0-9\\s\\.-_]{3,60}$", ErrorMessage = "Parameter :: subclient_code :: invalid format, only allow numbers, letters chars [-_.] and spaces, and length should be between 3 and 60 characters.")]
                public string subclient_code { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: SenderName :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                public string sender_name { get; set; }
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,150}$", ErrorMessage = "Parameter :: sender_address :: invalid format, only allow: letters and spaces, and length has between 1 and 150 characters.")]
                public string sender_address { get; set; }
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,20}$", ErrorMessage = "Parameter :: SenderState :: invalid format, only allow: letters and spaces, and length has between 1 and 20 characters.")]
                public string sender_state { get; set; }
                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,20}$", ErrorMessage = "Parameter :: SenderCountry :: invalid format, only allow: letters and spaces, and length has between 1 and 20 characters.")]
                public string sender_country { get; set; }
               public string sender_taxid { get; set; }
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: sender_birthdate :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string sender_birthdate { get; set; }
                [EmailAddress(ErrorMessage = "Parameter :: sender_email :: is not a valid e-mail.#INVALID")]
                public string sender_email { get; set; }

                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }
            public class Response : Request
            {
                public string status { get; set; }
                public Int32 withholding_tax { get; set; }
            }
        }


        public class List
        {
            public class Request
            {
                [RegularExpression("^[0-9]*$", ErrorMessage = "Parameter :: transaction_id :: invalid format, only allow: numbers.")]
                public Int64 transaction_id { get; set; }
                [RegularExpression("^[0-9]*$", ErrorMessage = "Parameter :: payout_id :: invalid format, only allow: numbers.")]
                public Int64 payout_id { get; set; }
                [StringLength(8, MinimumLength = 8, ErrorMessage = "Parameter :: date_from :: has 8 characters length.")]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: payout_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string date_from { get; set; }
                [StringLength(8, MinimumLength = 8, ErrorMessage = "Parameter :: date_to :: has 8 characters length.")]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: payout_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string date_to { get; set; }
                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }

            public class Response
            {
                public Int64 transaction_id { get; set; }
                public string type_of_id { get; set; }
                public string id { get; set; }
                public string beneficiary_name { get; set; }
                public string bank_account_type { get; set; }
                public string bank_account { get; set; }
                public string bank_code { get; set; }
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
                public string gmf_tax { get; set; }
            }
            public class LotBatch
            {
                public Int64 payout_id { get; set; }
                public string customer_name { get; set; }
                public string transaction_type { get; set; }
                public string status { get; set; }
                public string lot_date { get; set; }
                public Int64 gross_amount { get; set; }
                public Int64 net_amount { get; set; }

                private List<List.Response> pri_transaction_list = new List<List.Response>();
                public List<List.Response> transaction_list { get { return pri_transaction_list; } set { pri_transaction_list = value; } }
            }

        }


    }
}
