using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class RouteScheduleResult
    {
        public static RouteScheduleResult FailedRouteScheduleResult(string failureMessage, System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.BadRequest)
        {
            if (string.IsNullOrEmpty(failureMessage))
                throw new ArgumentNullException(nameof(failureMessage));

            return new RouteScheduleResult
            {
                Status = new RouteScheduleResultStatus
                {
                    IsSuccess = false,
                    StatusMessage = failureMessage,
                    StatusCode = statusCode
                }
            };
        }

        public List<RouteSchedule> RouteSchedules { get; set; } = new List<RouteSchedule>();
        /// <summary>
        /// Indicates the status of the optimize route process
        /// </summary>
        public RouteScheduleResultStatus Status { get; set; } = new RouteScheduleResultStatus();

        [JsonIgnore]
        public bool IsSuccessStatusCode => this.Status.StatusCode == System.Net.HttpStatusCode.OK;

        [JsonIgnore]
        public System.Net.HttpStatusCode StatusCode => this.Status.StatusCode;

    }

    [TypescriptAutoGeneration]
    public class RouteSchedule
    {
        public int RouteID { get; set; }
        public int PointPairID { get; set; }
        public int? Changes { get; set; }
        public string CarrierCode { get; set; }
        public string CarrierName { get; set; }
        public DateTime? defaultCutoff { get; set; }
        public WayPoint PortOfLoading { get; set; }
        public WayPoint PortOfDischarge { get; set; }
        public bool IsDirect { get; set; }
        public decimal TransitTimeDays { get; set; }
        public decimal TransitTimeHours { get; set; }
        public decimal TransitTimeMinutes { get; set; }

        [JsonIgnore]
        public List<Leg> Legs { get; set; } = new List<Leg>();
        public string TransportSummary { get; set; }

        public DateTime? ETD => (this.PortOfLoading.ETD.HasValue) ? this.PortOfLoading.ETD.GetValueOrDefault() : null;
        public DateTime? ETA => (this.PortOfDischarge.ETD.HasValue) ? this.PortOfDischarge.ETD.GetValueOrDefault() : null;
        public DateTime? CutOff { get; set; }
    }

    [TypescriptAutoGeneration]
    public class Leg
    {
        public Carrier Carrier { get; set; }
        public WayPoint FromPoint { get; set; }
        public WayPoint ToPoint { get; set; }
        public string TransportMode { get; set; }
        public Service Service { get; set; }
        public Vessel Vessel { get; set; }
        public string VoyageNumber { get; set; }
        public int TransitTime { get; set; }
    }

    [TypescriptAutoGeneration]
    public class WayPoint
    {
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }

        public string Id { get; set; }
        public string LocationCode { get; set; }
        public string Name { get; set; }
        public List<double> Geo { get; set; }
        public string FullName { get; set; }
        public string Timezone { get; set; }
    }

    public class Service
    {
        public int serviceID { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }


    public class RouteScheduleResultStatus
    {
        public bool IsSuccess { get; set; } = true;
        public string StatusMessage { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
    }

    /// <summary>
    /// Defines the properties required as input for route schedule request
    /// </summary>
    public class RouteScheduleCriteria
    {
        public RouteScheduleCriteria()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="RouteScheduleCriteria"/>
        /// </summary>
        public RouteScheduleCriteria(string portOfLoading, string portOfDischarge, DateTime earliestDate, string carrier = null, List<string> includeCarriers = null)
        {
            if (string.IsNullOrWhiteSpace(portOfLoading))
            {
                throw new ArgumentException(nameof(portOfLoading));
            }

            if (string.IsNullOrWhiteSpace(portOfDischarge))
            {
                throw new ArgumentException(nameof(portOfDischarge));
            }

            if (earliestDate < DateTime.UtcNow)
            {
                throw new ArgumentException($"{nameof(earliestDate)} should be in the future.");
            }

            // Ensure that all string values are URL encoded            
            PortOfLoading = Uri.EscapeUriString(portOfLoading);
            PortOfDischarge = Uri.EscapeUriString(portOfDischarge);
            if (carrier != null)
            {
                Carrier = Uri.EscapeUriString(portOfDischarge);
            }
            EarliestDate = earliestDate;
        }

        /// <summary>
        /// The start address for the route
        /// </summary>
        public string PortOfLoading { get; set; }

        /// <summary>
        /// The end address for the route
        /// </summary>
        public string PortOfDischarge { get; set; }

        /// <summary>
        /// Specific carrier specified for search
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// The earliest possible departure time
        /// </summary>
        public DateTime EarliestDate { get; set; }
    }

    public static class RouteScheduleStatusMessages
    {
        /// <summary>
        /// A general failure message when a specific cause cannot be / should not be provided to the end user
        /// </summary>
        public static string GeneralFailure = "Unable to get schedule for route";

        /// <summary>
        /// Indicates that the schedule request process completed and valid 
        /// </summary>
        public static string ScheduleAvailable = "Route schedule available";

        /// <summary>
        /// Indicates that the optimization process completed but no waypoints where included
        /// </summary>
        public static string ZeroResults = "Unable to find one or more of the request schedules";
    }
}
