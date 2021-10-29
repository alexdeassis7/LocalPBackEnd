using System;

namespace SharedModelDTO.Models.Transaction.Detail
{
    public class TransactionDetailModel
    {
        #region Properties::Public
        public Int64 idTransactionDetail { get; set; }
        public Int64 IdTransaction { get; set; }
        public Int64 GrossAmount { get; set; }
        public Int64 NetAmount { get; set; }
        public Int64 TaxWithholdings_Afip { get; set; }
        public Int64 TaxWithholdings_Agip { get; set; }
        public Int64 TaxWithholdings_Arba { get; set; }
        public Int64 GmfTax { get; set; }
        public Int64 Commission { get; set; }
        public Int64 IVACommission { get; set; }
        public Int64 Balance { get; set; }
        public Int64 BankCost { get; set; }
        public Int64 IVABankCost { get; set; }
        public Int64 DebitTax { get; set; }
        public Int64 CreditTax { get; set; }
        public Int64 TotalCostRdo { get; set; }
        public Int64 IVATotal { get; set; }
        public Int64 TransactionDate { get; set; }
        public Int64 idStatus { get; set; }
        public Int64 ExchangeRate { get; set; }

        public bool Active { get; set; }

        public Int16 Version{ get; set; }

        #endregion
    }
}
