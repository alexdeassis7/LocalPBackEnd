using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction
{
    public class TransAgForRetentionsFilterModel
    {
        public string CertTypeCode { get; set; }
        public int? MerchantId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }
}
