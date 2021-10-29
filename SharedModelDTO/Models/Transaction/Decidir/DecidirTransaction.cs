using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction.Decidir
{
    public class DecidirTransaction
    {
        public object statusDetail { get; set; }
        public string paymentId { get; set; }
        public string customerID { get; set; }
        public int amount { get; set; }
        public string transactionType { get; set; }
        public string currency { get; set; }
        public string detail { get; set; }
        public string ticket { get; set; }
        public string comprobanteTransaction { get; set; }
        public string clientIdentification { get; set; }
        public string installments { get; set; }
        public string internalStatus { get; set; }
        public AggregateTransaction aggregateTx { get; set; }
    }

    public class  AggregateTransaction
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
        public string merchant_id { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string merchant_email { get; set; }
        public string merchant_phone { get; set; }
    }   

    public class TokenizedCards
    {
        public List<TokenizedCard> tokens { get; set; }
    }

    public class TokenizedCard
    {
        public string token { get; set; }
        public int payment_method_id { get; set; }
        public string bin { get; set; }
        public string last_four_digits { get; set; }
        public string expiration_month { get; set; }
        public string expiration_year { get; set; }
        public CardHolder card_holder { get; set; }
        public bool expired { get; set; }
    }

    public class CardHolder
    {
        public Identification identification { get; set; }
        public string name { get; set; }
    }

    public class Identification
    {
        public string type { get; set; }
        public string number { get; set; }
    }


}
