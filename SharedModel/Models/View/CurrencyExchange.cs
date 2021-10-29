using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace SharedModel.Models.View
{
    public class withSource
    {

    }
    public class CurrencyExchange
    {
        public class Single
        {
            public class Request
            {
                [Required(ErrorMessage = "Parameter base_currency is required.#REQUIRED")]
                [RegularExpression("^USD$|^usd$", ErrorMessage = "Parameter base_currency allow only USD or usd value.#INVALID")]
                public string base_currency { get; set; }
                [Required(ErrorMessage = "Parameter quote_currency is required.#REQUIRED")]
                public string quote_currency { get; set; }
                [Required(ErrorMessage = "Parameter transaction_type is required.#REQUIRED")]
                [RegularExpression("^P[I|O]$|^RF$", ErrorMessage = "Parameter transaction_type allow only this values [ PI | PO | RF ].#INVALID")]
                public string transaction_type { get; set; }
                public string with_source { get; set; }
                public string date { get; set; }
                public General.ErrorModel.Error ErrorRow { get; set; } = new General.ErrorModel.Error();
            }

            public class Response : Request
            {
                public string quote_date { get; set; }
                public long quote { get; set; }
                public int source_quote { get; set; }
                public string valid_from_datetime { get; set; }
                public string expiration_datetime { get; set; }
            }
        }

        public class List
        {
            public class Request
            {
                private SharedModel.Models.General.ErrorModel.Error _errorrow = new General.ErrorModel.Error();

                public string base_currency { get; set; }
                public string entity_user { get; set; }

                public static explicit operator Request(Dictionary<string, string> data) => new Request
                {
                    base_currency = data.ContainsKey("base_currency") ? data["base_currency"] : null,
                    entity_user = data.ContainsKey("entity_user") ? data["entity_user"] : null
                };

                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return _errorrow; } set { _errorrow = value; } }
            }

            public class Response : Request
            {
                public Int64 Spot { get; set; }
                public Int64 SprissVenta { get; set; }
                public Int64 SprissCompra { get; set; }
                public Int64 ValorVenta { get; set; }
                public Int64 ValorCompra { get; set; }
                public DateTime Fecha { get; set; }
                public string EntityUser { get; set; }
                public string Merchant { get; set; }
                public string TransactionType { get; set; }
                public string Status { get; set; }
                public List<General.ErrorModel.ValidationErrorGroup> StatusMessage { get; set; }

            }
        }
    }
}
