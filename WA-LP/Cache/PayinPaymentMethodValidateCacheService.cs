using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_LP.Cache
{
    public static class PayinPaymentMethodValidateCacheService
    {
        private static List<string> paymentMethodCodes;
        public static void Init()
        {
            AddToCache();
        }

        public static void AddToCache()
        {
            var Codes = SharedBusiness.Common.Configuration.GetPaymentMethodCodes();
            paymentMethodCodes = Codes;
        }
        public static List<string> Get()
        {
            return paymentMethodCodes;
        }
    }
}