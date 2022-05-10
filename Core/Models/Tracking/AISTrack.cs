using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Tracking
{
    public class AISTrack
    {
        [JsonProperty("MMSI")]
        public long Mmsi { get; set; }

        [JsonProperty("TIMESTAMP")]
        public string Timestamp { get; set; }

        [JsonProperty("LATITUDE")]
        public double Latitude { get; set; }

        [JsonProperty("LONGITUDE")]
        public double Longitude { get; set; }

        [JsonProperty("COURSE")]
        public long Course { get; set; }

        [JsonProperty("SPEED")]
        public double Speed { get; set; }

        [JsonProperty("HEADING")]
        public long Heading { get; set; }

        [JsonProperty("NAVSTAT")]
        public long Navstat { get; set; }

        [JsonProperty("IMO")]
        public long Imo { get; set; }

        [JsonProperty("NAME")]
        public string Name { get; set; }

        [JsonProperty("CALLSIGN")]
        public string Callsign { get; set; }

        [JsonProperty("TYPE")]
        public long Type { get; set; }

        [JsonProperty("A")]
        public long A { get; set; }

        [JsonProperty("B")]
        public long B { get; set; }

        [JsonProperty("C")]
        public long C { get; set; }

        [JsonProperty("D")]
        public long D { get; set; }

        [JsonProperty("DRAUGHT")]
        public double Draught { get; set; }

        [JsonProperty("DESTINATION")]
        public string Destination { get; set; }

        [JsonProperty("ETA_AIS")]
        public string EtaAis { get; set; }

        [JsonProperty("ETA")]
        public DateTimeOffset Eta { get; set; }

        [JsonProperty("SRC")]
        public string Src { get; set; }

        [JsonProperty("ZONE")]
        public string Zone { get; set; }

        [JsonProperty("ECA")]
        public bool Eca { get; set; }
    }
}
