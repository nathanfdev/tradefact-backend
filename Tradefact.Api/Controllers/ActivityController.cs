using Core.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Application;
using Tradefact.Application.Models.Activity;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ActivityController : BaseApiController
    {
        public ActivityController(TradefactDbContext context) : base(context)
        {
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<ActivityResource>), Description = "Ok")]
        public async Task<IActionResult> GetActivities([FromQuery] PagedResultParameters @params, [FromQuery] ActivitySearchCriteria criteria)
        {
            List<ActivityResource> activities = await _context.Activities
                .Where(q => (!criteria.Entities.Any() || criteria.Entities.Contains(q.Entity)) && q.CreationDateInternal > DateTime.Now.AddDays(-7) && q.OrganisationId == this.OrganisationId)
                .Select(s => new ActivityResource
                {
                    Type = s.Type,
                    Text1 = s.Text1,
                    Text2 = s.Text2,
                    Description = s.Description,
                    Entity = s.Entity,
                    Data = JsonConvert.DeserializeObject(s.Data),
                    CreationDateInternal = s.CreationDateInternal
                })
                .OrderByDescending(q => q.CreationDateInternal)
                .ToListAsync();

            IPagedList<ActivityResource> paginatedActivites = activities.ToPagedList(@params.PageNumber, @params.PageSize);

            return new OkObjectResult(new ListResource<ActivityResource>(paginatedActivites));
        }
    }
}
