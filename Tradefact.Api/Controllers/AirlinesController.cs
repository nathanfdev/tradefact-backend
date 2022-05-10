using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirlinesController : ControllerBase
    {
        private readonly TradefactDbContext _context;

        public AirlinesController(TradefactDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(Airline), Description = "List of Airlines")]
        public async Task<IActionResult> List()
        {
            List<Airline> airlines = await _context.AirLines.Where(q => q.Active).OrderBy(o=>o.Name).ToListAsync();
            if (airlines == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(airlines);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(Airline), Description = "List of airlines with tracking provision within Tradefact")]
        [Route("Trackable")]
        public async Task<IActionResult> TrackableList()
        {
            List<Airline> airlines = await _context.AirLines.Where(q => q.Active && q.Tracking.Enabled).OrderBy(o => o.Name).ToListAsync();
            if (airlines == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(airlines);
        }
    }
}