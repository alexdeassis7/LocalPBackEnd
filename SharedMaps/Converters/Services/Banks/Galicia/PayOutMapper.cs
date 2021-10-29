using System.Collections.Generic;
using AutoMapper;

namespace SharedMaps.Converters.Services.Banks.Galicia
{
    public class PayOutMapper
    {
        /* PAYOUT ==> POST ==> REQUEST -> DTO */
        public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Request> data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
            return Result;
        }

        /* PAYOUT ==> POST ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response> MapperModels(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response>>(data);
            return Result;
        }

        /* PAYOUT ==> UPDATE ==> REQUEST -> DTO */
        public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Update.Request> data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
            return Result;
        }

        /* PAYOUT ==> UPDATE ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.Update.Response> MapperModelsUpdate(SharedModelDTO.Models.LotBatch.LotBatchModel data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Banks.Galicia.PayOut.Update.Response>>(data);
            return Result;
        }

        /* PAYOUT ==> DELETE ==> REQUEST -> DTO */
        public SharedModelDTO.Models.LotBatch.LotBatchModel MapperModels(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Request> data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.LotBatchModel>(data);
            return Result;
        }

        /* PAYOUT ==> DELETE ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> MapperModelsDelete(List<SharedModelDTO.Models.LotBatch.LotBatchModel> data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response>>(data);
            return Result;
        }

        /* PAYOUT ==> LIST ==> DTO -> RESPONSE */
        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch> MapperModelsBatch(List<SharedModelDTO.Models.LotBatch.LotBatchModel> data)
        {
            var Result = Mapper.Map<List<SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch>>(data);
            return Result;
        }
    }
}
