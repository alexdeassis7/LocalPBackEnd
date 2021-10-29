using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static SharedModel.Models.General.ErrorModel;

namespace SharedModel.Models.Services.Decidir
{
    public class SimpleTransactionModel
    {
        [Required(ErrorMessage = "Parameter :: identification :: is required.")]
        [RegularExpression("^(\\b(20|23|24|27|30|33|34)(\\D)?[0-9]{8}(\\D)?[0-9])$", ErrorMessage = "Parameter :: identification :: invalid value.")]
        public string Identification { get; set; }
        public PayToken.Request cardData { get; set; }
        public PayToken.RequestWithTokenizedCard tokenizedCardData { get; set; }


        public PayExecution.Request paymentData { get; set; }

        private Error errorrow = new General.ErrorModel.Error();
        public Error ErrorRow { get { return errorrow; } set { errorrow = value; } }


    }



    public class Customer
    {
        public string name { get; set; }

        public Identification identification { get; set; }

        public string customer_id { get; set; }

        [Required(ErrorMessage = "Final client id is required.")]
        public string finalclient_id { get; set; }

        [EmailAddress(ErrorMessage = "Parameter :: email :: is not a valid e-mail.")]
        [Required(ErrorMessage = "Email is required.")]
        public string email { get; set; }

        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ0-9\\s]{1,150}$", ErrorMessage = "Parameter :: address :: invalid format, only allow: letters, numbers and spaces, and length has between 1 and 150 characters.")]
        public string address { get; set; }

        [StringLength(8, ErrorMessage = "Customer birth_day has 8 characters.", MinimumLength = 8)]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "Parameter :: birth_day :: invalid format, only allow: numbers and length has 8 characters Format: DDMMAAAA .")]
        public string birth_day { get; set; }

        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: city :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
        public string city { get; set; }

        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: country :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
        public string country { get; set; }


    }
    public class PayToken
    {

        public class Request
        {
            public string token { get; set; }

            [Required(ErrorMessage = "Card number is required.")]
            [StringLength(18, ErrorMessage = "Card number has minimun 16 characters and 18 characters maximum.", MinimumLength = 16)]
            [RegularExpression("^[0-9]{16,18}$", ErrorMessage = "Parameter :: card_number :: invalid format, only allow: numbers and length has 18 characters.")]
            public string card_number { get; set; }

            [Required(ErrorMessage = "Card expiration month is required.")]
            [StringLength(2, ErrorMessage = "Card expiration month has 2 characters.", MinimumLength = 2)]
            [RegularExpression("^[0-9]{2}$", ErrorMessage = "Parameter :: card_expiration_month :: invalid format, only allow: numbers and length has 2 characters.")]
            public string card_expiration_month { get; set; }

            [Required(ErrorMessage = "Card expiration year is required.")]
            [StringLength(2, ErrorMessage = "Card expiration year has 2 characters.", MinimumLength = 2)]
            [RegularExpression("^[0-9]{2}$", ErrorMessage = "Parameter :: card_expiration_year :: invalid format, only allow: numbers and length has 2 characters.")]
            public string card_expiration_year { get; set; }

            public string security_code { get; set; }

            [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ\\s]{1,60}$", ErrorMessage = "Parameter :: card_holder_name :: invalid format, only allow: letters and spaces, and length has between 1 and 60 characters.")]
            public string card_holder_name { get; set; }

            [StringLength(8, ErrorMessage = "Card holder birthday has 8 characters.", MinimumLength = 8)]
            [RegularExpression("^[0-9]{8}$", ErrorMessage = "Parameter :: card_holder_birthday :: invalid format, only allow: numbers and length has 8 characters Format: DDMMAAAA .")]
            public string card_holder_birthday { get; set; }

            public Int32 card_holder_door_number { get; set; }

            public PayTokenIdentification card_holder_identification { get; set; }

            public string device_unique_identifier { get; set; }


            public Customer customer { get; set; }

            private Error errorrow = new General.ErrorModel.Error();

            public Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
        }

        public class RequestWithTokenizedCard
        {
            [Required(ErrorMessage = "Tokenized card number is required.")]
            public string token_card { get; set; }

            [Required(ErrorMessage = "Security code is required.")]
            public string security_code { get; set; }

            private Error errorrow = new General.ErrorModel.Error();
            public Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
        }

        public class PayTokenIdentification
        {
            [Required(ErrorMessage = "Identification type is required.")]
            [MinLength(3, ErrorMessage = "Identification type has minimum 3 characters required")]
            public string type { get; set; }

            [Required(ErrorMessage = "Identification number is required.")]
            [MinLength(7, ErrorMessage = "Identification number has minimum 7 characters required")]
            public string number { get; set; }
        }

        public class Response
        {
            public string id { get; set; }
            public string status { get; set; }
            public int card_number_length { get; set; }
            public string date_created { get; set; }
            public int bin { get; set; }
            public int last_four_digits { get; set; }
            public int security_code_length { get; set; }
            public int expiration_month { get; set; }
            public int expiration_year { get; set; }
            public string date_due { get; set; }
            public CardHolder cardholder { get; set; }
        }

    }
    public class CardHolder
    {
        public string name { get; set; }
        public Identification identification { get; set; }
    }

    public class Identification
    {
        [Required(ErrorMessage = "Type is required.")]
        public string type { get; set; }

        [StringLength(10, ErrorMessage = "Number has minimum 1 character and 10 characters maximum.", MinimumLength = 7)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Parameter :: number :: invalid format, only allow: numbers and length has 10 characters maximum.")]
        [Required(ErrorMessage = "Number is required.")]
        public int number { get; set; }
    }

    public class PayExecution
    {
        public class Request
        {
            public string id { get; set; }

            [Required(ErrorMessage = "Site Transaction ID is required.")]
            [StringLength(40, ErrorMessage = "Site Transaction ID has minimun 1 characters and 40 characters maximum.", MinimumLength = 1)]
            public string site_transaction_id { get; set; }

            public Customer customer { get; set; }

            //public string site_id { get; set; }
            //[Required(ErrorMessage = "Token is required.")]
            //[StringLength(36, ErrorMessage = "Token has minimun 1 characters and 36 characters maximum.", MinimumLength = 1)]
            public string token { get; set; }

            [Required(ErrorMessage = "Payment Method ID is required.")]
            [RegularExpression("^[0-9]{1}$", ErrorMessage = "Parameter :: payment_method_id :: invalid format, only allow: one number maximum.")]
            public int payment_method_id { get; set; }

            [Required(ErrorMessage = "Bin is required.")]
            [StringLength(6, ErrorMessage = "Bin has 6 characters.", MinimumLength = 6)]
            public string bin { get; set; }

            [Required(ErrorMessage = "Amount is required.")]
            [Range(0.1, 9223372036854775807.0, ErrorMessage = "Amount has minimun value $ 0.01 (value = 001) and maximum value $ 9223372036854775807.0 (value = 922337203685477580700).")]
            public Int32 amount { get; set; }

            [Required(ErrorMessage = "Currency is required.")]
            public string currency { get; set; }

            [Required(ErrorMessage = "Installments is required.")]
            [RegularExpression("^[1-99]$", ErrorMessage = "Installments has value between 1 and 99.")]
            public int installments { get; set; }

            [Required(ErrorMessage = "Payment Type is required.")]
            [RegularExpression("^[a-zA-Z0-9\\s]{1,10}$", ErrorMessage = "Parameter :: payment_type :: invalid format, only allow: letters, numbers, and spaces, and length has between 1 and 10 characters.")]
            public string payment_type { get; set; }

            [StringLength(25, ErrorMessage = "Establishment Name has minimun 1 characters and 25 characters maximum.", MinimumLength = 1)]
            public string establishment_name { get; set; }

            [Required(ErrorMessage = "Description is required.")]
            public string description { get; set; }

            public List<object> sub_payments { get; set; }

            public AggregateData aggregate_data { get; set; }

            private Error errorrow = new General.ErrorModel.Error();
            public Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
        }

        //public class Payments {
        //    public string site_id { get; set; }
        //    [Required(ErrorMessage = "Amount is required.")]
        //    [Range(0.1, 9223372036854775807.0, ErrorMessage = "Amount has minimun value $ 0.01 and maximum value $ 9223372036854775807.0.")]
        //    public double amount { get; set; }
        //    [Required(ErrorMessage = "Installments is required.")]
        //    //[RegularExpression("^[1-99]{2}$", ErrorMessage = "Installments has value between 1 and 99.")]
        //    public int installments { get; set; }
        //}
        public class Response
        {
            public string Campo { get; set; }
            public string token_card { get; set; }
            public string id { get; set; } //payment_id 
            public string site_transaction_id { get; set; }
            public string payment_method_id { get; set; }
            public string payment_method { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
            public string status { get; set; }
            public string ticket { get; set; }
            public string bin { get; set; }
            public string installments { get; set; }
            public string fraud_detection { get; set; }
            public string aggregate_data { get; set; }
            public string establishment_name { get; set; }
            public string spv { get; set; }
            public string confirmed { get; set; }
            public string pan { get; set; }
            public string customer_token { get; set; }
            public StatusDetails status_details { get; set; }
            private Error errorrow = new Error();
            public Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
        }
        public class StatusDetails
        {
            public string ticket { get; set; }
            public string card_authorization_code { get; set; }
            public string address_validation_code { get; set; }
            public StatusDetailsErrors error { get; set; }
        }

        public class StatusDetailsErrors
        {
            public string type { get; set; }
            public StatusDetailsErrorsReason reason { get; set; }

        }

        public class StatusDetailsErrorsReason
            {
                public string id { get; set; }
                public string description { get; set; }
            }

        }


    public class AggregateData
    {
        public string indicator { get; set; }
        public string identification_number { get; set; }
        public string bill_to_pay { get; set; }
        public string bill_to_refund { get; set; }
        public string merchant_name { get; set; }
        public string street { get; set; }
        public string number { get; set; }
        public string postal_code { get; set; }
        public string category { get; set; }
        public string channel { get; set; }
        public string geographic_code { get; set; }
        public string city { get; set; }
        public string merchant_id { get; set;}
        public string province { get; set; }
        public string country { get; set; }
        public string merchant_email { get; set; }
        public string merchant_phone { get; set; }

        private Error errorrow = new Error();
        public Error ErrorRow { get { return errorrow; } set { errorrow = value; } }

    }

}