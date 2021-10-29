using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Security
{
    public class Token
    {
        public string token_id { get; set; }
    }

    public class TokenWeb:Token
    {
        public string customer_id { get; set; }
    }
}
