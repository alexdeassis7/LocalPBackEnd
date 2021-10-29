using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Beneficiary
{
    public class Beneficiary
    {
        public string Recipient { get; set; }
        public string DocumentNumber { get; set; }
        public string CBU { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public string SubMerchantAddress { get; set; }
        public Int64 idTransaction { get; set; }
        public string Ticket { get; set; }
    }

    public class CertificateData 
    {
        public Int64 idTransaction { get; set; }
        public byte[] data { get; set; }
        public string certificateNumber { get; set; }
    }
}
