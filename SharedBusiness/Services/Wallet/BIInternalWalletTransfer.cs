using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.Services.Wallet
{
   public class BIInternalWalletTransfer
    {
        public SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response WalletTransfer(SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request data, string customer, bool TransactionMechanism)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                //SharedMaps.Converters.Services.Wallet.Wallet LPMapper = new SharedMaps.Converters.Services.Wallet.Wallet();
               // SharedModelDTO.Models.Transaction.Wallet.InternalWalletTransferModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                DAO.DataAccess.Services.DbWallet LPDAO = new DAO.DataAccess.Services.DbWallet();

                SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response Response = new SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response();
                Response = LPDAO.CreateInternalWalletTransaction(data, customer, TransactionMechanism);



                /* CASTEO LOT - RESPONSE */
                //SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response Response = LPMapper.MapperModels(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
