using SharedModelDTO.Models.LotBatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction.Debin
{
    public class InternalDebinModel : LotBatchModel
    {
        public List<DebinTransaction> DebinTransaction { get; set; }

        public override void Create() { }

        public override void Update() { }

        public override void Delete(Int64 id) { }
    }
}
