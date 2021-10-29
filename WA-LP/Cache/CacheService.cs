using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WA_LP.Cache
{
    public static class CacheService
    {
        const string TokenCacheExpiration = "TokenCacheExpiration";

        private static ICacheManager<object> manager;
        public static void Init()
        {
            int tokenCacheExpirationTime = int.Parse(ConfigurationManager.AppSettings[TokenCacheExpiration]); 
            manager = CacheFactory.Build<object>(p => p.WithSystemRuntimeCacheHandle().WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(tokenCacheExpirationTime)));
        }
        public static void Add(string key, object value)
        {
            manager.AddOrUpdate(key, value, _=> value);
        }
        public static object Get(string key)
        {
            return manager.Get(key);
        }

    }
    public class CacheValue
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}