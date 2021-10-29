using System.Collections.Generic;
using AutoMapper;

namespace SharedMaps.Converters.Services.Colombia.Banks.Bancolombia
{
    public class PayOutMapperColombia
    {
        /* PAYOUT ==> POST ==> REQUEST -> DTO */
        public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Request> data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
            return Result;
        }

        /* PAYOUT ==> POST ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response> MapperModels(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response>>(data);
            return Result;
        }

        /* PAYOUT ==> UPDATE ==> REQUEST -> DTO */
        public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Update.Request> data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
            return Result;
        }

        /* PAYOUT ==> UPDATE ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Update.Response> MapperModelsUpdate(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Update.Response>>(data);
            return Result;
        }
        //public List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.List.Response> MapperModelsBatch(List<SharedModelDTO.Models.LotBatch.LotBatchModel> lots)
        //{
        //    var responses = new List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.List.Response>();
        //    foreach (var data in lots)
        //    { 
        //         var res = Mapper.Map<List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.List.Response>>(data);
        //        responses.AddRange(res);
        //    }
        //    return responses;
        //}

        public List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.LotBatchColombia> MapperModelsBatch(List<SharedModelDTO.Models.LotBatch.LotBatchModel> data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.LotBatchColombia>>(data);
            return Result;
        }

        /* PAYOUT ==> DELETE ==> REQUEST -> DTO */
        //public SharedModelDTO.Models.Colombia.LotBatchColombiaModel LotBatchModel MapperModels(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Delete.Request> data)
        //{
        //    var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
        //    return Result;
        //}

        /* PAYOUT ==> DELETE ==> DTO -> RESPONSE */
        //public List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Delete.Response> MapperModelsDelete(List<SharedModelDTO.Models.LotBatch.LotBatchModel> data)
        //{
        //    var Result = Mapper.Map<List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Delete.Response>>(data);
        //    return Result;
        //}

        /* PAYOUT ==> LIST ==> DTO -> RESPONSE */
        //public List<SharedModel.Models.Colombia.Banks.Bancolombia.PayOut.LotBatch> MapperModelsBatch(List<SharedModelDTO.Models.LotBatch.LotBatchModel> data)
        //{
        //    var Result = Mapper.Map<List<SharedModel.Models.Colombia.Banks.Bancolombia.PayOut.LotBatch>>(data);
        //    return Result;
        //}
    }
}
