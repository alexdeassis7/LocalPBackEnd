using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WA_LP.Serializer
{
    public class IgnoreErrorPropertiesResolver : DefaultContractResolver
    {
        List<string> ignoreList = new List<string>(new string[]{
                "InputStream",
                "Filter",
                "Length",
                "Position",
                "ReadTimeout",
                "WriteTimeout",
                "LastActivityDate",
                "LastUpdatedDate",
                "Session"
            });
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (ignoreList.Contains(property.PropertyName)) {
                property.Ignored = true;
            }
            return property;
        }
    }
}