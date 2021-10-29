using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace SharedMaps.Converters.Services.Banks.Bind
{
    public class Debin
    {
        public SharedModelDTO.Models.Transaction.Debin.InternalDebinModel MapperModels(SharedModel.Models.Services.Banks.Bind.Debin.RequestDebin data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.Transaction.Debin.InternalDebinModel>(data);
            return Result;
        }

        public SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin MapperModels(SharedModelDTO.Models.Transaction.Debin.InternalDebinModel data)
        {
            var Result = Mapper.Map<SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin>(data);
            return Result;
        }
    }
}