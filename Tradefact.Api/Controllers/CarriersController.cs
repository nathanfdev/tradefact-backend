using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarriersController : ControllerBase
    {
        private readonly TradefactDbContext _context;

        public CarriersController(TradefactDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(Carrier), Description = "OK Result" )]
        // [OpenApiTag("carriers")]
        [Route(nameof(List))]
        public async Task<IActionResult> List()
        {
            List<Carrier> carriers = await _context.Carriers.Where(q=>q.Active).ToListAsync();
            if (carriers == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(carriers);
        }
    }
}