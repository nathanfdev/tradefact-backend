using Core.Models.External;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Tradefact.Data;

namespace Tradefact.External.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]

    public class BaseAPIController : Controller
    {
        protected Guid OrganisationId;
        protected readonly TradefactDbContext _context;
        protected DateTime ActionTime;

        public BaseAPIController(TradefactDbContext context)
        {
            _context = context;
            this.ActionTime = DateTime.Now;
        }

        protected bool CheckApiKey()
        {
            ApiKey auth = _context.ApiKeys.Find(HttpContext.Request.Headers["Tradefact-Api-Key"]);
            if (auth == null)  return false;

            this.OrganisationId = auth.OrganisationId;
            return true;
        }

        protected ActionResult HandleSuccessResponse(object data, HttpStatusCode status = HttpStatusCode.OK)
        {
            return StatusCode((int)status, data);
        }

        protected ActionResult HandleCreatedResponse(object data, HttpStatusCode status = HttpStatusCode.Created)
        {
            return StatusCode((int)status, data);
        }

        protected ActionResult HandleDeletedResponse()
        {
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}
