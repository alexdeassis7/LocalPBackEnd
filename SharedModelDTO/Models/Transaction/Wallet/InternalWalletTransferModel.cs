using SharedModelDTO.Models.LotBatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction.Wallet
{
    public class InternalWalletTransferModel: LotBatchModel
    {
        public List<WalletTransaction> WalletTransaction { get; set; }

        public override void Create() { }

        public override void Update() { }

        public override void Delete(Int64 id) { }
    }
}
