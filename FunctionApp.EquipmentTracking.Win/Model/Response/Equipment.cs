using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FunctionApp.EquipmentTracking.Win.Model
{
    public class Equipment
    {

        [JsonIgnore]
        public bool Errors { get; set; }


        [JsonProperty("query")]
        public TrackRequest Search { get; set; }
        public Results Results { get; set; }


        public void SetErrorState(bool state)
        {
            this.Errors = state;
        }
    }

}
