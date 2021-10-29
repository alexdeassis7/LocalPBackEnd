using SharedModel.Models.Database.Security;
using SharedModel.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_LP.Cache
{
    public static class LogInCacheService
    {
        public static void Init()
        {
            CacheService.Init();
        }

        public static void AddToCache(Authentication.Account account)
        {
            CacheService.Add(account.Login.ClientID, account);
        }
        public static Authentication.Account Get(string clientId)
        {
           var client = CacheService.Get(clientId);
            if (client != null)
                return (Authentication.Account)client;
            else
                return null;
        }

    }
}