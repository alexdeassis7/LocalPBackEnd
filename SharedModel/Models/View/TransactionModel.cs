using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharedModel.Models.View
{
    public class TransactionModel
    {
        public class Transaction
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter action_type is required.")]
                public int idTransactionType;

                [Required(ErrorMessage = "Parameter action_type is required.")]
                public String TransactionTypeCode;

                [Required(ErrorMessage = "Parameter amount is required.")]
                public decimal Amount;

                [Required(ErrorMessage = "Parameter client is required.")]
                public int idEntityUser;

                [Required(ErrorMessage = "Parameter currency is required.")]
                public int idCurrencyType;

                [Required(ErrorMessage = "Parameter description is required.")]
                public string Description;

                public int ValueFX;

                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();

                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }

            public class Response : Request
            {
                public string status { get; set; }

                public string status_message { get; set; }
            }
        }

        public class Close
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter :: value :: is required.")]
                [RegularExpression("(^[0-9]{1,4})+\\.+([0-9]{0,6})$", ErrorMessage = "Parameter :: value :: invalid value.")]
                public string value { get; set; }

                [Required(ErrorMessage = "Parameter :: ListID :: is required.")]
                public long[] transactions { get; set; }
                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }

            public class Response : Request
            {
                public string status { get; set; }
                public string status_message { get; set; }
            }
        }

        public class TransactionLot 
        {
            public class Response
            {
                public string User { get; set; }
                public string Merchant { get; set; }
                public int TotalTransactions { get; set; }
                public decimal TotalAmount { get; set; }
                public string CurrencyType { get; set; }
                public DateTime LotDate { get; set; }
                public string Country { get; set; }
            }

        }
    }
}
