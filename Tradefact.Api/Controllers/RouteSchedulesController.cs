using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Caching;
using Core.Common;
using Core.Common.Caching;
using Core.Interfaces;
using Core.Models;
using Core.Models.ReferenceData;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSwag.Annotations;
using Tradefact.Api.Model;
using Tradefact.Application.Common.Models;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteSchedulesController : BaseApiController
    {
        private readonly IAirSchedulesService _airService;
        private readonly ISeaSchedulesService _seaService;
        private readonly IMemoryCache _memoryCache;


        public RouteSchedulesController(TradefactDbContext context, IAirSchedulesService _airService, ISeaSchedulesService _seaService, IMemoryCache memoryCache) : base(context)
        {
            this._airService = _airService;
            this._seaService = _seaService;
            _memoryCache = memoryCache;
        }

        [HttpGet()]
        [SwaggerResponse("200", typeof(ListResource<RouteSchedule>), Description = "OK Result")]
        [Route("Air")]
        public async Task<IActionResult> Air([FromQuery] RouteScheduleCriteria criteria, [FromQuery] PagedResultParameters @params)
        {
            var cacheKey = CacheKey.With(GetType(), GetSchedulesCacheKey(criteria, SchedulesRequestMode.Air));

            RouteScheduleResult cacheEntry;
            // Look for cache key.
            if (!_memoryCache.TryGetValue(cacheKey, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = await _airService.GetRouteSchedule(criteria);
                if (cacheEntry != null && cacheEntry.RouteSchedules.Count > 0)
                {
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(180));

                    // Save data in cache.
                    _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
                }
            }
            if (cacheEntry == null)
            {
                cacheEntry = new RouteScheduleResult();
            }
            IPagedList<RouteSchedule> result = await cacheEntry.RouteSchedules.ToPagedListAsync(@params.PageNumber, @params.PageSize);
            return this.HandleSuccessResponse(new ListResource<RouteSchedule>(result));
        }

        [HttpGet()]
        [SwaggerResponse("200", typeof(ListResource<RouteSchedule>), Description = "OK Result")]
        [Route("Sea")]
        public async Task<IActionResult> Sea([FromQuery] RouteScheduleCriteria criteria, [FromQuery] PagedResultParameters @params)
        {
            var cacheKey = CacheKey.With(GetType(), GetSchedulesCacheKey(criteria, SchedulesRequestMode.Sea));
            RouteScheduleResult cacheEntry;
            // Look for cache key.
            if (!_memoryCache.TryGetValue(cacheKey, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = await _seaService.GetRouteSchedule(criteria);
                if (cacheEntry != null && cacheEntry.RouteSchedules.Count > 0)
                {
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(180));

                    // Save data in cache.
                    _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
                }
            }
            if (cacheEntry == null)
            {
                cacheEntry = new RouteScheduleResult();
            }
            IPagedList<RouteSchedule> result = await cacheEntry.RouteSchedules.ToPagedListAsync(@params.PageNumber, @params.PageSize);
            return this.HandleSuccessResponse(new ListResource<RouteSchedule>(result));
        }

        private enum SchedulesRequestMode
        {
            Air,
            Sea
        }

        private string GetSchedulesCacheKey(RouteScheduleCriteria criteria, SchedulesRequestMode transitMode)
        {
            if (transitMode == SchedulesRequestMode.Sea) {
                DateTime cachedate = criteria.EarliestDate.AddDays(DayOfWeek.Sunday - criteria.EarliestDate.DayOfWeek).Date;
                string carrier = !String.IsNullOrEmpty(criteria.Carrier) ? criteria.Carrier.ToLower() : "";

                return $"Backend:Sea{carrier}_{criteria.PortOfLoading.ToLower()}_{criteria.PortOfDischarge.ToLower()}_{cachedate.ToString("yyyy-MM-dd")}";
            }
            return $"Backend:Air_{criteria.PortOfLoading.ToLower()}_{criteria.PortOfDischarge.ToLower()}_{criteria.EarliestDate.Date.ToString("yyyy-MM-dd")}";
        }

        [HttpGet()]
        [Route("config/sea")]
        public IActionResult SeaConfig()
        {
            return this.HandleSuccessResponse(_seaService.GetOptions());
        }

        [HttpGet()]
        [Route("config/air")]
        public IActionResult AirConfig()
        {
            return this.HandleSuccessResponse(_airService.GetOptions());
        }
    }

}