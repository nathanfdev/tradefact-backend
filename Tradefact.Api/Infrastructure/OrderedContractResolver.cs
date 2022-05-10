using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Infrastructure
{
    public class OrderedContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) => base.CreateProperties(type,
                                                                                                                                             memberSerialization)
            .OrderBy(p => p.PropertyName)
            .ToList();
    }
}
