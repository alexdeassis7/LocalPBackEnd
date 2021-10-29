using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Services.Decidir
{
    public class TokenizeCards
    {
        public List<TokenizeCard> tokens { get; set; }
    }

    public class TokenizeCard
    {
        public string token_card { get; set; }
        public int payment_method_id { get; set; }
        public string bin { get; set; }
        public string last_four_digits { get; set; }
        public string expiration_month { get; set; }

        public string expiration_year { get; set; }

        public CardHolder cardholder { get; set; }

        public bool expired { get; set; }
    }
    
}
