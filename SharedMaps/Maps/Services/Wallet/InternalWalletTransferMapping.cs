using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace SharedMaps.Maps.Services.Wallet
{
    class InternalWalletTransferMapping : Profile
    {
        public InternalWalletTransferMapping()
        {
            CreateMap<SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request, SharedModelDTO.Models.Transaction.Wallet.WalletTransaction>()
                .ForMember(to => to.Value, from => from.MapFrom(x => x.amount))
                .ForMember(to => to.CurrencyType, from => from.MapFrom(x => x.currency))
                .ForMember(to => to.CustomerIdTo, from => from.MapFrom(x => x.customer_id))
                .ForMember(to => to.TransactionDate, from => from.MapFrom(x => DateTime.Now.ToString("yyyyMMdd")));

            CreateMap<SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request, SharedModelDTO.Models.Transaction.Detail.TransactionDetailModel>()
              .ForMember(to => to.CreditTax, from => from.MapFrom(x => x.credit_tax))
              .ForMember(to => to.TransactionDate, from => from.MapFrom(x => DateTime.Now.ToString("yyyyMMdd")));



            CreateMap<SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request, SharedModelDTO.Models.Transaction.Wallet.InternalWalletTransferModel>().ConvertUsing((lRequest, LotBatch) =>
            {
                LotBatch = new SharedModelDTO.Models.Transaction.Wallet.InternalWalletTransferModel()
                {
                    WalletTransaction = new List<SharedModelDTO.Models.Transaction.Wallet.WalletTransaction>()
                };

                SharedModelDTO.Models.Transaction.Wallet.WalletTransaction TransactionToAdd;

                /* foreach (SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request item in lRequest)
                 {*/
                TransactionToAdd = Mapper.Map<SharedModelDTO.Models.Transaction.Wallet.WalletTransaction>(lRequest);
                TransactionToAdd.TransactionDetail = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionDetailModel>(lRequest);
                LotBatch.WalletTransaction.Add(TransactionToAdd);
                //  }

                return LotBatch;
            });



            CreateMap<SharedModelDTO.Models.Transaction.Wallet.WalletTransaction, SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response>()
            .ForMember(to => to.transactionLot_id, from => from.MapFrom(x => x.idTransaction))
            .ForMember(to => to.status, from => from.MapFrom(x => x.idStatus))
            .ForMember(to => to.customer_id, from => from.MapFrom(x => x.CustomerIdTo))
            .ForMember(to => to.amount, from => from.MapFrom(x => x.Value))
            .ForMember(to => to.currency, from => from.MapFrom(x => x.CurrencyType))
            .ForMember(to => to.credit_tax, from => from.MapFrom(x => x.TransactionDetail.CreditTax));
            //.ForMember(to => to.payout_date, from => from.MapFrom(x => x.TransactionDetail.TransacionDate));

            CreateMap<SharedModelDTO.Models.Transaction.Wallet.InternalWalletTransferModel, SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response>().ConvertUsing((LotBatch, lRequest) =>
           {
               lRequest = new SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response();

                SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response ResponseTransaction;

                foreach (SharedModelDTO.Models.Transaction.Wallet.WalletTransaction item in LotBatch.WalletTransaction)
                {
                   ResponseTransaction = Mapper.Map<SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response>(item);
                   lRequest = ResponseTransaction;
                // ResponseTransaction.transaction_id = item.idTransaction;


                //lRequest..Add(ResponseTransaction);
                }

                return lRequest;
           });


        }
    }
}
