using System;


namespace SharedModelDTO.Models.Transaction.Detail
{
    public class TransactionDebinDetailModel
    {
        #region Properties::Public
        public Int64 idTransactionDebinDetail { get; set; }
        public Int64 idTransaction { get; set; }
        public Int64 idStatus { get; set; }
        public bool Active { get; set; }

        public string id_ticket { get; set; }
        public string bank_transaction { get; set; }
        public string currency { get; set; }
        public Int32 amount { get; set; }
        public string beneficiary_softd { get; set; }
        public string site_transaction_id { get; set; }
        public string description { get; set; }
        public string concept_code { get; set; }
        public string payment_type { get; set; }
        public string payout_date { get; set; }
        public string alias { get; set; }
        public string cbu { get; set; }
        public string buyer_cuit { get; set; }
        public string buyer_name { get; set; }
        public string buyer_bank_code { get; set; }
        public string buyer_cbu { get; set; }
        public string buyer_alias { get; set; }
        public string buyer_bank_description { get; set; }
        public string status { get; set; }

        #endregion
    }
}
