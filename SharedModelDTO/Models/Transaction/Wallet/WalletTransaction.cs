using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction.Wallet
{
    public class WalletTransaction : Transaction.TransactionModel
    {
        public string CurrencyType { get; set; }
        public string CustomerIdTo { get; set; }
    }
}
