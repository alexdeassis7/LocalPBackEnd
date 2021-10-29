using System;

namespace SharedModelDTO.Models.LotBatch.Distributed
{
    public class LotBatchMerchantModel : LotBatchModel
    {
        #region Properties::Public
        public string ProcessDate { get; set; }
        #endregion

        #region Methods::Public
        public override void Create() { }
        public override void Update() { }
        public override void Delete(Int64 id) { }
        public override void List() { }
        #endregion
    }
}
