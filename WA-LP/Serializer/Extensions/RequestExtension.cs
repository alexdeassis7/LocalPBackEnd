using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace WA_LP.Serializer.Extensions
{
    public static class RequestExtension
    {
        public static string SerializeRequest(this HttpRequestMessage request)
        {
            try
            {
                var strBuilder = new StringBuilder();
                var header = JsonConvert.SerializeObject(request.Headers, Formatting.None, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new IgnoreErrorPropertiesResolver()
                });
                strBuilder.Append(header);

                var body = request.Content.ReadAsStringAsync().Result;
                strBuilder.Append(body);

                return strBuilder.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}