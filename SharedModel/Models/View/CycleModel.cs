using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using SharedModel.ValidationsAttrs;

namespace SharedModel.Models.View
{
    public class CycleModel
    {
        public class Create
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter :: idEntity :: is required.")]
                [RegularExpression("[0-9]", ErrorMessage = "Parameter :: idEntity :: Allow only numbers.")]
                public Int64 idEntity { get; set; }

                [Required(ErrorMessage = "Parameter :: Type :: is required.")]
                [RegularExpression("^(PROV)$|^(ENTI)$|^(PWS)$", ErrorMessage = "Parameter :: Type :: Allow only three values [ PROV | ENTI | PWS ].")]
                public string Type { get; set; }

                [Required(ErrorMessage = "Parameter :: idTransactionType :: is required.")]
                [RegularExpression("[0-9]", ErrorMessage = "Parameter :: idTransactionType :: Allow only numbers.")]
                public int idTransactionType { get; set; }

                [Required(ErrorMessage = "Parameter :: isMonthly :: is required.")]
                [RegularExpression("^[0-1]{1}$", ErrorMessage = "Parameter :: idMonthly :: Allow only 1 for true or 0 for false.")]
                public int isMonthly { get; set; }

                [RequiredIfBoolean("isMonthly", false, ErrorMessage = "Parameter :: TransactionFromDay :: is required.")]
                public int TransactionFromDay { get; set; }

                [RequiredIfBoolean("isMonthly", false, ErrorMessage = "Parameter :: TransactionToDay :: is required.")]
                public int TransactionToDay { get; set; }

                [RequiredIfBoolean("isMonthly", false, ErrorMessage = "Parameter :: AddDaysToPay :: is required.")]
                public int AddDaysToPay { get; set; }

                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }

            public class Response : Request
            {
                public string Status { get; set; }
                public string StatusMessage { get; set; }
            }
        }
    }
}
