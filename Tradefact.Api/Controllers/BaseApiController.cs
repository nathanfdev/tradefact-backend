using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Tradefact.Api.Common.Controllers;
using Tradefact.Api.Infrastructure.Helpers;
using Tradefact.Api.Model;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]

    public class BaseApiController : BaseController
    {

        private static readonly string AcceptLanguageHeader = "accept-language";
        private static readonly string DefaultCulture = "en-us";


        public virtual Guid OrganisationId => this.GetOrganisationId((ClaimsIdentity)User.Identity);
        public virtual OrganisationTypeEnum OrganisationType => this.GetOrganisationType((ClaimsIdentity)User.Identity);

        protected readonly TradefactDbContext _context;

        public BaseApiController(TradefactDbContext context)
        {
            _context = context;
        }

        protected Guid GetOrganisationId(ClaimsIdentity identity)
        {
            var sid = identity.Claims.Where(c => c.Type == "OrganisationId")
                   .Select(c => c.Value).SingleOrDefault();
            return new Guid(sid);
        }

        protected OrganisationTypeEnum GetOrganisationType(ClaimsIdentity identity)
        {
            var orgType = identity.Claims.Where(c => c.Type == "OrganisationType")
                   .Select(c => c.Value).SingleOrDefault();

            return orgType switch
            {
                "1" => OrganisationTypeEnum.SHIPPER,
                "2" => OrganisationTypeEnum.SUPPLIER,
                "3" => OrganisationTypeEnum.BUYER,
                "4" => OrganisationTypeEnum.PARTNER,
                _ => OrganisationTypeEnum.UNKNOWN,
            };
        }

        protected IActionResult Single<T>(T model, Func<T, bool> criteria = null)
        {
            if (model == null)
            {
                return NotFound();
            }
            var isValid = criteria == null || criteria(model);
            if (isValid)
            {
                return Ok(model);
            }

            return NotFound();
        }

        protected void InjectPagedResultHeader<T>(PagedList<T> model)
        {
            var metadata = new
            {
                model.TotalCount,
                model.PageSize,
                model.CurrentPage,
                model.TotalPages,
                model.HasNext,
                model.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        }

        protected IDbConnection Connection => new SqlConnection("Server=tcp:sql-tradefact-dev.database.windows.net,1433;Initial Catalog=sqldb-tradefact-dev;Persist Security Info=False;User ID=sql-admin-dev;Password=Tr4dE73ChurchF4ct;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        //protected IActionResult Collection<T>(PagedResult<T> pagedResult, Func<PagedResult<T>, bool> criteria = null)
        //{
        //    if (pagedResult == null)
        //    {
        //        return NotFound();
        //    }
        //    var isValid = criteria == null || criteria(pagedResult);
        //    if (!isValid)
        //    {
        //        return NotFound();
        //    }
        //    if (pagedResult.IsEmpty)
        //    {
        //        return Ok(Enumerable.Empty<T>());
        //    }
        //    Response.Headers.Add("Link", GetLinkHeader(pagedResult));
        //    Response.Headers.Add("X-Total-Count", pagedResult.TotalResults.ToString());

        //    return Ok(pagedResult.Items);
        //}

        //protected async Task<IActionResult> SendAsync<T>(T command,
        //    Guid? resourceId = null, string resource = "") where T : ICommand
        //{
        //    var context = GetContext<T>(resourceId, resource);
        //    await _busPublisher.SendAsync(command, context);

        //    return Accepted(context);
        //}
        // Generate a random string with a given size  
        protected string GenerateReference()
        {
            DateTime _now = DateTime.Now;

            StringBuilder builder = new StringBuilder();
            builder.Append(_now.ToString("yy"));
            builder.Append("-");
            builder.Append(_now.ToString("MM"));
            builder.Append("-");

            Random random = new Random();
            char ch;
            for (int i = 0; i < 5; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }


        protected bool IsAdmin
            => User.IsInRole("admin");

        protected Guid UserId
            => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
                Guid.Empty :
                Guid.Parse(User.Identity.Name);

        protected string Culture
            => Request.Headers.ContainsKey(AcceptLanguageHeader) ?
                    Request.Headers[AcceptLanguageHeader].First().ToLowerInvariant() :
                    DefaultCulture;
    }
}