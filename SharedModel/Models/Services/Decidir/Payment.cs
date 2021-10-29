using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Services.Decidir
{
    public class Payment
    {
        public string payment_id { get; set; }
        public string site_transaction_id { get; set; }
    }
    
    public class Refund
    {
        public string payment_id { get; set; }
        public string amount { get; set; }
        public string refund_id { get; set; }

        public string site_transaction_id { get; set; }
    }
}
