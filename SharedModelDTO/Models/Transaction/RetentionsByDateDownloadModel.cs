using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction
{
    public class RetentionsByDateDownloadModel
    {
        public string CertTypeCode { get; set; }
        public MerchantModel Merchant { get; set; }
        public DateTime date { get; set; }
        public int Certificates_Created { get; set; }
        public OrganismModel Organism { get; set; }
    }
}
