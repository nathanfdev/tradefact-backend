using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp.EquipmentTracking.Win.Model
{


    [JsonObject(MemberSerialization.OptIn)]
    public class Queries
    {
        [JsonProperty("Query")]
        public List<Equipment> Results { get; set; }
    }


    

}
