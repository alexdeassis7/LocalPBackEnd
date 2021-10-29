using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction
{
    public class RetentionsByFilterDownloadModel : RetentionsByDateDownloadModel
    {
        public long MerchantID { get; set; }

        public long PayoutID { get; set; }

        public string CertificateNumber { get; set; }
    }
}
