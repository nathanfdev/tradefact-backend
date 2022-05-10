using Core.Common;
using Core.Enums;
using Flare.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Tradefact.Api.Common.Controllers;

namespace Flare.Api.Controllers
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

        protected readonly FlareDbContext _context;

        public BaseApiController(FlareDbContext context)
        {
            _context = context;
        }

        protected Guid GetOrganisationId(ClaimsIdentity identity)
        {
            var sid = identity.Claims.Where(c => c.Type == "OrganisationId")
                   .Select(c => c.Value).SingleOrDefault();
            return new Guid(sid);
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