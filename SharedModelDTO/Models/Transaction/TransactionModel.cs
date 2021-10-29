using System;
using SharedModelDTO.Models.Transaction.Detail;

namespace SharedModelDTO.Models.Transaction
{
    public class TransactionModel
    {
        #region Properties::Public
        public Int64 idTransaction { get; set; }
        public string TransactionDate { get; set; }
        public Int64 Value { get; set; }
        public Int64 Version { get; set; }
        public Int64 idTransactionLot { get; set; }
        public Int64 idTransactionType { get; set; }
        public Int64 idStatus { get; set; }
        public Int64 idEntityAccount { get; set; }
        public string Status { get; set; }
        public string StatusDetail { get; set; }
        public bool Active { get; set; }
        public string StatusObservation { get; set; }
        public Int64 idCurrencyType { get; set; }
        public string id { get; set; }
        public string type_of_id { get; set; }
        public string bank_code { get; set; }
        public string Ticket { get; set; }


        private TransactionDetailModel _TransactionDetail = new TransactionDetailModel();
        public TransactionDetailModel TransactionDetail { get { return _TransactionDetail; } set { _TransactionDetail = value; } }
        private TransactionRecipientDetailModel _TransactionRecipientDetail = new TransactionRecipientDetailModel();
        public TransactionRecipientDetailModel TransactionRecipientDetail { get { return _TransactionRecipientDetail; } set { _TransactionRecipientDetail = value; } }
        private TransactionPayWayDetailModel _TransactionPayWayDetail = new TransactionPayWayDetailModel();
        public TransactionPayWayDetailModel TransactionPayWayDetail { get { return _TransactionPayWayDetail; } set { _TransactionPayWayDetail = value; } }
        private TransactionDebinDetailModel _TransactionDebinDetail = new TransactionDebinDetailModel();
        public TransactionDebinDetailModel TransactionDebinDetail { get { return _TransactionDebinDetail; } set { _TransactionDebinDetail = value; } }

        private TransactionCustomerInformation _TransactionCustomerInformation = new TransactionCustomerInformation();
        public TransactionCustomerInformation TransactionCustomerInformation { get { return _TransactionCustomerInformation; } set { _TransactionCustomerInformation = value; } }

        private TransactionSubMerchantDetailModel _TransactionSubMerchantDetail = new TransactionSubMerchantDetailModel();
        public TransactionSubMerchantDetailModel TransactionSubMerchantDetail { get { return _TransactionSubMerchantDetail; } set { _TransactionSubMerchantDetail = value; } }

        #endregion

        #region Methods::Public
        public virtual void Create() { }
        public virtual void Update() { }
        public virtual void Delete(Int64 id) { }
        public virtual void List() { }
        #endregion
    }
}
