using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Beneficiary
{
    public class BlackList
    {
        public string beneficiaryName { get; set; }
        public string accountNumber { get; set; }
        public string documentId { get; set; }
        public string isSender { get; set; }
    }
}
