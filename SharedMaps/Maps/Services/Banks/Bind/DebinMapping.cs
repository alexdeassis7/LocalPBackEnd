using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace SharedMaps.Maps.Services.Banks.Bind
{
    class DebinMapping : Profile
    {
        public DebinMapping()
        {
            CreateMap<SharedModel.Models.Services.Banks.Bind.Debin.RequestDebin, SharedModelDTO.Models.Transaction.Debin.DebinTransaction>()

                .ForMember(to => to.Value, from => from.MapFrom(x => x.amount))
                .ForMember(to => to.TransactionDate, from => from.MapFrom(x => DateTime.Now.ToString("yyyyMMdd")));


            CreateMap<SharedModel.Models.Services.Banks.Bind.Debin.RequestDebin, SharedModelDTO.Models.Transaction.Detail.TransactionDebinDetailModel>()
                .ForMember(to => to.id_ticket, from => from.MapFrom(x => x.id_ticket))
                .ForMember(to => to.idStatus, from => from.MapFrom(x => 1))
                .ForMember(to => to.bank_transaction, from => from.MapFrom(x => x.bank_transaction))
                .ForMember(to => to.currency, from => from.MapFrom(x => x.currency))
                .ForMember(to => to.beneficiary_softd, from => from.MapFrom(x => x.beneficiary_softd))
                .ForMember(to => to.site_transaction_id, from => from.MapFrom(x => x.site_transaction_id))
                .ForMember(to => to.description, from => from.MapFrom(x => x.description))
                .ForMember(to => to.concept_code, from => from.MapFrom(x => x.concept_code))
                .ForMember(to => to.payment_type, from => from.MapFrom(x => x.payment_type))
                .ForMember(to => to.payout_date, from => from.MapFrom(x => x.payout_date))
                .ForMember(to => to.alias, from => from.MapFrom(x => x.alias))
                .ForMember(to => to.cbu, from => from.MapFrom(x => x.cbu))
                .ForMember(to => to.buyer_cuit, from => from.MapFrom(x => x.buyer_cuit))
                .ForMember(to => to.buyer_name, from => from.MapFrom(x => x.buyer_name))
                .ForMember(to => to.buyer_bank_code, from => from.MapFrom(x => x.buyer_bank_code))
                .ForMember(to => to.buyer_cbu, from => from.MapFrom(x => x.buyer_cbu))
                .ForMember(to => to.buyer_alias, from => from.MapFrom(x => x.buyer_alias))
                .ForMember(to => to.buyer_bank_description, from => from.MapFrom(x => x.buyer_bank_description))
                .ForMember(to => to.status, from => from.MapFrom(x => x.status));




            CreateMap<SharedModel.Models.Services.Banks.Bind.Debin.RequestDebin, SharedModelDTO.Models.Transaction.Debin.InternalDebinModel>().ConvertUsing((lRequest, LotBatch) =>
                {
                    LotBatch = new SharedModelDTO.Models.Transaction.Debin.InternalDebinModel()
                    {
                        DebinTransaction = new List<SharedModelDTO.Models.Transaction.Debin.DebinTransaction>()
                    };

                    SharedModelDTO.Models.Transaction.Debin.DebinTransaction TransactionToAdd;


                TransactionToAdd = Mapper.Map<SharedModelDTO.Models.Transaction.Debin.DebinTransaction>(lRequest);
                TransactionToAdd.TransactionDebinDetail = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionDebinDetailModel>(lRequest);
                LotBatch.DebinTransaction.Add(TransactionToAdd);


                    return LotBatch;
                });



            CreateMap<SharedModelDTO.Models.Transaction.Debin.DebinTransaction, SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin>()
            .ForMember(to => to.amount, from => from.MapFrom(x => x.Value));



            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionDebinDetailModel, SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin>()
             .ForMember(to => to.transaction_id, from => from.MapFrom(x => x.idTransaction))
            //.ForMember(to => to.id_ticket, from => from.MapFrom(x => x.id_ticket))
            //.ForMember(to => to.bank_transaction, from => from.MapFrom(x => x.bank_transaction))
            .ForMember(to => to.currency, from => from.MapFrom(x => x.currency))
            .ForMember(to => to.beneficiary_softd, from => from.MapFrom(x => x.beneficiary_softd))
            .ForMember(to => to.site_transaction_id, from => from.MapFrom(x => x.site_transaction_id))
            .ForMember(to => to.description, from => from.MapFrom(x => x.description))
            .ForMember(to => to.concept_code, from => from.MapFrom(x => x.concept_code))
            .ForMember(to => to.payment_type, from => from.MapFrom(x => x.payment_type))
            .ForMember(to => to.payout_date, from => from.MapFrom(x => x.payout_date))
            //.ForMember(to => to.alias, from => from.MapFrom(x => x.alias))
            //.ForMember(to => to.cbu, from => from.MapFrom(x => x.cbu))
            .ForMember(to => to.buyer_cuit, from => from.MapFrom(x => x.buyer_cuit))
            .ForMember(to => to.buyer_name, from => from.MapFrom(x => x.buyer_name))
            .ForMember(to => to.buyer_bank_code, from => from.MapFrom(x => x.buyer_bank_code))
            .ForMember(to => to.buyer_cbu, from => from.MapFrom(x => x.buyer_cbu))
            .ForMember(to => to.buyer_alias, from => from.MapFrom(x => x.buyer_alias))
            .ForMember(to => to.buyer_bank_description, from => from.MapFrom(x => x.buyer_bank_description))
            .ForMember(to => to.status, from => from.MapFrom(x => x.status));




            CreateMap<SharedModelDTO.Models.Transaction.Debin.InternalDebinModel, SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin>().ConvertUsing((LotBatch, lRequest) =>
            {
                lRequest = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin();

                SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin ResponseTransaction;

                foreach (SharedModelDTO.Models.Transaction.Debin.DebinTransaction item in LotBatch.DebinTransaction)
                {
                    ResponseTransaction = Mapper.Map<SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin>(item.TransactionDebinDetail);
                    lRequest = ResponseTransaction;
                    
                }

                return lRequest;
            });


        }
    }
}

