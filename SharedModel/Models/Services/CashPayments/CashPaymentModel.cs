using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharedModel.Models.Services.CashPayments
{
    public class CashPaymentModel
    {
        public class Create
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter :: invoice :: is required.")]
                [RegularExpression("^[a-zA-Z0-9.-_\\s]{12,12}$", ErrorMessage = "Parameter :: invoice :: invalid format: allow only numbers, length 12 digits.")]
                public string invoice { get; set; }
                [Required(ErrorMessage = "Parameter :: payment_method :: is required.")]
                [RegularExpression("^(RAPA)$|^(PAFA)$|^(BAPR)|^(COEX)$", ErrorMessage = "Parameter :: payment_method :: invalid value: [RAPA: RapiPago | PAFA: PagoFacil | BAPR: Bapro | COEX: CobroExpress].")]
                public string payment_method { get; set; }
                [Required(ErrorMessage = "Parameter :: additional_info :: is required.")]
                [RegularExpression("^[a-zA-Z0-9.-_\\s]{1,30}$", ErrorMessage = "Parameter :: additional_info :: invalid value: alphanumeric [a-zA-Z0-9.-_ ], lenggth between 1 - 30 characters.")]
                public string additional_info { get; set; }
                [Required(ErrorMessage = "Parameter :: first_expiration_amount :: is required.")]
                //[Range(00000100, 99999999, ErrorMessage = "Parameter :: first_expiration_amount :: invalid value: allow only numbers, length 8 characters (6e + 2d) and range between 00000100 and 99999999.")]
                [StringLength(8, MinimumLength = 8, ErrorMessage = "Parameter :: first_expiration_amount :: invalid value: allow only numbers, length 8 characters (6e + 2d) and range between 00000100 and 99999999.")]
                public string first_expiration_amount { get; set; }
                [Required(ErrorMessage = "Parameter :: first_expirtation_date :: is required.")]
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: first_expirtation_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string first_expirtation_date { get; set; }
                [Required(ErrorMessage = "Parameter :: currency :: is required.")]
                //[RegularExpression("^(ARS)$", ErrorMessage = "Parameter :: currency :: invalid value: value must be: ARS.")]
                public string currency { get; set; }
                [Required(ErrorMessage = "Parameter :: surcharge :: is required.")]
                [Range(0000000, 99999999, ErrorMessage = "Parameter :: surcharge :: invalid value: allow only numbers, length 8 characters (6e + 2d) and range between 00000100 and 99999999, if payment_method in [RAPA | PAFA | COEX] the length must be: 6 digits (4e + 2d).")]
                public string surcharge { get; set; }
                [Required(ErrorMessage = "Parameter :: days_to_second_exp_date :: is required.")]
                [RegularExpression("^[0-3]{1}$", ErrorMessage = "Parameter :: currency :: invalid value: value must be: 1 digit between 1 and 3.")]
                public string days_to_second_exp_date { get; set; }
                [Required(ErrorMessage = "Parameter :: identification :: is required.")]
                [RegularExpression("^(\\b(20|23|24|27|30|33|34)(\\D)?[0-9]{8}(\\D)?[0-9])$", ErrorMessage = "Parameter :: identification :: invalid value.")]
                public string identification { get; set; }

                [Required(ErrorMessage = "Parameter :: name :: is required.")]
                [StringLength(16, MinimumLength = 1, ErrorMessage = "Parameter :: name :: has minimun 1 characters and 16 characters maximum. ")]
                [RegularExpression("^[a-zA-Z\\s]{1,16}$", ErrorMessage = "Parameter :: name :: invalid format, only allow: letters and spaces, and length has between 1 and 16 characters.")]
                public string name { get; set; }

                [Required(ErrorMessage = "Parameter :: mail :: is required.")]
                public string mail { get; set; }

                //[Required(ErrorMessage = "Parameter :: first_name :: is required.")]
                //[RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: first_name :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                //public string first_name { get; set; }

                //[Required(ErrorMessage = "Parameter :: last_name :: is required.")]
                //[RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: last_name :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                //public string last_name { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,60}$", ErrorMessage = "Parameter :: address :: invalid format, only allow: letters, numbers and spaces, and length has between 1 and 60 characters.")]
                public string address { get; set; }
                [RegularExpression("([12]\\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\\d|3[01]))", ErrorMessage = "Parameter :: birth_date :: invalid value: allow only date format, length 8 characters (YYYMMDD).")]
                public string birth_date { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: country :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                public string country { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: city :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
                public string city { get; set; }

                [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,60}$", ErrorMessage = "Parameter :: annotation :: invalid format, only allow: letters, numbers and spaces, and length has between 1 and 60 characters.")]
                public string annotation { get; set; }

                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }

            public class Response : Request
            {
                public string bar_code { get; set; }
                public string transaction_id {  get; set; }
                public string bar_code_number { get; set; }

                public string bar_code_url { get; set; }
            }
        }

        public class Upload
        {
            public string TransactionType { get; set; }
            public string File { get; set; }
            public string FileName { get; set; }
        }

        public class ReadFile
        {
            public string Status { get; set; } /* OK | ERROR */
            public string StatusMessage { get; set; }
            public int QtyTransactionRead { get; set; }
            public int QtyTransactionProcess { get; set; }
            public int QtyTransactionDismiss { get; set; }
            private List<TransactionResult> _TrProcessDetail = new List<TransactionResult>();
            public List<TransactionResult> TrProcessDetail { get { return _TrProcessDetail; } set { _TrProcessDetail = value; } }
            private List<TransactionResult> _TrDismissDetail = new List<TransactionResult>();
            public List<TransactionResult> TrDismissDetail { get { return _TrDismissDetail; } set { _TrDismissDetail = value; } }
        }

        public class TransactionResult
        {
            public string Ticket { get; set; }
            public string BarCodeNumber { get; set; }
            public double Amount { get; set; }
        }
    }
}
