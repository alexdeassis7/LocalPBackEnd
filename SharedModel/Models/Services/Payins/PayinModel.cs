using Newtonsoft.Json;
using SharedModel.ValidationsAttrs.Payin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Services.Payins
{
    public class PayinModel
    {
        public class Create
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter :: amount :: is required.#REQUIRED")]
                [Range(100, 100000000, ErrorMessage = "Parameter :: amount :: value out range [100 - 100000000].#INVALID")]
                [RegularExpression("^[0-9]{3,9}$", ErrorMessage = "Parameter :: amount :: invalid length, must be between 3 and 9 characters.#LENGTH")]
                public long amount { get; set; }

                [Required(ErrorMessage = "Parameter :: currency :: is required.#REQUIRED")]
                [RegularExpression("^ARS$|^USD$|^BRL$|^UYU$|^COP$|^MXN$|^CLP$", ErrorMessage = "Parameter :: currency :: invalid format, only allow codes: 'ARS' | 'MXN' | 'BRL' | 'CLP' | 'UYU' | 'COP'.#INVALID")]
                public string currency { get; set; }

                [Required(ErrorMessage = "Parameter :: payment_method_code :: is required.#REQUIRED")]
                [PaymentMethodCodeAttribute(ErrorMessage = "Parameter :: payment_method_code :: is invalid.#REQUIRED")]
                public string payment_method_code { get; set; }

                [Required(ErrorMessage = "Parameter :: merchant_id :: is required.#REQUIRED")]
                [StringLength(60, MinimumLength = 0, ErrorMessage = "Parameter :: merchant_id :: has minimun 0 characters and 60 characters maximum.#LENGTH")]
                [RegularExpression("^[a-zA-Z0-9\\s]{0,60}$", ErrorMessage = "Parameter :: merchant_id :: invalid format, only allow: letters and spaces.#INVALID")]
                public string merchant_id { get; set; }

                [Required(ErrorMessage = "Parameter :: payer_name :: is required.#REQUIRED")]
                [StringLength(60, MinimumLength = 1, ErrorMessage = "Parameter :: payer_name :: has minimun 1 characters and 60 characters maximum.#LENGTH")]
                [RegularExpression("^([A-Za-z\\u00C0-\\u00D6\\u00D8-\\u00f6\\u00f8-\\u00ff\\s]{1,60})$", ErrorMessage = "Parameter :: merchant_id :: invalid format, only allow: latin letters and spaces.#INVALID")]
                public string payer_name { get; set; }

                [StringLength(15, MinimumLength = 0, ErrorMessage = "Parameter :: payer_document_number :: has minimun 0 characters and 15 characters maximum.#LENGTH")]
                [DocumentNumber]
                public string payer_document_number { get; set; } = "";

                [Required(ErrorMessage = "Parameter :: payer_account_number :: is required.#REQUIRED")]
                [StringLength(22, MinimumLength = 4, ErrorMessage = "Parameter :: payer_account_number :: has minimun 4 characters and 22 characters maximum.#LENGTH")]
                public string payer_account_number { get; set; }

                [EmailAddress(ErrorMessage = "Parameter :: payer_email :: is not a valid e-mail.#INVALID")]
                public string payer_email { get; set; }

                [Phone(ErrorMessage = "Parameter :: payer_phone_number :: is not a valid phone number.#INVALID")]
                public string payer_phone_number { get; set; }

                [Required(ErrorMessage = "Parameter :: submerchant_code :: is required.#REQUIRED")]
                [StringLength(60, MinimumLength = 3, ErrorMessage = "Parameter :: submerchant_code :: has minimun 3 characters and 60 characters maximum.#LENGTH")]
                [RegularExpression("^[a-zA-Z0-9\\s\\-_]{3,60}$", ErrorMessage = "Parameter :: submerchant_code :: invalid format, only allow numbers, letters chars [-_.] and spaces, and length should be between 3 and 60 characters.#INVALID")]
                public string submerchant_code { get; set; }

                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                [JsonProperty(Order = 31)]
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }

            public class Response : Request
            {
                public long payin_id { get; set; }
                public string transaction_date { get; set; }
                public string status { get; set; }
                public string status_detail { get; set; }

                public string reference_code { get; set; }
            }

        }

        public class List
        {
            public class Request
            {
                public string merchant_id { get; set; }
                public long payin_id { get; set; }
                public string date_from { get; set; }
                public string date_to { get; set; }
            }

            public class Response
            {
                public long payin_id { get; set; }
                public string transaction_date { get; set; }
                public string status { get; set; }
                public string status_detail { get; set; }
                public long amount { get; set; }
                public string currency { get; set; }
                public string payment_method_code { get; set; }
                public string merchant_id { get; set; }
                public string payer_name { get; set; }
                public string payer_document_number { get; set; }
                public string payer_account_number { get; set; }
                public string payer_phone_number { get; set; }
                public string payer_email { get; set; }
                public string submerchant_code { get; set; }
                public string reference_code { get; set; }
            }
        }

        public class Manage 
        {
            public class Request 
            { 
                public string provider { get; set; }

                public string merchant { get; set; }

                public string submerchant { get; set; }
            }

            public class Response : Create.Response
            { 
                public string merchant_name { get; set; }

                public string ticket { get; set; }
            }
        
        }

        public class RejectedPayins 
        {
            public class TransactionError : Create.Request
            {
                public string errors { get; set; }
            }
        }
        
    }
}
