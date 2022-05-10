using Core.Interfaces;
using Core.Models.Tracking;
using Integration.VTExplorer.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Integration.VTExplorer.Services
{
    public class VTExplorerAISTrackingService : IAISTrackingService
    {
        private readonly ILogger<VTExplorerAISTrackingService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly VTExplorerOptions _options;

        public VTExplorerAISTrackingService(IHttpClientFactory httpClientFactory, ILogger<VTExplorerAISTrackingService> logger, VTExplorerOptions options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options;
        }


        private IAsyncPolicy<AISTrackingResult> GetRetryPolicy()
        {

            return Policy.HandleResult<AISTrackingResult>(r => !r.IsSuccessStatusCode && (r.StatusCode == HttpStatusCode.TooManyRequests || r.StatusCode == HttpStatusCode.RequestTimeout || r.StatusCode == HttpStatusCode.NotFound))
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(retryAttempt), onRetry: (response, timespan, retryCount) =>
                {
                    _logger.LogDebug($"Flightstats Service: Request failed with {response.Result.StatusCode}. Waiting {timespan} before next retry. Retry attempt {retryCount}");
                });
        }

        public Dictionary<string, string> GetOptions()
        {
            Dictionary<string, string> retValue = new Dictionary<string, string>();
            retValue.Add("Enabled", this._options.Enabled.ToString());
            retValue.Add("ApplicationKey", this._options.ApplicationKey);
            retValue.Add("EnableSatTracking", this._options.EnableSatTracking.ToString());
            retValue.Add("Retries", this._options.Retries.ToString());
            return retValue;
        }


        public async Task<AISTrackingResult> GetVesselPosition(AISTrackCriteria criteria)
        {

            try
            {
                var client = _httpClientFactory.CreateClient("vtExplorer");

                return await GetRetryPolicy().ExecuteAsync(async () =>
                {
                    var response = await client.GetAsync(GenerateVesselTrackUrl(criteria));
                    if (response.IsSuccessStatusCode)
                    {
                        // Handle success
                        var content = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<List<VesselsResponse>>(content);
                        AISTrackingResult aisResult = new AISTrackingResult
                        {
                            Status = new AISTrackingResultStatus
                            {
                                IsSuccess = true,
                                StatusMessage = "Success",
                                StatusCode = response.StatusCode
                            },
                            Vessels = new List<AISTrack>()
                        };
                        foreach (VesselsResponse vr in result)
                        {
                            aisResult.Vessels.Add(new AISTrack
                            {
                                Mmsi = vr.Ais.Mmsi,
                                Timestamp = vr.Ais.Timestamp,
                                Latitude = vr.Ais.Latitude,
                                Longitude = vr.Ais.Longitude,
                                Course = vr.Ais.Course,
                                Speed = vr.Ais.Speed,
                                Heading = vr.Ais.Heading,
                                Navstat = vr.Ais.Navstat,
                                Imo = vr.Ais.Imo,
                                Name = vr.Ais.Name,
                                Callsign = vr.Ais.Callsign,
                                Type = vr.Ais.Type,
                                A = vr.Ais.A,
                                B = vr.Ais.B,
                                C = vr.Ais.C,
                                D = vr.Ais.D,
                                Draught = vr.Ais.Draught,
                                Destination = vr.Ais.Destination,
                                EtaAis = vr.Ais.EtaAis,
                                Eta = vr.Ais.Eta,
                                Src = vr.Ais.Src,
                                Zone = vr.Ais.Zone,
                                Eca = vr.Ais.Eca
                            });
                        }
                        return aisResult;
                    }
                    else
                    {
                        // Handle failure
                        return AISTrackingResult.FailedAISTrackingResult(AISTrackingStatusMessages.GeneralFailure, response.StatusCode);
                    }

                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to connect to FlightStats API {ex}");
            }
            return AISTrackingResult.FailedAISTrackingResult(AISTrackingStatusMessages.GeneralFailure);
        }


        private string GenerateVesselTrackUrl(AISTrackCriteria criteria)
        {
            var requestUrl = new StringBuilder();
            requestUrl.Append($"vessels?&userkey={_options.ApplicationKey}");
            if (criteria.IMO.Count > 0)
            {
                requestUrl.Append($"&imo={String.Join(',', criteria.IMO)}");
            }
            if (criteria.MMSI.Count > 0)
            {
                requestUrl.Append($"&mmsi={String.Join(',', criteria.MMSI)}");
            }
            requestUrl.Append($"&format=json");
            if (_options.EnableSatTracking)
            {
                requestUrl.Append($"&sat=1");
            }
            else
            {
                requestUrl.Append($"&sat=0");
            }
            return requestUrl.ToString();
        }
    }


}
