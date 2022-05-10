using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Caching;
using Core.Common.Caching;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NSwag.Annotations;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerTypeController : ControllerBase
    {
        private readonly TradefactDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public ContainerTypeController(TradefactDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ContainerType), Description = "OK Result")]
        [Route(nameof(List))]
        public async Task<IActionResult> List([FromQuery] ContainerTypeEnum containerType = ContainerTypeEnum.ALL)
        {
            var cacheKey = CacheKey.With(GetType(), "Backend:::ContainerTypes");
            List<ContainerType> container_types = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                return (await _context.ContainerTypes.Where(q => q.Active).ToListAsync());
            });
                
            if (container_types == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(containerType switch
            {
                ContainerTypeEnum.AIR => container_types.Where(q => q.Air).ToList(),
                ContainerTypeEnum.SEA => container_types.Where(q => q.Sea).ToList(),
                ContainerTypeEnum.ROAD => container_types.Where(q => q.Road).ToList(),
                _ => container_types,
            });
        }

    }
}