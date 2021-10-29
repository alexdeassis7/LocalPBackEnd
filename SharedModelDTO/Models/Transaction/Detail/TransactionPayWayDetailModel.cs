using System;

namespace SharedModelDTO.Models.Transaction.Detail
{
    public class TransactionPayWayDetailModel
    {
        #region Properties::Public
        public Int64 idTransactionPayWayDetail { get; set; }
        public int AmountPaymentInstallments { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Int64 FinalAmount { get; set; }
        public Int64 idTransaction { get; set; }
        public Int64 idStatus { get; set; }
        public bool Active { get; set; }
        #endregion
    }
}
