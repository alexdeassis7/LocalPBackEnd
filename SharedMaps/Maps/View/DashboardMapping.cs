using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMaps.Maps.View
{
    public class DashboardMapping : Profile
    {
        public DashboardMapping()
        {

            CreateMap<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel, SharedModel.Models.View.Dashboard.DashboardLotes>()
            .ForMember(to => to.bank_cost, from => from.MapFrom(x => x.BankCost))
            .ForMember(to => to.bank_cost_vat, from => from.MapFrom(x => x.BankCostVat))
            .ForMember(to => to.commissions, from => from.MapFrom(x => x.Commissions))
            .ForMember(to => to.vat, from => from.MapFrom(x => x.Vat))
            .ForMember(to => to.tax_withholdings, from => from.MapFrom(x => x.TaxWithholdings))
            .ForMember(to => to.tax_debit, from => from.MapFrom(x => x.TaxDebit))
            .ForMember(to => to.tax_credit, from => from.MapFrom(x => x.TaxCredit))
            .ForMember(to => to.rounding, from => from.MapFrom(x => x.Rounding))
            .ForMember(to => to.pay_vat, from => from.MapFrom(x => x.PayVat))
            .ForMember(to => to.net_amount, from => from.MapFrom(x => x.NetAmount))
            .ForMember(to => to.lot_number, from => from.MapFrom(x => x.LotNumber))
            .ForMember(to => to.idTransactionLot, from => from.MapFrom(x => x.idTransactionLot))
            .ForMember(to => to.status, from => from.MapFrom(x => x.Status))
            .ForMember(to => to.gross_amount, from => from.MapFrom(x => x.GrossAmount))
            .ForMember(to => to.customer_name, from => from.MapFrom(x => x.CustomerName))
            .ForMember(to => to.accreditation_date, from => from.MapFrom(x => x.LotDate))
             .ForMember(to => to.transaction_type, from => from.MapFrom(x => x.TransactionType))
            .ForMember(to => to.balance, from => from.MapFrom(x => x.Balance));



            CreateMap<List<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel>, SharedModel.Models.View.Dashboard.List.Response>().ConvertUsing((LotBatch, lResponse) =>
            {
                lResponse = new SharedModel.Models.View.Dashboard.List.Response();

                SharedModel.Models.View.Dashboard.DashboardLotes ResponseTransaction;

                foreach (SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel item in LotBatch)
                {
                    ResponseTransaction=Mapper.Map<SharedModel.Models.View.Dashboard.DashboardLotes>(item);
                    lResponse.dashboardLotes.Add(ResponseTransaction);
                }

                return lResponse;
            });
        }
    }
}
