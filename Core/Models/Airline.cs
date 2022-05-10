using Core.Enums;
using Newtonsoft.Json;

namespace Core.Models
{
    public class Airline
    {
        [JsonProperty("iata2LetterCcode")] 
        public string IATA2LetterCcode { get; set; }
        [JsonProperty("awbPrefix")] 
        public string AWBPrefix { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public bool Active { get; set; }
        public AWBTracking Tracking { get; set; }
    }

    public class AWBTracking
    {
        public bool Enabled { get; set; }
        [JsonIgnore]
        public string Provider { get; set; }
    }
}
