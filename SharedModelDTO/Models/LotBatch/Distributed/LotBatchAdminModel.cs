using System;

namespace SharedModelDTO.Models.LotBatch.Distributed
{
    public class LotBatchAdminModel : LotBatchModel
    {
        #region Properties::Public
        public string CustomerName { get; set; }
        public string Commissions { get; set; }
        public string Vat { get; set; }
        public string BankCost { get; set; }
        public string BankCostVat { get; set; }
        public string GrossRevenuePerception { get; set; }
        public string TaxDebit { get; set; }
        public string TaxCredit { get; set; }
        public string Rounding { get; set; }
        public string PayVat { get; set; }
        public string BankBalance { get; set; }
        public string TransactionType { get; set; }

        public string Status { get; set; }
        #endregion

        #region Methods::Public
        public override void Create() { }
        public override void Update() { }
        public override void Delete(Int64 id) { }
        public override void List() { }
        #endregion
    }
}
