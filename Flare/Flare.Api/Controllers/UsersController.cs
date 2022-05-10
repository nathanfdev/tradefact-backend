using Core.Common;
using Core.Enums;
using Core.Models;
using Flare.Api.Model;
using Flare.Data;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Flare.Api.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly AppSettings _appSettings;

        public UsersController(IOptions<AppSettings> appSettings, ILogger<UserController> logger, FlareDbContext context) : base(context)
        {
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<UserResource>), Description = "OK Result")]
        [Route("")]
        public async Task<IActionResult> Get([FromQuery] PagedResultParameters @params, string search = null)
        {
            var users_query = _context.Users.Where(q => q.OrganisationId == this.OrganisationId);
            if (search != null && search.Length > 1)
            {
                users_query = users_query.Where(q => EF.Functions.Like(q.FullName, $"%{search}%") || EF.Functions.Like(q.Email, $"%{search}%"));
            }
            var all_users = users_query.ToList();

            var invites_pending = _context.InvitationLog.Where(q => q.MemberOfOrganisationId == this.OrganisationId);
            if (search != null && search.Length > 1)
            {
                invites_pending = invites_pending.Where((q => (EF.Functions.Like(q.GivenName, $"%{search}%") || EF.Functions.Like(q.EmailAddress, $"%{search}%")) && q.ActivationStatus != AccountActivationStatus.Complete));
            }
            foreach (var invite in invites_pending)
            {
                if (!all_users.Any(q => q.Email == invite.EmailAddress)) all_users.Add(new ApplicationUser
                {
                    Email = invite.EmailAddress,
                    FullName = invite.GivenName,
                    GivenName = invite.GivenName,
                    Status = UserStatus.Pending,
                    RegistrationDate = invite.CreationDateInternal
                });
            }
            List<UserResource> all_users_sorted = all_users.OrderByDescending(x => x.RegistrationDate).Adapt<List<UserResource>>();
            IPagedList<UserResource> users = await all_users_sorted.ToPagedListAsync(@params.PageNumber, @params.PageSize);
            return new OkObjectResult(new ListResource<UserResource>(users));
        }

        [HttpGet("{id}")]
        [SwaggerResponse("200", typeof(UserResource), Description = "OK Result")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _context.Users.Include(i => i.Address).FirstOrDefaultAsync(q => q.OrganisationId == this.OrganisationId && q.Id == id);
            return new OkObjectResult(user.Adapt<UserResource>());
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(UserResource), Description = "Created result")]
        public virtual async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            ApplicationUser existing_user = await _context.Users.FirstOrDefaultAsync(q => q.Id == request.Id.ToString() && q.OrganisationId == this.OrganisationId);

            bool userStatusChanged = request.Status != existing_user.Status;

            existing_user.FullName = request.FullName;
            existing_user.PhoneNumber = request.PhoneNumber;
            existing_user.LocationId = request.LocationId;
            existing_user.GivenName = request.GivenName;
            existing_user.Surname = request.FamilyName;
            existing_user.Status = request.Status;

            if (request.Role != null)
            {
                existing_user.IsAdmin = request.Role.Any(q => q.ToLower() == "admin");
            }

            string b2cid = existing_user.ExternalProverUUID.ToString();

            if (!string.IsNullOrEmpty(b2cid))
            {
                string clientId = _appSettings.GraphApiClientId;
                string tenantId = _appSettings.GraphApiTenantId;
                string clientSecret = _appSettings.GraphApiClientSecret;

                IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithTenantId(tenantId)
                    .WithClientSecret(clientSecret)
                    .Build();

                ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

                GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                var user = new Microsoft.Graph.User
                {
                    DisplayName = request.FullName,
                    GivenName = request.GivenName,
                    Surname = request.FamilyName
                };

                try
                {
                    await graphClient.Users[b2cid]
                        .Request()
                        .UpdateAsync(user);
                }
                catch
                {
                    // Catch rare occasion when user not properly set up in graph
                }
            }

            _ = await _context.SaveChangesAsync();

            return new OkObjectResult(existing_user.Adapt<UserResource>());
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Disable(string id)
        {
            var existing_user = await _context.Users.FirstOrDefaultAsync(q => q.Id == id.ToString() && q.OrganisationId == this.OrganisationId);
            existing_user.Status = UserStatus.Disabled;

            _ = await _context.SaveChangesAsync();

            string b2cid = existing_user.ExternalProverUUID.ToString();

            if (!string.IsNullOrEmpty(b2cid))
            {
                string clientId = _appSettings.GraphApiClientId;
                string tenantId = _appSettings.GraphApiTenantId;
                string clientSecret = _appSettings.GraphApiClientSecret;

                IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithTenantId(tenantId)
                    .WithClientSecret(clientSecret)
                    .Build();

                ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

                GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                var user = new Microsoft.Graph.User
                {
                    AccountEnabled = false
                };

                await graphClient.Users[b2cid]
                    .Request()
                    .UpdateAsync(user);
            }

            return new OkObjectResult(existing_user.Adapt<UserResource>());
        }

    }
}
