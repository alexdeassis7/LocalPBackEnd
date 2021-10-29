using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Services.Payouts
{
    public class Payouts
    {
        public class ListPayoutsDownload
        {
            public class Request
            {
                public int PaymentType { get; set; }
                public string FileCode { get; set; }
                public string idMerchant { get; set; }
                public string idSubMerchant { get; set; }
                public string dateTo { get; set; }
                public string amount { get; set; }
                public int operationType { get; set; }
                public int includeDuplicateAmounts { get; set; }
                public int internalFiles { get; set; }
                public string provider { get; set; }
                public string txLimit { get; set; }
                public string txMaxAmount { get; set; }
                public string merchantId { get; set; }
            }
            public class Response
            {
                public string LotNumber { get; set; }

                public string Ticket { get; set; }

                public string TransactionDate { get; set; }

                public string idEntityUser { get; set; }

                public string LastName { get; set; }

                public string SubMerchantIdentification { get; set; }

                public string GrossValueClient { get; set; }

                public string TaxWithholdings { get; set; }

                public string TaxWithholdingsARBA { get; set; }

                public string LocalTax { get; set; }

                public string NetAmount { get; set; }

                public bool Repeated { get; set; }

                public bool HistoricalyRepetead { get; set; }

                public string InternalDescription { get; set; }

                public string PreRegisterLot { get; set; }

                public bool PreRegisterApproved { get; set; }

                public string BeneficiaryName { get; set; }

                public string AccountNumber { get; set; }

                public string TicketAlternative { get; set; }
                public string LotOut { get; set; }
                public string Bank { get; set; }
            }
        }

        public class UploadModel 
        { 
            public string ticket { get; set; }
            public string status { get; set; }
        }

        public class UploadModelMifel : UploadModel
        {
            public decimal amount { get; set; }
            public string beneficiaryName { get; set; }
        }

        public class UploadModelPlural : UploadModel
        {
            public string rejectDetail { get; set; }
        }

        public class UploadModelItau : UploadModel
        {
            public string rejectDetail { get; set; }
        }

        public class TicketsToCertificate 
        { 
            public List<string> tickets { get; set; }
        }

        public class TransactionToNotify
        {
            public List<string> Transactions { get; set; }
        }
    }
}
