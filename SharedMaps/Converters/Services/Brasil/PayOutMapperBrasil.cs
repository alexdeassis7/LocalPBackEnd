using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace SharedMaps.Converters.Services.Brasil
{
    public class PayOutMapperBrasil
    {
        /* PAYOUT ==> POST ==> REQUEST -> DTO */
        public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Request> data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
            return Result;
        }

        /* PAYOUT ==> POST ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response> MapperModels(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response>>(data);
            return Result;
        }

        /* PAYOUT ==> UPDATE ==> REQUEST -> DTO */
        //public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Brasil.PayOutBrasil.Update.Request> data)
        //{
        //    var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
        //    return Result;
        //}

        /* PAYOUT ==> UPDATE ==> DTO -> RESPONSE */
        //public List<SharedModel.Models.Services.Brasil.PayOutBrasil.Update.Response> MapperModelsUpdate(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        //{
        //    var Result = Mapper.Map<List<SharedModel.Models.Services.Brasil.PayOutBrasil.Update.Response>>(data);
        //    return Result;
        //}

        public List<SharedModel.Models.Services.Brasil.PayOutBrasil.LotBatch> MapperModelsBatch(List<SharedModelDTO.Models.LotBatch.LotBatchModel> data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Brasil.PayOutBrasil.LotBatch>>(data);
            return Result;
        }
    }
}
