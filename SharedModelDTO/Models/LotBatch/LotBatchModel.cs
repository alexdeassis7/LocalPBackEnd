using System;
using System.Collections.Generic;

using SharedModelDTO.Models.Transaction;

namespace SharedModelDTO.Models.LotBatch
{
    public class LotBatchModel
    {
        #region Properties::Public
        public Int64 idTransactionLot { get; set; }
        public Int64 LotNumber { get; set; }
        public string LotCode { get; set; }
        public string Description { get; set; }
        public string LotDate { get; set; }
        public string CustomerName { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public Int64 GrossAmount { get; set; }
        public Int64 NetAmount { get; set; }
        public Int64 TaxWithholdings { get; set; }
        public Int64 Balance { get; set; }
        public Int64 idFlow { get; set; }
        public Int64 idStatus { get; set; }
        public bool Active { get; set; }
        public List<Transaction.TransactionModel> Transactions { get; set; }

        #endregion

        #region Methods::Public
        public virtual void Create()
        {
            //CREATE BATCH
            // { ... }

            //CREATE TRANSACTIONS
            //foreach (TransactionModel Transaction in this.Transactions)
            //{
            //    Transaction.Create();
            //    Transaction.TransactionDetail.Create();
            //    Transaction.TransactionRecipientDetail.Create();
            //    Transaction.TransactionPayWayDetail.Create();
            //}
        }
        public virtual void Update()
        {
            //UPDATEA BATCH
            // { ... }

            //UPDATE TRANSACTIONS
            //foreach (TransactionModel Transaction in this.Transactions)
            //{
            //    Transaction.Update();
            //    Transaction.TransactionDetail.Update();
            //    Transaction.TransactionRecipientDetail.Update();
            //    Transaction.TransactionPayWayDetail.Update();
            //}
        }
        public virtual void Delete(Int64 id)
        {
            //DELETE TRANSACTIONS
            //foreach (TransactionModel Transaction in this.Transactions)
            //{
            //    Transaction.TransactionPayWayDetail.Delete(Transaction.TransactionPayWayDetail.idTransactionPayWayDetail);
            //    Transaction.TransactionRecipientDetail.Delete(Transaction.TransactionRecipientDetail.idTransactionRecipientDetail);
            //    Transaction.TransactionDetail.Delete(Transaction.TransactionDetail.idTransactionDetail);
            //    Transaction.Delete(Transaction.idTransaction);
            //}

            //DELETE BATCH
            // { ... }
        }
        public virtual void List() { }
        #endregion
    }
}
