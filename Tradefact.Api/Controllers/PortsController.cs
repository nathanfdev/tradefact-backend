using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Tradefact.Api.Model.Port.Response;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortsController : ControllerBase
    {
        private readonly TradefactDbContext _context;

        public PortsController(TradefactDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(List<PortInfo>), Description = "OK Result")]
        [Route(nameof(List))]
        public async Task<IActionResult> List(string type = null, string search = null)
        {
            IQueryable<Location> query = _context.Locations.Include(i => i.Country);
            type = type ?? "na";

            switch (type.ToLower())
            {
                case "sea":
                    query = query.Where(q => q.Port);
                    break;
                case "air":
                    query = query.Where(q => q.Airport);
                    break;
                default:
                    break;
            }

            if (search != null && search.Length > 1)
            {
                query = query.Where(q => EF.Functions.Like(q.LocCode, $"%{search}%") || EF.Functions.Like(q.Name, $"%{search}%") || EF.Functions.Like(q.Country.Name, $"%{search}%"));
            }
            List<Location> locations = await query.ToListAsync();
            if (locations == null)
            {
                return new NotFoundResult();
            }
            if (search != null && search.Length > 1)
            {
                var ordered = locations.Select(s => new { Code = s.LocCode, Name = s.Name.ToUpper(), CountryCode = s.CountryCode, Country = s.Country.Name.ToUpper(), Position = ($"{s.LocCode}{s.Name}{s.Country.Name}").ToLower().IndexOf(search.ToLower()) }).OrderByDescending(o => o.Code.Contains("_ANY")).ThenBy(o => o.Position).ThenBy(o => o.CountryCode).ThenBy(o => o.Name);
                return new OkObjectResult(ordered.Select(s => new PortInfo { Code = s.Code, Name = s.Name.ToUpper(), CountryCode = s.CountryCode, Country = s.Country.ToUpper() }));
            }
            return new OkObjectResult(locations.Select(s=> new PortInfo { Code = s.LocCode, Name = s.Name.ToUpper(), CountryCode = s.CountryCode, Country = s.Country.Name.ToUpper() }).OrderBy(o=>o.Country).ThenBy(o => o.Name));
        }

    }
}