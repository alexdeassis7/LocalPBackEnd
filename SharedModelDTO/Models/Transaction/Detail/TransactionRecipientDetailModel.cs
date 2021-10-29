using System;

namespace SharedModelDTO.Models.Transaction.Detail
{
    public class TransactionRecipientDetailModel
    {
        #region Properties::Public
        public Int64 idTransactionRecipientDetail { get; set; }
        public string Recipient { get; set; }
        public string RecipientCUIT { get; set; }
        public string CBU { get; set; }
        public string RecipientAccountNumber { get; set; }
        public string TransactionAcreditationDate { get; set; }
        public string Description { get; set; }
        public string InternalDescription { get; set; }
        public string ConceptCode { get; set; }
        public string BankAccountType { get; set; }
        public string EntityIdentificationType { get; set; }
        public string CurrencyType { get; set; }
        public string PaymentType { get; set; }
        public Int64 idTransaction { get; set; }        
        public Int64 idStatus { get; set; }
        public bool Active { get; set; }

        public string IdType { get; set; }
        public string Id { get; set; }
        public string BankCode { get; set; }
        public string BankBranch { get; set; }
        public string RecipientPhoneNumber { get; set; }


        #endregion
    }
}
