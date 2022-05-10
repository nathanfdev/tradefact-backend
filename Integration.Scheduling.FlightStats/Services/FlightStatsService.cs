using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Integration.FlightStats
{
    public class FlightStatsService : IAirSchedulesService
    {
        private readonly ILogger<FlightStatsService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FlightStatsOptions _options;

        public FlightStatsService(IHttpClientFactory httpClientFactory, ILogger<FlightStatsService> logger, FlightStatsOptions options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options;
        }


        private IAsyncPolicy<RouteScheduleResult> GetRetryPolicy()
        {

            return Policy.HandleResult<RouteScheduleResult>(r => !r.IsSuccessStatusCode && (r.StatusCode == HttpStatusCode.TooManyRequests || r.StatusCode == HttpStatusCode.RequestTimeout || r.StatusCode == HttpStatusCode.NotFound))
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
            retValue.Add("ApplicationId", this._options.ApplicationId.ToString());
            retValue.Add("CodeType", this._options.CodeType.ToString());
            retValue.Add("Retries", this._options.Retries.ToString());
            return retValue;
        }


        public async Task<RouteScheduleResult> GetRouteSchedule(RouteScheduleCriteria criteria)
        {

            try
            {
                var client = _httpClientFactory.CreateClient("flightstats");

                return await GetRetryPolicy().ExecuteAsync(async () =>
                {
                    var response = await client.GetAsync(GenerateFlightStatsServiceUrl(criteria));
                    if (response.IsSuccessStatusCode)
                    {
                        // Handle success
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<FlightStatsResponse>(content);

                        if (result.HasErrors)
                        {
                            return RouteScheduleResult.FailedRouteScheduleResult(result.Error.ErrorMessage, (HttpStatusCode)result.Error.HttpStatusCode);
                        }

                        if (result.HasResults)
                        {
                            RouteScheduleResult schedules_result = BuildScheduleResult(result);
                            return schedules_result;
                        }
                        else
                        {
                            return RouteScheduleResult.FailedRouteScheduleResult(RouteScheduleStatusMessages.ZeroResults, response.StatusCode);
                        }
                    }
                    else
                    {
                        // Handle failure
                        return RouteScheduleResult.FailedRouteScheduleResult(RouteScheduleStatusMessages.GeneralFailure, response.StatusCode);
                    }

                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to connect to FlightStats API {ex}");
            }
            return RouteScheduleResult.FailedRouteScheduleResult(RouteScheduleStatusMessages.GeneralFailure);
        }

        private RouteScheduleResult BuildScheduleResult(FlightStatsResponse feed)
        {
            RouteScheduleResult res = new RouteScheduleResult();
            foreach (var item in feed.connections)
            {
                RouteSchedule rs = new RouteSchedule();

                StringBuilder codes = new StringBuilder();
                StringBuilder name = new StringBuilder();
                bool isfirstLeg = true;
                foreach (var flight in item.scheduledFlight)
                {
                    Leg leg = this.BuildLeg(flight, feed.appendix);
                    if (!isfirstLeg)
                    {
                        codes.Append("-");
                        name.Append(" - ");
                    }
                    codes.Append(leg.Carrier.SCAC);
                    name.Append(leg.Carrier.Name);
                    if (isfirstLeg) isfirstLeg = false;
                    rs.Legs.Add(leg);
                }

                rs.CarrierCode = codes.ToString();
                rs.CarrierName = name.ToString();

                rs.PortOfLoading = rs.Legs[0].FromPoint;
                rs.PortOfDischarge = rs.Legs[rs.Legs.Count - 1].ToPoint;

                rs.TransitTimeMinutes = item.elapsedTime;
                rs.TransitTimeHours = rs.TransitTimeMinutes / 60;
                rs.TransitTimeDays = rs.TransitTimeMinutes / (60 * 24);

                rs.TransportSummary = String.Join(',', rs.Legs.Select(s => $"{s.FromPoint.LocationCode}>{s.ToPoint.LocationCode}"));
                rs.IsDirect = (rs.Legs.Count < 2);
                res.RouteSchedules.Add(rs);
            }
            res.Status.IsSuccess = true;

            //{
            //    "carrierFsCode": "EI",
            //        "flightNumber": "123",
            //        "departureAirportFsCode": "DUB",
            //        "arrivalAirportFsCode": "ORD",
            //        "stops": 0,
            //        "departureTime": "2020-05-11T11:35:00.000",
            //        "arrivalTime": "2020-05-11T14:00:00.000",
            //        "flightEquipmentIataCode": "330",
            //        "isCodeshare": false,
            //        "isWetlease": false,
            //        "serviceType": "J",
            //        "trafficRestrictions": [],
            //        "elapsedTime": 505
            //    }


            return res;
        }

        private Leg BuildLeg(ScheduledFlight flight, Appendix app)
        {
            Leg leg = new Leg();

            try
            {
                leg.Carrier = this.BuildCarrier(app.airlines.FirstOrDefault(x => x.iata == flight.carrierFsCode));
                leg.FromPoint = this.BuildWayPoint(app.airports.FirstOrDefault(x => x.fs == flight.departureAirportFsCode));
                leg.FromPoint.ETD = flight.departureTime;

                leg.ToPoint = this.BuildWayPoint(app.airports.FirstOrDefault(x => x.fs == flight.arrivalAirportFsCode));
                leg.ToPoint.ETA = flight.arrivalTime;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return leg;
        }

        private Carrier BuildCarrier(Airline airline)
        {
            return new Carrier
            {
                SCAC = airline?.iata,
                Name = airline?.name,
            };
        }

        private WayPoint BuildWayPoint(Airport waypoint)
        {
            return new WayPoint
            {
                LocationCode = waypoint.iata,
                Name = waypoint.name,
                Timezone = waypoint.timeZoneRegionName,
            };
        }

        private string GenerateFlightStatsServiceUrl(RouteScheduleCriteria criteria)
        {
            var requestUrl = new StringBuilder($"https://api.flightstats.com/flex/connections/rest/v2/json/firstflightout/{criteria.PortOfLoading}/to/{criteria.PortOfDischarge}/leaving_after/{criteria.EarliestDate:yyyy/MM/dd/00/00}");
            requestUrl.Append($"?appId={_options.ApplicationId}");
            requestUrl.Append($"&appKey={_options.ApplicationKey}");
            requestUrl.Append($"&codetype=IATA");
            requestUrl.Append($"&numHours=24");
            requestUrl.Append($"&maxConnections=3");
            requestUrl.Append($"&payloadType=all");
            requestUrl.Append($"&maxResults=50&includeMultipleCarriers=true&includeCodeshares=false");
            // requestUrl.Append($"&extendedOptions=includeDirects+includeCargo+includeSurface");
            return requestUrl.ToString();
        }

        // Service Type
        // L   Charter(Passenger and Cargo and / or Mail) 
        // R   Additional Flights -Passenger / Cargo
        // Q   Scheduled Passenger/ Cargo in Cabin
        // F   Scheduled Cargo / Mail(Loose loaded cargo and / or preloaded devices)
        // M   Scheduled Cargo/ Mail(Mail Only)
        // A   Non - scheduled Cargo / Mail
        // H   Charter(Cargo and / or Mail)
        // V   Scheduled Cargo/ Mail(Surface Vehicle)
    }
}
