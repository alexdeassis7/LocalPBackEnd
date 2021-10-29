using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMaps.Converters.Services.Mexico
{
    public class PayOutMapperMexico
    {
        /* PAYOUT ==> POST ==> REQUEST -> DTO */
        public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Request> data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
            return Result;
        }

        /* PAYOUT ==> POST ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response> MapperModels(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response>>(data);
            return Result;
        }

        /* PAYOUT ==> UPDATE ==> REQUEST -> DTO */
        //public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Mexico.PayOutMexico.Update.Request> data)
        //{
        //    var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
        //    return Result;
        //}

        /* PAYOUT ==> UPDATE ==> DTO -> RESPONSE */
        //public List<SharedModel.Models.Services.Mexico.PayOutMexico.Update.Response> MapperModelsUpdate(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        //{
        //    var Result = Mapper.Map<List<SharedModel.Models.Services.Mexico.PayOutMexico.Update.Response>>(data);
        //    return Result;
        //}

        public List<SharedModel.Models.Services.Mexico.PayOutMexico.LotBatch> MapperModelsBatch(List<SharedModelDTO.Models.LotBatch.LotBatchModel> data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Mexico.PayOutMexico.LotBatch>>(data);
            return Result;
        }
    }
}
