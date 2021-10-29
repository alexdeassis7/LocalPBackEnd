using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMaps.Converters.Services.Chile
{
    public class PayOutMapperChile
    {
        /* PAYOUT ==> POST ==> REQUEST -> DTO */
        public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Chile.PayOutChile.Create.Request> data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
            return Result;
        }

        /* PAYOUT ==> POST ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Chile.PayOutChile.Create.Response> MapperModels(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Chile.PayOutChile.Create.Response>>(data);
            return Result;
        }

        public List<SharedModel.Models.Services.Chile.PayOutChile.LotBatch> MapperModelsBatch(List<SharedModelDTO.Models.LotBatch.LotBatchModel> data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Chile.PayOutChile.LotBatch>>(data);
            return Result;
        }
    }
}
