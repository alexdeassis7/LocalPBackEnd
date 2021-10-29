using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace SharedMaps.Converters.Services.Wallet
{
   public  class Wallet
    {
        public SharedModelDTO.Models.Transaction.Wallet.InternalWalletTransferModel MapperModels(SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request data)
        {
            var Result = Mapper.Map<SharedModelDTO.Models.Transaction.Wallet.InternalWalletTransferModel>(data);
            return Result;
        }

        public SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response MapperModels(SharedModelDTO.Models.Transaction.Wallet.InternalWalletTransferModel data)
        {
            var Result = Mapper.Map<SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response>(data);
            return Result;
        }
    }
}
