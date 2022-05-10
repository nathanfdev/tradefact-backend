using Core.Models;
using Flare.Api.Model;
using Flare.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flare.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly FlareDbContext _context;

        public CountryController(FlareDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(List<CountryInfo>), Description = "OK Result")]
        [Route(nameof(List))]
        public async Task<IActionResult> List(string search = null)
        {
            IQueryable<Country> query = _context.Countries;
            if (search != null && search.Length > 0)
            {
                query = query.Where(q => EF.Functions.Like(q.Name, $"%{search}%") || EF.Functions.Like(q.Code2, $"%{search}%"));
            }
            List<Country> countries = await query.ToListAsync();
            if (query == null)
            {
                return new NotFoundResult();
            }
            if (search != null && search.Length > 0)
            {
                var ordered = countries.Select(s => new { s.Name, s.Code2, Position = ($"{s.Name}{s.Code2}").ToLower().IndexOf(search.ToLower()) }).OrderBy(o => o.Position).ThenBy(o => o.Name).ThenBy(o => o.Code2);
                return new OkObjectResult(ordered.Select(s => new CountryInfo { Code = s.Code2, Name = s.Name }));
            }
            return new OkObjectResult(countries.Select(s=> new CountryInfo { Code = s.Code2, Name = s.Name }).OrderBy(o=> o.Name));
        }
    }
}