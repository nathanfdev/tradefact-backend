using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Integration.FlightStats
{
    public class FlightStatsResponse
    {
        public Request request { get; set; }
        public List<ScheduledFlight> scheduledFlights { get; set; }
        public List<Connections> connections { get; set; }
        public Appendix appendix { get; set; }

        [JsonPropertyName("error")]
        public Error Error { get; set; }

        [JsonIgnore]
        public bool HasResults => this.connections.Any();

        [JsonIgnore]
        public bool HasErrors => this.Error != null;
    }

    public partial class Error
    {
        [JsonPropertyName("httpStatusCode")]
        public long HttpStatusCode { get; set; }

        [JsonPropertyName("errorId")]
        public Guid ErrorId { get; set; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }
    }


    public class Connections
    {
        public int elapsedTime { get; set; }
        public int score { get; set; }
        public List<ScheduledFlight> scheduledFlight { get; set; }
    }

    public class DepartureAirport
    {
        public string requestedCode { get; set; }
        public string fsCode { get; set; }
    }

    public class ArrivalAirport
    {
        public string requestedCode { get; set; }
        public string fsCode { get; set; }
    }

    public class CodeType
    {
        public string requested { get; set; }
        public string interpreted { get; set; }
    }

    public class Date
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string interpreted { get; set; }
    }

    public class Request
    {
        public DepartureAirport departureAirport { get; set; }
        public ArrivalAirport arrivalAirport { get; set; }
        public CodeType codeType { get; set; }
        public bool departing { get; set; }
        public Date date { get; set; }
        public string url { get; set; }
    }

    public class Operator
    {
        public string carrierFsCode { get; set; }
        public string flightNumber { get; set; }
        public string serviceType { get; set; }
        public List<string> serviceClasses { get; set; }
        public List<object> trafficRestrictions { get; set; }
    }

    public class ScheduledFlight
    {
        public string carrierFsCode { get; set; }
        public string flightNumber { get; set; }
        public string departureAirportFsCode { get; set; }
        public string arrivalAirportFsCode { get; set; }
        public int stops { get; set; }
        public DateTime departureTime { get; set; }
        public DateTime arrivalTime { get; set; }
        public string flightEquipmentIataCode { get; set; }
        public bool isCodeshare { get; set; }
        public bool isWetlease { get; set; }
        public string serviceType { get; set; }
        public List<string> serviceClasses { get; set; }
        public List<object> trafficRestrictions { get; set; }
        public List<object> codeshares { get; set; }
        public string referenceCode { get; set; }
        public Operator @operator { get; set; }
        public string wetleaseOperatorFsCode { get; set; }
    }

    public class Airline
    {
        public string fs { get; set; }
        public string iata { get; set; }
        public string icao { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public bool active { get; set; }
        public string category { get; set; }
    }

    public class Airport
    {
        public string fs { get; set; }
        public string iata { get; set; }
        public string icao { get; set; }
        public string faa { get; set; }
        public string name { get; set; }
        public string street1 { get; set; }
        public string city { get; set; }
        public string cityCode { get; set; }
        public string stateCode { get; set; }
        public string postalCode { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string regionName { get; set; }
        public string timeZoneRegionName { get; set; }
        public string weatherZone { get; set; }
        public DateTime localTime { get; set; }
        public string utcOffsetHours { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string elevationFeet { get; set; }
        public string classification { get; set; }
        public bool active { get; set; }
    }

    public class Equipment
    {
        public string iata { get; set; }
        public string name { get; set; }
        public bool turboProp { get; set; }
        public bool jet { get; set; }
        public bool widebody { get; set; }
        public bool regional { get; set; }
    }

    public class Appendix
    {
        public List<Airline> airlines { get; set; }
        public List<Airport> airports { get; set; }
        public List<Equipment> equipments { get; set; }
    }

}
