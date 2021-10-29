using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types
{
    public class Common
    {
        public string BarCode { get; set; }
        public string CustomerID { get; set; }
        public string Value { get; set; }
        public string TransactionType { get; set; }
        public string ExpirtationDate { get; set; }
        public string Currency { get; set; }
        public string Invoice { get; set; }
        public string AdditionalInfo { get; set; }
        public string Ticket { get; set; }
        public string ClientIdentification { get; set; }

        public string ClientName { get; set; }
        public string ClientMail { get; set; }
        public string CustomerFrom { get; set; }

        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientBirthDate { get; set; }
        public string ClientCountry { get; set; }
        public string ClientCity { get; set; }
        public string ClientAnnotation { get; set; }

        public string transaction_id { get; set; } = null;
    }
}
