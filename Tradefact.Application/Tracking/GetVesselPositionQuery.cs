using Core.Caching;
using Core.Common.Caching;
using Core.Interfaces;
using Core.Models;
using Core.Models.Tracking;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Data;

namespace Tradefact.Application.Tracking
{
    public class GetVesselAISPositionQuery : IRequest<AISTrackingResult>
    {
        public Guid ShipmentId { get; set; }

        private class ShipmentVesselInfo
        {
            public Guid Id { get; set; }
            public string IMO { get; set; }
            public bool IsSeaTrack { get; set; }
        }

        public class GetVesselAISPositionQueryHandler : IRequestHandler<GetVesselAISPositionQuery, AISTrackingResult>
        {
            private readonly TradefactDbContext _context;
            private readonly IAISTrackingService _trackingService;
            private IMemoryCache _memoryCache { get; set; }

            public GetVesselAISPositionQueryHandler(TradefactDbContext context, IAISTrackingService trackingService, IMemoryCache memoryCache)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _trackingService = trackingService ?? throw new ArgumentNullException(nameof(trackingService));
                _memoryCache = memoryCache;
        }

        public async Task<AISTrackingResult> Handle(GetVesselAISPositionQuery request, System.Threading.CancellationToken cancellationToken)
            {
                var vessel = await _context.Shipments.Where(q => q.Id == request.ShipmentId).Select(s=> new ShipmentVesselInfo { Id = s.Id, IMO = s.IMO, IsSeaTrack = s.ShipmentType == Core.Enums.ShipmentTypeEnum.SEA } ).SingleOrDefaultAsync();

                int cache_minutes = 60*6;

                // if (vessel != null && vessel.IsSeaTrack && String.IsNullOrEmpty(vessel.IMO)) vessel.IMO = "9294381";

                if (vessel != null && vessel.IsSeaTrack && vessel.IMO != null)
                {
                    var cacheKey = CacheKey.With(GetType(), $"AIS__{vessel.IMO}");
                    if (!_memoryCache.TryGetValue(cacheKey, out AISTrackingResult result))
                    {
                        using (await AsyncLock.GetLockByKey(cacheKey).LockAsync())
                        {
                            if (!_memoryCache.TryGetValue(cacheKey, out result))
                            {
                                var options = _memoryCache is ITradefactMemoryCache tradefactMemoryCache ? tradefactMemoryCache.GetDefaultCacheEntryOptions() : new MemoryCacheEntryOptions();

                                AISTrackCriteria criteria = new AISTrackCriteria();
                                criteria.IMO.Add(vessel.IMO);
                                result = await _trackingService.GetVesselPosition(criteria);
                                if (result != null && result.IsSuccessStatusCode)
                                {
                                    options.AbsoluteExpiration = result.Vessels[0].Src == "TER" ? new DateTimeOffset(DateTime.UtcNow.AddMinutes(cache_minutes)) : new DateTimeOffset(DateTime.UtcNow.AddMinutes(cache_minutes));
                                    _memoryCache.Set(cacheKey, result, options);
                                }
                            }
                        }
                    }
                    return result;
                };
                return null;
            }
        }
    }

}
