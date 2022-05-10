using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flare.Api.Infrastructure
{
    public class OrderedContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) => base.CreateProperties(type,
                                                                                                                                             memberSerialization)
            .OrderBy(p => p.PropertyName)
            .ToList();
    }
}
