using SharedModel.Models.Database.Security;
using SharedModel.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SharedModel.Models.Database.General.BankCodesModel;

namespace WA_LP.Cache
{
    public static class BankValidateCacheService
    {
        private static List<BankCodesOrdered> bankCodes;
        public static void Init()
        {
            AddToCache();
        }

        public static void AddToCache()
        {
            var Codes =  SharedBusiness.Common.Configuration.GetBankCodes();
            bankCodes = Codes.GroupBy(x => x.countryCode, x => x.bankCode,(key,g) => new BankCodesOrdered{ countryCode = key, bankCodes = g.ToList()}).ToList();
        }
        public static List<BankCodesOrdered> Get()
        {
            return bankCodes;
        }

    }
}