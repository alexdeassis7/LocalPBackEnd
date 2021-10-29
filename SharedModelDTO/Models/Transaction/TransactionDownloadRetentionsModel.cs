using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction
{
    public class TransactionDownloadRetentionsModel
    {
        public MerchantModel Merchant { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Organism { get; set; }
        public int CertificatesCount { get; set; }
    }
}
