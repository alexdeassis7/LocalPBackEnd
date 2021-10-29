using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

namespace SharedMaps.Maps
{
    public static class LocalPaymentMapper
    {
        public static void InitializeAutoMapper()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile(new Services.Banks.Galicia.PayOutMapping());
                x.AddProfile(new Services.Wallet.InternalWalletTransferMapping());
                x.AddProfile(new View.DashboardMapping());
                x.AddProfile(new Services.Banks.Bind.DebinMapping());
                x.AddProfile(new Services.Colombia.Banks.Bancolombia.PayOutMappingColombia());
                x.AddProfile(new Services.Brasil.PayOutMappingBrasil());
                x.AddProfile(new Services.Uruguay.PayOutMappingUruguay());
                x.AddProfile(new Services.Chile.PayOutMappingChile());
                x.AddProfile(new Services.Mexico.PayOutMappingMexico());
            });
        }
    }
}
