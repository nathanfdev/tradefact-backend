using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.Models.Tracking
{
    public class AISTrackingResult
    {
        public static AISTrackingResult FailedAISTrackingResult(string failureMessage, System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.BadRequest)
        {
            if (string.IsNullOrEmpty(failureMessage))
                throw new ArgumentNullException(nameof(failureMessage));

            return new AISTrackingResult
            {
                Status = new AISTrackingResultStatus
                {
                    IsSuccess = false,
                    StatusMessage = failureMessage,
                    StatusCode = statusCode
                }
            };
        }

        public List<AISTrack> Vessels { get; set; } = new List<AISTrack>();
        /// <summary>
        /// Indicates the status of the optimize route process
        /// </summary>
        public AISTrackingResultStatus Status { get; set; } = new AISTrackingResultStatus();

        [JsonIgnore]
        public bool IsSuccessStatusCode => this.Status.StatusCode == System.Net.HttpStatusCode.OK;

        [JsonIgnore]
        public System.Net.HttpStatusCode StatusCode => this.Status.StatusCode;
    }


    public class AISTrackingResultStatus
    {
        public bool IsSuccess { get; set; } = true;
        public string StatusMessage { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
    }

    /// <summary>
    /// Defines the properties required as input for route schedule request
    /// </summary>
    public class AISTrackCriteria
    {
        public AISTrackCriteria()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="AISTrackCriteria"/>
        /// </summary>
        public AISTrackCriteria(List<string> mmsi, List<string> imo)
        {
            MMSI = mmsi;
            IMO = imo;
        }

        /// <summary>
        /// List Vessel MMSI numbers to be tracked
        /// </summary>
        public List<string> MMSI { get; set; } = new List<string>();

        /// <summary>
        /// List Vessel IMO numbers to be tracked
        /// </summary>
        public List<string> IMO { get; set; } = new List<string>();

    }

    public static class AISTrackingStatusMessages
    {
        /// <summary>
        /// A general failure message when a specific cause cannot be / should not be provided to the end user
        /// </summary>
        public static string GeneralFailure = "Unable to get schedule for route";

        /// <summary>
        /// Indicates that the schedule request process completed and valid 
        /// </summary>
        public static string TrackingAvailable = "Route schedule available";

        /// <summary>
        /// Indicates that the optimization process completed but no waypoints where included
        /// </summary>
        public static string ZeroResults = "Unable to find one or more of the requested vessels";
    }
}
