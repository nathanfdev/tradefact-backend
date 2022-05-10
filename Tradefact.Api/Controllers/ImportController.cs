using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ImportController : ControllerBase
    {
        private readonly TradefactDbContext _context;

        public ImportController(TradefactDbContext context)
        {
            _context = context;
        }
 
        [HttpGet]
        [SwaggerResponse(200, typeof(Import[]), Description = "OK result")]
        [Route(nameof(ListImports))]
        public IActionResult ListImports()
        {
            List<Import> imports = new List<Import>();

            for (int i = 0; i < 10; i++)
            {
                imports.Add(new Import { Name = $"A{i}_to_B{i}" });
            }
            return new OkObjectResult(imports);
        }
    }
}