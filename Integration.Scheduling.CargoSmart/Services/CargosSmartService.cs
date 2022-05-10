using Core.Interfaces;
using Core.Interfaces.Scheduling;
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

namespace Integration.CargoSmart
{
    public class CargosSmartService : ISeaSchedulesService
    {
        private readonly ILogger<CargosSmartService> _logger;
        private readonly CargoSmartOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public CargosSmartService(IHttpClientFactory httpClientFactory, ILogger<CargosSmartService> logger, CargoSmartOptions options)
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
            retValue.Add("EnableNearbySchedules", this._options.EnableNearbySchedules.ToString());
            retValue.Add("SearchDuration", this._options.SearchDuration.ToString());
            retValue.Add("Retries", this._options.Retries.ToString());

            return retValue;
        }
        public async Task<RouteScheduleResult> GetRouteSchedule(RouteScheduleCriteria criteria)
        {

            try
            {
                var client = _httpClientFactory.CreateClient("cargosmart");
                client.DefaultRequestHeaders.Add("appKey", _options.ApplicationKey);
                return await GetRetryPolicy().ExecuteAsync(async () =>
                {
                    var response = await client.GetAsync(GenerateCargosSmartServiceUrl(criteria));
                    if (response.IsSuccessStatusCode)
                    {
                        // Handle success
                        var content = await response.Content.ReadAsStringAsync();

                        CargoSmartScheduleResponse cg_response = new CargoSmartScheduleResponse();
                        try
                        {
                            cg_response = JsonConvert.DeserializeObject<CargoSmartScheduleResponse>(content);
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                        if (cg_response.routeGroupsList.Count > 0)
                        {
                            RouteScheduleResult schedules_result = BuildScheduleResult(cg_response);
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
                _logger.LogError($"Unable to connect to Cargosmart API {ex}");
            }
            return RouteScheduleResult.FailedRouteScheduleResult(RouteScheduleStatusMessages.GeneralFailure);
        }

        private RouteScheduleResult BuildScheduleResult(CargoSmartScheduleResponse feed)
        {
            RouteScheduleResult res = new RouteScheduleResult();
            foreach (var carrier_route_group in feed.routeGroupsList)
            {
                foreach (var route in carrier_route_group.route)
                {
                    RouteSchedule rs = new RouteSchedule();
                    rs.CarrierCode = route.carrierScac;
                    rs.CarrierName = carrier_route_group.carrier.name;
                    rs.PortOfLoading = new WayPoint
                    {
                        ETD = route.por.etd,
                        Timezone = route.por.location.timezone,
                        FullName = route.por.location.name,
                        Name = route.por.location.name,
                        LocationCode = route.por.location.unlocode
                    };
                    rs.PortOfDischarge = new WayPoint
                    {
                        ETA = route.fnd.eta,
                        Timezone = route.fnd.location.timezone,
                        FullName = route.fnd.location.name,
                        Name = route.fnd.location.name,
                        LocationCode = route.fnd.location.unlocode
                    };
                    foreach (var leg in route.leg)
                    {
                        rs.Legs.Add(this.BuildLeg(leg));
                    }
                    rs.TransportSummary = $"{rs.PortOfLoading.Name} > {rs.PortOfDischarge.Name}";
                    if (rs.Legs.Count > 0)
                    {
                        rs.TransportSummary = String.Join(',', rs.Legs.Select(s => $"{s.FromPoint.LocationCode}>{s.ToPoint.LocationCode}"));
                    }
                    rs.TransitTimeDays = route.transitTime;
                    rs.TransitTimeHours = rs.TransitTimeDays * 24;
                    rs.TransitTimeMinutes = rs.TransitTimeHours * 60;
                    res.RouteSchedules.Add(rs);
                }
            }
            res.Status.IsSuccess = true;
            return res;
        }

        private Core.Models.Leg BuildLeg(Leg route_leg)
        {
            Core.Models.Leg leg = new Core.Models.Leg();
            leg.FromPoint = new WayPoint
            {
                ETD = route_leg.fromPoint.etd,
                Timezone = route_leg.fromPoint.location.timezone,
                FullName = route_leg.fromPoint.location.name,
                Name = route_leg.fromPoint.location.name,
                LocationCode = route_leg.fromPoint.location.unlocode,
            };
            leg.ToPoint = new WayPoint
            {
                ETA = route_leg.toPoint.eta,
                Timezone = route_leg.toPoint.location.timezone,
                FullName = route_leg.toPoint.location.name,
                Name = route_leg.toPoint.location.name,
                LocationCode = route_leg.toPoint.location.unlocode,
            };
            return leg;
        }

        private WayPoint BuildWayPoint(Port waypoint)
        {
            return new WayPoint
            {
                LocationCode = waypoint.location.unlocode,
                Name = waypoint.location.uc_name,
                Timezone = waypoint.location.timezone,
            };
        }


        private string GenerateCargosSmartServiceUrl(RouteScheduleCriteria criteria)
        {
            string fromDate = criteria.EarliestDate.ToString("yyyy-MM-ddThh:mm:ss.sssZ");

            var requestUrl = new StringBuilder($"https://apis.cargosmart.com/openapi/");
            requestUrl.Append($"schedules/routeschedules?");
            requestUrl.Append($"porID={criteria.PortOfLoading}");
            requestUrl.Append($"&fndID={criteria.PortOfDischarge}");
            if (!String.IsNullOrEmpty(criteria.Carrier))
            {
                requestUrl.Append($"&carrier={criteria.Carrier.ToUpper()}");
            }
            requestUrl.Append($"&searchDuration={_options.SearchDuration}");
            requestUrl.Append($"&departureFrom={fromDate}");
            requestUrl.Append($"&enableNearbySchedules={_options.EnableNearbySchedules.ToString()}");
            return requestUrl.ToString();
        }

    }
}
