using System.Collections.Generic;
using AutoMapper;

namespace SharedMaps.Converters.View
{
    public class ViewMapper
    {
        public SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel MapperModels(SharedModel.Models.View.Dashboard.List.Request data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel>(data);
            return Result;
        }
        public SharedModel.Models.View.Dashboard.List.Response MapperModels(List<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel> data)
        {
            var Result = Mapper.Map<SharedModel.Models.View.Dashboard.List.Response>(data);
            return Result;
        }
    }
}
