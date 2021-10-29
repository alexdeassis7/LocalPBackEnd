using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Database.General
{
    public class BankCodesModel
    {
        public class BankCodes
        { 
            public string countryCode { get; set; }

            public string bankCode { get; set; }
        }

        public class BankCodesOrdered 
        { 
            public string countryCode { get; set; }

            public List<string> bankCodes { get; set; }
        }
    }
}
